using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StyleGenie.Infrastructure.Data.Models;
using StyleGenie.Application.Wallet;
using StyleGenie.Infrastructure.Persistence;
using Swashbuckle.AspNetCore.Annotations;

namespace StyleGenie.Api.Controllers
{
    [ApiController]
    [Route("api/admin/tenants")]
    public class TenantController : ControllerBase
    {
        private readonly TryOnDbContext _db;
        private readonly IWalletService _wallet;

        public TenantController(TryOnDbContext db, IWalletService wallet)
        {
            _db = db;
            _wallet = wallet;
        }

        // ✅ Lấy danh sách tenant
        [HttpGet]
        [SwaggerOperation(Summary = "Lấy danh sách tenants")]
        public async Task<IActionResult> GetAllTenants(CancellationToken ct)
        {
            var tenants = await _db.Tenants.AsNoTracking().ToListAsync(ct);
            return Ok(tenants);
        }

        // ✅ Lấy thông tin tenant theo id
        [HttpGet("{tenantId:long}")]
        [SwaggerOperation(Summary = "Lấy thông tin tenant")]
        public async Task<IActionResult> GetTenantById(long tenantId, CancellationToken ct)
        {
            var tenant = await _db.Tenants.AsNoTracking().FirstOrDefaultAsync(x => x.Id == tenantId, ct);
            if (tenant == null)
                return NotFound(new { error = "Tenant not found" });

            var wallet = await _db.CreditWallets.AsNoTracking().FirstOrDefaultAsync(x => x.TenantId == tenantId, ct);

            return Ok(new
            {
                tenant.Id,
                tenant.Name,
                tenant.Status,
                tenant.CreatedAt,
                balance = wallet?.Balance ?? 0
            });
        }

        // ✅ Tạo mới tenant
        [HttpPost]
        [SwaggerOperation(Summary = "Tạo mới tenant")]
        public async Task<IActionResult> CreateTenant([FromBody] Tenant tenant, CancellationToken ct)
        {
            if (tenant == null)
                return BadRequest(new { error = "Invalid tenant data" });

            // Thêm tenant vào DB
            _db.Tenants.Add(tenant);
            await _db.SaveChangesAsync(ct);

            // Tạo ví cho tenant mới (với số dư mặc định là 0)
            var wallet = new CreditWallet
            {
                TenantId = tenant.Id,
                Balance = 3,  // Số dư mặc định là 0
              //  CreatedAt = DateTime.UtcNow
            };
            _db.CreditWallets.Add(wallet);
            await _db.SaveChangesAsync(ct);

            return CreatedAtAction(nameof(GetTenantById), new { tenantId = tenant.Id }, tenant);
        }

        // ✅ Cập nhật thông tin tenant
        [HttpPut("{tenantId:long}")]
        [SwaggerOperation(Summary = "Cập nhật thông tin tenant")]
        public async Task<IActionResult> UpdateTenant(long tenantId, [FromBody] Tenant updatedTenant, CancellationToken ct)
        {
            var tenant = await _db.Tenants.FirstOrDefaultAsync(x => x.Id == tenantId, ct);
            if (tenant == null)
                return NotFound(new { error = "Tenant not found" });

            tenant.Name = updatedTenant.Name ?? tenant.Name;
            tenant.Status = updatedTenant.Status;

            _db.Tenants.Update(tenant);
            await _db.SaveChangesAsync(ct);

            return Ok(tenant);
        }

        // ✅ Cập nhật số dư balance của tenant
        [HttpPut("{tenantId:long}/balance")]
        [SwaggerOperation(Summary = "Cập nhật số dư balance của tenant")]
        public async Task<IActionResult> UpdateBalance(long tenantId, [FromBody] decimal newBalance, CancellationToken ct)
        {
            var wallet = await _db.CreditWallets.FirstOrDefaultAsync(x => x.TenantId == tenantId, ct);

            if (wallet == null)
                return NotFound(new { error = "Wallet for tenant not found" });

            wallet.Balance = (int)newBalance;  // Cập nhật số dư balance

            _db.CreditWallets.Update(wallet);
            await _db.SaveChangesAsync(ct);

            return Ok(new { tenantId = wallet.TenantId, balance = wallet.Balance });
        }

        // ✅ Xóa tenant
        [HttpDelete("{tenantId:long}")]
        [SwaggerOperation(Summary = "Xóa tenant")]
        public async Task<IActionResult> DeleteTenant(long tenantId, CancellationToken ct)
        {
            var tenant = await _db.Tenants.FirstOrDefaultAsync(x => x.Id == tenantId, ct);
            if (tenant == null)
                return NotFound(new { error = "Tenant not found" });

            _db.Tenants.Remove(tenant);
            await _db.SaveChangesAsync(ct);

            return NoContent();
        }
    }
}
