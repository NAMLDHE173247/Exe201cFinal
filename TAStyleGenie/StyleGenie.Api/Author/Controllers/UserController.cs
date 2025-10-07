using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StyleGenie.Infrastructure.Data.Models;
using StyleGenie.Application.Wallet;
using StyleGenie.Infrastructure.Persistence;
using Swashbuckle.AspNetCore.Annotations;

namespace StyleGenie.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly TryOnDbContext _db;
        private readonly IWalletService _wallet;

        public UserController(TryOnDbContext db, IWalletService wallet)
        {
            _db = db;
            _wallet = wallet;
        }

        // ✅ Lấy danh sách người dùng với thông tin tenant và balance
        [HttpGet]
        [SwaggerOperation(Summary = "Lấy danh sách người dùng với tenantId và balance")]
        public async Task<IActionResult> GetAllUsers(CancellationToken ct)
        {
            var users = await _db.Users
                .Include(u => u.Tenant)
                .AsNoTracking()
                .ToListAsync(ct);

            var userInfos = new List<object>();

            foreach (var user in users)
            {
                var wallet = await _db.CreditWallets.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.TenantId == user.TenantId, ct);

                userInfos.Add(new
                {
                    user.Id,
                    user.Email,
                    user.FullName,
                    user.IsVerified,
                    user.IsActive,
                    user.TenantId,
                    balance = wallet?.Balance ?? 0
                });
            }

            return Ok(userInfos);
        }

        // ✅ Lấy thông tin người dùng theo id
        [HttpGet("{id:long}")]
        [SwaggerOperation(Summary = "Lấy thông tin người dùng với tenantId và balance")]
        public async Task<IActionResult> GetUserById(long id, CancellationToken ct)
        {
            var user = await _db.Users
                .AsNoTracking()
                .Include(u => u.Tenant)
                .FirstOrDefaultAsync(u => u.Id == id, ct);

            if (user == null)
                return NotFound(new { error = "User not found" });

            var wallet = await _db.CreditWallets.AsNoTracking()
                .FirstOrDefaultAsync(x => x.TenantId == user.TenantId, ct);

            return Ok(new
            {
                user.Id,
                user.Email,
                user.FullName,
                user.IsVerified,
                user.IsActive,
                user.TenantId,
                balance = wallet?.Balance ?? 0
            });
        }

        // ✅ Cập nhật thông tin người dùng
        [HttpPut("{id:long}")]
        [SwaggerOperation(Summary = "Cập nhật thông tin người dùng")]
        public async Task<IActionResult> UpdateUser(long id, [FromBody] User updatedUser, CancellationToken ct)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

            if (user == null)
                return NotFound(new { error = "User not found" });

            // Cập nhật thông tin người dùng
            user.Email = updatedUser.Email ?? user.Email;
            user.FullName = updatedUser.FullName ?? user.FullName;
            user.IsVerified = updatedUser.IsVerified;
            user.IsActive = updatedUser.IsActive;

            _db.Users.Update(user);
            await _db.SaveChangesAsync(ct);

            return Ok(user);
        }

        // ✅ Cập nhật số dư balance của người dùng
        [HttpPut("{id:long}/balance")]
        [SwaggerOperation(Summary = "Cập nhật số dư balance của người dùng")]
        public async Task<IActionResult> UpdateBalance(long id, [FromBody] decimal newBalance, CancellationToken ct)
        {
            // Tìm người dùng theo id
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

            if (user == null)
                return NotFound(new { error = "User not found" });

            // Tìm ví (wallet) của người dùng từ bảng CreditWallets
            var wallet = await _db.CreditWallets.FirstOrDefaultAsync(x => x.TenantId == user.TenantId, ct);

            if (wallet == null)
                return NotFound(new { error = "Wallet not found for this user" });

            // Chỉ cập nhật balance, không thay đổi các thuộc tính khác
            wallet.Balance = (int)newBalance;  // Cập nhật balance

            _db.CreditWallets.Update(wallet);  // Cập nhật wallet trong DB
            await _db.SaveChangesAsync(ct);  // Lưu thay đổi vào DB

            // Trả về kết quả cập nhật balance
            return Ok(new { tenantId = wallet.TenantId, balance = wallet.Balance });
        }


        // ✅ Xóa người dùng
        [HttpDelete("{id:long}")]
        [SwaggerOperation(Summary = "Xóa người dùng")]
        public async Task<IActionResult> DeleteUser(long id, CancellationToken ct)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

            if (user == null)
                return NotFound(new { error = "User not found" });

            _db.Users.Remove(user);
            await _db.SaveChangesAsync(ct);

            return NoContent();
        }
    }
}
