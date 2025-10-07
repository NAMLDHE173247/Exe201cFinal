using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StyleGenie.Infrastructure.Data.Models;
using StyleGenie.Infrastructure.Persistence;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StyleGenie.Api.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [ApiController]
    [Route("api/adminmanageraccount")]
    public class ManagerAccountController : ControllerBase
    {
        private readonly TryOnDbContext _db;

        public ManagerAccountController(TryOnDbContext db)
        {
            _db = db;
        }

        // ✅ GET: Danh sách người dùng + Trạng thái hệ thống
        [HttpGet]
        [SwaggerOperation(Summary = "Danh sách người dùng (có search, filter, pagination, kèm thống kê trạng thái)")]
        public async Task<IActionResult> GetAllUsers(
            [FromQuery] string? search,
            [FromQuery] string? role,
            [FromQuery] bool? isActive,
            [FromQuery] bool? isVerified,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            // -------------------------------
            // B1️⃣: Truy vấn danh sách user cơ bản
            // -------------------------------
            var query = _db.Users
                .Include(u => u.Tenant)
                .Include(u => u.Role)
                .AsNoTracking()
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.FullName,
                    TenantId = u.TenantId,
                    TenantName = u.Tenant != null ? u.Tenant.Name : "(Chưa có Tenant)",
                    RoleName = u.Role != null ? u.Role.Name : "(Chưa có Role)",
                    u.IsActive,
                    u.IsVerified,
                    Balance = _db.CreditWallets
                        .Where(w => w.TenantId == u.TenantId)
                        .Select(w => (decimal?)w.Balance)
                        .FirstOrDefault() ?? 0,
                    u.CreatedAt
                })
                .AsQueryable();

            // -------------------------------
            // B2️⃣: Lọc dữ liệu theo query string
            // -------------------------------
            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.Trim().ToLower();
                query = query.Where(u =>
                    u.Email.ToLower().Contains(keyword) ||
                    (u.FullName != null && u.FullName.ToLower().Contains(keyword)));
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                var roleLower = role.Trim().ToLower();
                query = query.Where(u => u.RoleName.ToLower() == roleLower);
            }

            if (isActive.HasValue)
                query = query.Where(u => u.IsActive == isActive.Value);

            if (isVerified.HasValue)
                query = query.Where(u => u.IsVerified == isVerified.Value);

            // -------------------------------
            // B3️⃣: Tổng số bản ghi (phân trang)
            // -------------------------------
            var totalCount = await query.CountAsync(ct);

            // -------------------------------
            // B4️⃣: Lấy dữ liệu trang hiện tại
            // -------------------------------
            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            // -------------------------------
            // B5️⃣: Tính toán trạng thái hệ thống (status summary)
            // -------------------------------
            var totalUsers = await _db.Users.CountAsync(ct);
            var activeCount = await _db.Users.CountAsync(u => u.IsActive, ct);
            var inactiveCount = totalUsers - activeCount;
            var verifiedCount = await _db.Users.CountAsync(u => u.IsVerified, ct);
            var unverifiedCount = totalUsers - verifiedCount;

            var byRole = await _db.Users
                .Include(u => u.Role)
                .GroupBy(u => u.Role.Name)
                .Select(g => new
                {
                    Role = g.Key,
                    Count = g.Count()
                })
                .ToListAsync(ct);

            // -------------------------------
            // B6️⃣: Trả về JSON tổng hợp
            // -------------------------------
            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                statusSummary = new
                {
                    totalUsers,
                    activeCount,
                    inactiveCount,
                    verifiedCount,
                    unverifiedCount,
                    byRole
                },
                data = users
            });
        }



        // ✅ PUT: Cập nhật vai trò (Role)
        [HttpPut("{id:long}/role")]
        [SwaggerOperation(Summary = "Cập nhật vai trò (Role) của người dùng")]
        public async Task<IActionResult> UpdateUserRole(long id, [FromBody] string newRoleName, CancellationToken ct)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
            if (user == null)
                return NotFound(new { error = "User not found" });

            var role = await _db.Roles.FirstOrDefaultAsync(r => r.Name.ToLower() == newRoleName.ToLower(), ct);
            if (role == null)
                return BadRequest(new { error = $"Role '{newRoleName}' không tồn tại" });

            user.RoleId = role.Id;
            _db.Users.Update(user);
            await _db.SaveChangesAsync(ct);

            return Ok(new { message = $"Đã cập nhật role của {user.Email} thành '{role.Name}'" });
        }



        // ✅ DTO dùng chung cho /active và /verified
        public sealed class ToggleDto
        {
            [Required]
            [JsonPropertyName("active")]
            public bool? Active { get; set; }
        }

        // ... giữ nguyên các action khác ...
        [HttpPut("{id:long}/active")]
        // 💡 SỬA: Nhận trực tiếp CHUỖI từ body
        public async Task<IActionResult> UpdateActiveStatus(long id, [FromBody] string value, CancellationToken ct)
        {
            // Kiểm tra tính hợp lệ của chuỗi
            if (string.IsNullOrEmpty(value) ||
                (value.ToLowerInvariant() != "true" && value.ToLowerInvariant() != "false"))
            {
                return BadRequest("Body must be the string 'true' or 'false'.");
            }

            // Chuyển đổi chuỗi thành boolean
            // Lưu ý: JSON.NET sẽ tự động loại bỏ dấu ngoặc kép khi deserializing chuỗi trần.
            // Ví dụ: body là "true" (có dấu ngoặc kép) sẽ được gán là chuỗi "true" (không dấu ngoặc kép)
            bool newStatus = value.ToLowerInvariant() == "true";

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
            if (user is null) return NotFound(new { error = "User not found" });

            // Sử dụng giá trị boolean đã chuyển đổi
            user.IsActive = newStatus;
            _db.Users.Update(user);
            await _db.SaveChangesAsync(ct);

            return Ok(new { message = $"Tài khoản {(user.IsActive ? "đã kích hoạt" : "đã bị khóa")} thành công." });
        }

        [HttpPut("{id:long}/verified")]
        // 💡 SỬA: Nhận trực tiếp CHUỖI từ body
        public async Task<IActionResult> UpdateVerifiedStatus(long id, [FromBody] string value, CancellationToken ct)
        {
            // Kiểm tra tính hợp lệ của chuỗi
            if (string.IsNullOrEmpty(value) ||
                (value.ToLowerInvariant() != "true" && value.ToLowerInvariant() != "false"))
            {
                return BadRequest("Body must be the string 'true' or 'false'.");
            }

            // Chuyển đổi chuỗi thành boolean
            bool newStatus = value.ToLowerInvariant() == "true";

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
            if (user is null) return NotFound(new { error = "User not found" });

            // Sử dụng giá trị boolean đã chuyển đổi
            user.IsVerified = newStatus;
            _db.Users.Update(user);
            await _db.SaveChangesAsync(ct);

            return Ok(new { message = $"Trạng thái xác minh email đã được {(user.IsVerified ? "bật" : "tắt")}." });
        }

    }
}
