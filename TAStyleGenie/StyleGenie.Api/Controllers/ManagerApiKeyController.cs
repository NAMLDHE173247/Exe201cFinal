using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StyleGenie.Infrastructure.Data.Models;

namespace StyleGenie.Api.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [ApiController]
    [Route("api/[controller]")]
    public class ManagerApiKeyController : ControllerBase
    {
        private readonly TryOnDbContext _db;

        public ManagerApiKeyController(TryOnDbContext db)
        {
            _db = db;
        }

        // ===== UPDATE API KEY =====
        [HttpPut("update")]
        public async Task<IActionResult> UpdateApiKey([FromBody] UpdateApiKeyRequest req)
        {
            // Kiểm tra rỗng
            if (string.IsNullOrWhiteSpace(req.ApiKey))
                return BadRequest(new { message = "ApiKey không được để trống." });

            // Luôn cập nhật bản ghi có Id = 1
            var apiKey = await _db.ApiKeys.FirstOrDefaultAsync(x => x.Id == 1);
            if (apiKey == null)
                return NotFound(new { message = "Không tìm thấy bản ghi API Key có Id = 1." });

            apiKey.ApiKey = req.ApiKey;
            apiKey.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "✅ Cập nhật API Key thành công.",
                data = new
                {
                    apiKey.Id,
                    apiKey.ApiKey,
                    apiKey.Credits,
                    apiKey.UpdatedAt
                }
            });
        }

        // ===== GET CURRENT API KEY =====
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentApiKey()
        {
            var apiKey = await _db.ApiKeys.FirstOrDefaultAsync(x => x.Id == 1);
            if (apiKey == null)
                return NotFound(new { message = "Không tìm thấy API Key có Id = 1." });

            return Ok(new
            {
                apiKey.Id,
                apiKey.ApiKey,
                apiKey.Credits,
                apiKey.UpdatedAt
            });
        }
    }

    // ===== Request DTO chỉ còn 1 field =====
    public class UpdateApiKeyRequest
    {
        public string ApiKey { get; set; } = default!;
    }
}
