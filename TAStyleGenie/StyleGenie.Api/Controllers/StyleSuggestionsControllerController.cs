using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StyleGenie.Infrastructure.Data.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace StyleGenie.Api.Controllers
{
    [Route("api/style")]
    [ApiController]
    public class StyleSuggestionsController : ControllerBase
    {
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _cfg;
        private readonly TryOnDbContext _context;
        private static readonly SemaphoreSlim Gate = new(1);

        public StyleSuggestionsController(IHttpClientFactory http, IConfiguration cfg, TryOnDbContext context)
        {
            _http = http;
            _cfg = cfg;
            _context = context;
        }

        [IgnoreAntiforgeryToken]
        [HttpPost("analyze")]
        [RequestSizeLimit(10_000_000)]
        public async Task<IActionResult> Analyze([FromForm] AnalyzeRequest req, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(req.SkinTone) ||
                string.IsNullOrWhiteSpace(req.Gender) ||
                string.IsNullOrWhiteSpace(req.BodyType))
                return BadRequest("Thiếu thông tin đầu vào.");

            var styleList = await _context.Items
                .Where(p => p.IsActive && !string.IsNullOrEmpty(p.Material))
                .Select(p => p.Material!.Trim())
                .Distinct()
                .ToListAsync(ct);

            if (styleList.Count == 0)
                return BadRequest("Chưa có phong cách nào trong DB.");

            var prompt = BuildPrompt(req.SkinTone, req.Gender, req.BodyType, styleList);

            string aiResponse;
            try
            {
                aiResponse = await CallOpenAiAsync(prompt, null, ct);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"AI error: {ex.Message}");
            }

            if (!TryParseJson(aiResponse, out var jsonText))
                return new JsonResult(new { recommendations = new List<object>() });

            JsonElement root;
            try
            {
                using var doc = JsonDocument.Parse(jsonText);
                root = doc.RootElement.Clone();
            }
            catch
            {
                return new JsonResult(new { recommendations = new List<object>() });
            }

            if (!root.TryGetProperty("recommendations", out var recsElement))
                return new JsonResult(new { recommendations = new List<object>() });

            var styles = new List<string>();
            foreach (var item in recsElement.EnumerateArray())
            {
                if (item.TryGetProperty("title", out var titleEl) && titleEl.ValueKind == JsonValueKind.String)
                    styles.Add(titleEl.GetString()!);
            }

            // ✅ Truy vấn sản phẩm
            var recommendations = new List<object>();
            foreach (var style in styles.Take(3))
            {
                var products = await _context.Items
                    .Where(p => p.IsActive && p.Material == style)
                    .Select(p => new
                    {
                        id = p.Id,
                        name = p.Name,
                        price = p.Price,
                        imageBase64 = p.ImageBase64,
                        affiliateUrl = p.AffiliateUrl
                    })
                    .Take(20)
                    .ToListAsync(ct);

                recommendations.Add(new { title = style, items = products });
            }

            return new JsonResult(new { recommendations });
        }

        public class AnalyzeRequest
        {
            public IFormFile? Image { get; set; }
            public string SkinTone { get; set; } = "";
            public string Gender { get; set; } = "";
            public string BodyType { get; set; } = "";
        }


        // 🧩 Prompt thông minh hơn
        private static string BuildPrompt(string skinTone, string gender, string bodyType, IEnumerable<string> styles)
        {
            var desc = bodyType switch
            {
                "apple" => "quả táo (vai rộng, bụng tròn)",
                "pear" => "quả lê (hông to, vai nhỏ)",
                "triangle" => "tam giác ngược (vai to, hông nhỏ)",
                "rectangle" => "hình chữ nhật (cân đối)",
                "hourglass" => "đồng hồ cát (eo nhỏ, vai hông cân)",
                _ => bodyType
            };

            var list = string.Join(", ", styles.Select(s => $"\"{s}\""));

            return $@"
Bạn là stylist AI chuyên nghiệp.
Người dùng có:
- Giới tính: {gender}
- Tông da: {skinTone}
- Dáng người: {desc}

Danh sách phong cách có sẵn trong kho:
[{list}]

Hãy chọn ra 3 phong cách trong danh sách phù hợp nhất 
với người dùng và trả về JSON **chính xác tên phong cách trong danh sách**, 
không tạo tên mới.

Trả về đúng format:
{{ ""recommendations"": [
  {{ ""title"": ""Tên phong cách 1"" }},
  {{ ""title"": ""Tên phong cách 2"" }},
  {{ ""title"": ""Tên phong cách 3"" }}
] }}
";
        }

        private async Task<string> CallOpenAiAsync(string prompt, string? base64Image, CancellationToken ct)
        {
            // ✅ Lấy key trực tiếp từ DB (Id = 4)
            var apiKeyEntity = await _context.ApiKeys
                .Where(k => k.Id == 4)
                .FirstOrDefaultAsync(ct);

            if (apiKeyEntity == null || string.IsNullOrWhiteSpace(apiKeyEntity.ApiKey))
                throw new Exception("Không tìm thấy OpenAI API key trong cơ sở dữ liệu (Id = 4).");

            var apiKey = apiKeyEntity.ApiKey.Trim();
            var model = "gpt-4o-mini"; // có thể đổi sang model khác nếu cần

            var client = _http.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var body = new
            {
                model,
                messages = new[]
                {
            new
            {
                role = "user",
                content = new object[]
                {
                    new { type = "text", text = prompt }
                }
            }
        },
                temperature = 0.4,
                max_tokens = 300,
                response_format = new { type = "json_object" }
            };

            await Gate.WaitAsync(ct);
            try
            {
                var jsonPayload = JsonSerializer.Serialize(body);
                using var payload = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                using var resp = await client.PostAsync("https://api.openai.com/v1/chat/completions", payload, ct);

                var rawJson = await resp.Content.ReadAsStringAsync(ct);

                if (!resp.IsSuccessStatusCode)
                    return $"{{\"error\":\"OpenAI returned {(int)resp.StatusCode}: {rawJson}\"}}";

                using var doc = JsonDocument.Parse(rawJson);
                var root = doc.RootElement;

                if (root.TryGetProperty("choices", out var choices) &&
                    choices.GetArrayLength() > 0 &&
                    choices[0].TryGetProperty("message", out var message) &&
                    message.TryGetProperty("content", out var contentEl))
                {
                    return contentEl.GetString() ?? "";
                }

                return $"{{\"error\":\"Unexpected response format: {rawJson}\"}}";
            }
            catch (Exception ex)
            {
                return $"{{\"error\":\"{ex.Message}\"}}";
            }
            finally
            {
                Gate.Release();
            }
        }



        private static bool TryParseJson(string text, out string json)
        {
            var s = text.IndexOf('{');
            var e = text.LastIndexOf('}');
            if (s >= 0 && e > s)
            {
                json = text[s..(e + 1)];
                return true;
            }
            json = "";
            return false;
        }
    }
}
