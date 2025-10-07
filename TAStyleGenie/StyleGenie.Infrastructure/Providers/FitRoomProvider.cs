using StyleGenie.Application.Credentials;
using StyleGenie.Application.TryOn;
using System.Text.Json;

public sealed class FitRoomProvider : ITryOnProvider
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly ICredentialStore _creds;

    public FitRoomProvider(IHttpClientFactory httpFactory, ICredentialStore creds)
    {
        _httpFactory = httpFactory;
        _creds = creds;
    }

    public async Task<(string TaskId, Func<CancellationToken, Task<byte[]>> Downloader)>
        CreateAndPollAsync(long tenantId,
                           string clothType, bool hdMode,
                           byte[] model, byte[] cloth, byte[]? lower,
                           CancellationToken ct)
    {
        // Lấy API key từ DB (Id = 1)
        var apiKey = await _creds.GetApiKeyAsync();

        if (string.IsNullOrEmpty(apiKey))
            throw new InvalidOperationException("FitRoom API key not configured");

        var baseUrl = "https://platform.fitroom.app";
        var http = _httpFactory.CreateClient();
        http.Timeout = TimeSpan.FromMinutes(3);
        http.DefaultRequestHeaders.Add("X-API-KEY", apiKey);

        using var form = new MultipartFormDataContent();
        form.Add(new StringContent(clothType), "cloth_type");
        form.Add(new StringContent(hdMode ? "true" : "false"), "hd_mode");
        form.Add(new ByteArrayContent(model), "model_image", "model.jpg");
        if (clothType == "combo" && lower != null)
        {
            form.Add(new ByteArrayContent(cloth), "cloth_image", "upper.jpg");
            form.Add(new ByteArrayContent(lower), "lower_cloth_image", "lower.jpg");
        }
        else
        {
            form.Add(new ByteArrayContent(cloth), "cloth_image", "cloth.jpg");
        }

        var resp = await http.PostAsync($"{baseUrl}/api/tryon/v2/tasks", form, ct);
        var body = await resp.Content.ReadAsStringAsync(ct);

        if (!resp.IsSuccessStatusCode)
        {
            if (resp.StatusCode == System.Net.HttpStatusCode.PaymentRequired)
            {
                throw new UnauthorizedAccessException("Tài khoản FitRoom đã hết lượt sử dụng (Insufficient credits)");
            }

            if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("Tài khoản hết hạn hoặc API key không hợp lệ");
            }

            throw new InvalidOperationException($"FitRoom create failed {resp.StatusCode}: {body}");
        }

        using var doc = JsonDocument.Parse(body);
        var taskId = doc.RootElement.GetProperty("task_id").GetString()!;

        async Task<byte[]> Downloader(CancellationToken token)
        {
            var until = DateTime.UtcNow.AddSeconds(90);
            var delay = 1200;
            while (DateTime.UtcNow < until)
            {
                var s = await http.GetStringAsync($"{baseUrl}/api/tryon/v2/tasks/{taskId}", token);
                using var sd = JsonDocument.Parse(s);
                var status = sd.RootElement.GetProperty("status").GetString()!;

                if (status == "COMPLETED")
                {
                    var url = sd.RootElement.GetProperty("download_signed_url").GetString()!;
                    return await http.GetByteArrayAsync(url, token);
                }

                if (status == "FAILED")
                {
                    var err = sd.RootElement.TryGetProperty("error", out var e) ? e.GetString() : "unknown";

                    if (!string.IsNullOrEmpty(err) && err.Contains("key", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new UnauthorizedAccessException("Tài khoản hết hạn hoặc API key không hợp lệ");
                    }

                    if (!string.IsNullOrEmpty(err) && err.Contains("credit", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new UnauthorizedAccessException("Tài khoản FitRoom đã hết lượt sử dụng (Insufficient credits)");
                    }

                    throw new InvalidOperationException($"FitRoom failed: {err}");
                }

                await Task.Delay(delay, token);
                delay = Math.Min(3000, delay + 400);
            }

            throw new TimeoutException("FitRoom polling timeout");
        }

        return (taskId, Downloader);
    }
}
