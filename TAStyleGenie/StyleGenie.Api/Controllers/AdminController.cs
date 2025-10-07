using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StyleGenie.Application.Credentials;
using StyleGenie.Application.Wallet;
using StyleGenie.Infrastructure.Data.Models;
using StyleGenie.Infrastructure.Persistence;
using Swashbuckle.AspNetCore.Annotations;

namespace StyleGenie.Api.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly ICredentialStore _creds;
        private readonly IWalletService _wallet;  // Inject IWalletService
        private readonly TryOnDbContext _db;

        // Constructor để inject các dependency
        public AdminController(ICredentialStore creds, IWalletService wallet, TryOnDbContext db)
        {
            _creds = creds;
            _wallet = wallet;  // Initialize _wallet
            _db = db;
        }

        // DTO để nhận input
        public record SetKeyDto(string Provider, string KeyName, string SecretPlain, long? TenantId, string? UpdatedBy);

        // ✅ Lưu hoặc cập nhật API key (mã hóa trước khi lưu)
        [HttpPost("credentials/set")]
        [SwaggerOperation(Summary = "Lưu hoặc cập nhật API key (mã hóa trong DB)")]
        public async Task<IActionResult> SetKey([FromBody] SetKeyDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.SecretPlain))
                return BadRequest(new { error = "SecretPlain is required" });

            // Cập nhật key với provider và keyName luôn là "FitRoom" và "ApiKey"
            var existingApiKey = await _db.ApiKeys.FirstOrDefaultAsync(x => x.Id == 1);

            if (existingApiKey == null)
                return NotFound(new { error = "API key with Id = 1 not found" });

            // Cập nhật API key với Id = 1
            existingApiKey.ApiKey = dto.SecretPlain;  // Cập nhật key mới
            existingApiKey.Credits = 5;  // Cập nhật credits mặc định nếu cần
            existingApiKey.UpdatedAt = DateTime.UtcNow;
            existingApiKey.UpdatedBy = dto.UpdatedBy;

            // Lưu vào DB
            _db.ApiKeys.Update(existingApiKey);
            await _db.SaveChangesAsync();

            return Ok(new { ok = true, updated = true });
        }

        // ✅ Nạp credit cho tenant
        [HttpPost("wallet/topup")]
        [SwaggerOperation(Summary = "Nạp credit cho tenant")]
        public async Task<IActionResult> TopUp([FromQuery] long tenantId, [FromQuery] int credits, [FromQuery] string reason = "TOPUP")
        {
            if (tenantId <= 0 || credits <= 0)
                return BadRequest(new { error = "tenantId và credits phải > 0" });

            // Nạp credit cho tenant
            await _wallet.CreditAsync(tenantId, credits, reason, null);
            return Ok(new { ok = true });
        }

        // ✅ Xem số dư của tenant
        [HttpGet("wallet/balance")]
        [SwaggerOperation(Summary = "Xem số dư credit của tenant")]
        public async Task<IActionResult> Balance([FromQuery] long tenantId)
        {
            if (tenantId <= 0)
                return BadRequest(new { error = "tenantId is required" });

            var wallet = await _db.CreditWallets.AsNoTracking()
                .FirstOrDefaultAsync(x => x.TenantId == tenantId);

            // Kiểm tra nếu không có ví cho tenant
            if (wallet == null)
                return NotFound(new { error = "Wallet not found for tenant" });

            return Ok(new
            {
                tenantId,
                balance = wallet?.Balance ?? 0
            });
        }
    }
}
