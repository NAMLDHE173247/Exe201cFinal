using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StyleGenie.Infrastructure.Data.Models;
using StyleGenie.Infrastructure.Persistence;
using Swashbuckle.AspNetCore.Annotations;

namespace StyleGenie.Api.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [ApiController]
    [Route("api/admin/apikeys")]
    public class ApiKeysController : ControllerBase
    {
        private readonly TryOnDbContext _db;

        public ApiKeysController(TryOnDbContext db)
        {
            _db = db;
        }

        // DTOs
        public record ApiKeyCreateDto(string ApiKey, int Credits, string? UpdatedBy);
        public record ApiKeyUpdateDto(string? ApiKey, int? Credits, string? UpdatedBy);
        public record UpdateApiKeyDto(string SecretPlain, string UpdatedBy); // Giữ nguyên hàm này

        /* ============================================================
           ✅ 1. GET ALL - Lấy danh sách toàn bộ API Keys
        ============================================================ */
        [HttpGet]
        [SwaggerOperation(Summary = "Lấy danh sách tất cả API Keys")]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var list = await _db.ApiKeys.OrderBy(a => a.Id).ToListAsync(ct);
            return Ok(list);
        }

        /* ============================================================
           ✅ 2. GET BY ID - Lấy chi tiết 1 API Key
        ============================================================ */
        [HttpGet("{id:long}")]
        [SwaggerOperation(Summary = "Lấy chi tiết API Key theo Id")]
        public async Task<IActionResult> GetById(long id, CancellationToken ct)
        {
            var apiKey = await _db.ApiKeys.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (apiKey == null)
                return NotFound(new { error = $"API key with Id = {id} not found" });

            return Ok(apiKey);
        }

        /* ============================================================
           ✅ 3. CREATE - Tạo mới API Key
        ============================================================ */
        [HttpPost]
        [SwaggerOperation(Summary = "Tạo mới API Key")]
        public async Task<IActionResult> Create([FromBody] ApiKeyCreateDto dto, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(dto.ApiKey))
                return BadRequest(new { error = "ApiKey cannot be empty" });

            var entity = new ApiKeys
            {
                ApiKey = dto.ApiKey,
                Credits = dto.Credits,
                UpdatedBy = dto.UpdatedBy,
                UpdatedAt = DateTime.UtcNow
            };

            _db.ApiKeys.Add(entity);
            await _db.SaveChangesAsync(ct);

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        /* ============================================================
           ✅ 4. UPDATE - Cập nhật thông tin API Key theo Id (chung)
        ============================================================ */
        [HttpPut("{id:long}")]
        [SwaggerOperation(Summary = "Cập nhật API Key theo Id")]
        public async Task<IActionResult> Update(long id, [FromBody] ApiKeyUpdateDto dto, CancellationToken ct)
        {
            var apiKey = await _db.ApiKeys.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (apiKey == null)
                return NotFound(new { error = $"API key with Id = {id} not found" });

            if (!string.IsNullOrWhiteSpace(dto.ApiKey))
                apiKey.ApiKey = dto.ApiKey;

            if (dto.Credits.HasValue)
                apiKey.Credits = dto.Credits.Value;

            if (!string.IsNullOrWhiteSpace(dto.UpdatedBy))
                apiKey.UpdatedBy = dto.UpdatedBy;

            apiKey.UpdatedAt = DateTime.UtcNow;

            _db.ApiKeys.Update(apiKey);
            await _db.SaveChangesAsync(ct);

            return Ok(new { ok = true, updated = true });
        }

        /* ============================================================
           ✅ 5. DELETE - Xóa API Key theo Id
        ============================================================ */
        [HttpDelete("{id:long}")]
        [SwaggerOperation(Summary = "Xóa API Key theo Id")]
        public async Task<IActionResult> Delete(long id, CancellationToken ct)
        {
            var apiKey = await _db.ApiKeys.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (apiKey == null)
                return NotFound(new { error = $"API key with Id = {id} not found" });

            _db.ApiKeys.Remove(apiKey);
            await _db.SaveChangesAsync(ct);

            return Ok(new { ok = true, deleted = true });
        }

        /* ============================================================
           ✅ 6. UPDATE FIXED KEY ID=1 (Giữ nguyên yêu cầu cũ)
        ============================================================ */
        [HttpPut("update")]
        [SwaggerOperation(Summary = "Cập nhật API key với Id = 1 (key cố định)")]
        public async Task<IActionResult> UpdateApiKey([FromBody] UpdateApiKeyDto dto, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(dto.SecretPlain))
                return BadRequest(new { error = "SecretPlain is required" });

            var apiKeyRecord = await _db.ApiKeys.FirstOrDefaultAsync(x => x.Id == 1, ct);
            if (apiKeyRecord == null)
                return NotFound(new { error = "API key with Id = 1 not found" });

            apiKeyRecord.ApiKey = dto.SecretPlain;
            apiKeyRecord.UpdatedAt = DateTime.UtcNow;
            apiKeyRecord.UpdatedBy = dto.UpdatedBy;

            _db.ApiKeys.Update(apiKeyRecord);
            await _db.SaveChangesAsync(ct);

            return Ok(new { ok = true, updated = true });
        }
    }
}
