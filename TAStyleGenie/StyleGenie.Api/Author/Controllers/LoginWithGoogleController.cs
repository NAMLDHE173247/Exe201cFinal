using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StyleGenie.Infrastructure.Data.Models;
using StyleGenie.Application.Wallet;
using System.ComponentModel.DataAnnotations;
using Google.Apis.Auth; // <— cần package Google.Apis.Auth

namespace StyleGenie.Api.Author.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly TryOnDbContext _db;
        private readonly IConfiguration _config;
        private readonly PasswordHasher<User> _passwordHasher = new();
        private readonly IWalletService _wallet;

        public LoginController(TryOnDbContext db, IConfiguration config, IWalletService wallet)
        {
            _db = db;
            _config = config;
            _wallet = wallet;
        }

        // ==== DTOs ====
        public class LoginNormalRequest
        {
            [Required, EmailAddress] public string Email { get; set; } = default!;
            [Required] public string Password { get; set; } = default!;
        }

        public class LoginGoogleRequest
        {
            [Required] public string IdToken { get; set; } = default!;
        }

        public class UserResponse
        {
            public long Id { get; set; }              // int -> long
            public string Email { get; set; } = default!;
            public string? FullName { get; set; }
            public bool IsVerified { get; set; }
            public bool IsActive { get; set; }
            public long TenantId { get; set; }        // int -> long
            public long? RoleId { get; set; }         // int? -> long?
            public string? Role { get; set; }
            public DateTime CreatedAt { get; set; }
        }


        public class LoginResponse
        {
            public string Token { get; set; } = default!;
            public UserResponse User { get; set; } = default!;
        }

        // ==== Helpers ====
        private static UserResponse MapUser(User u) => new()
        {
            Id = u.Id,
            Email = u.Email,
            FullName = u.FullName,
            IsVerified = u.IsVerified,
            IsActive = u.IsActive,
            TenantId = u.TenantId,
            RoleId = u.RoleId,
            Role = u.Role?.Name,
            CreatedAt = u.CreatedAt
        };

        private LoginResponse ToLoginResponse(User user)
        {
            var token = JwtTokenHelper.GenerateToken(
                user,
                _config["Jwt:Key"]!,
                _config["Jwt:Issuer"]!
            );
            return new LoginResponse { Token = token, User = MapUser(user) };
        }

        private static IActionResult ForbiddenIfInactiveOrUnverified(User user)
        {
            if (!user.IsActive)
                return new ObjectResult("Tài khoản đã bị vô hiệu hoá.") { StatusCode = StatusCodes.Status403Forbidden };
            //if (!user.IsVerified)
            //    return new ObjectResult("Tài khoản chưa xác minh email.") { StatusCode = StatusCodes.Status403Forbidden };
            return null!;
        }

        // ==== Email + Password ====
        [HttpPost("normal")]
        public async Task<IActionResult> LoginNormal([FromBody] LoginNormalRequest req, CancellationToken ct)
        {
            var user = await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == req.Email, ct);

            if (user is null)
                return Unauthorized("Email hoặc mật khẩu không đúng.");

            var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, req.Password);
            if (verify != PasswordVerificationResult.Success)
                return Unauthorized("Email hoặc mật khẩu không đúng.");

            var forbid = ForbiddenIfInactiveOrUnverified(user);
            if (forbid is not null) return forbid;

            // Trả về cùng cấu trúc với Google login (đủ Id, TenantId, RoleId, ...)
            return Ok(ToLoginResponse(user));
        }
        // ==== Google Login (chỉ xử lý đăng nhập) ====
        [HttpPost("google")]
        public async Task<IActionResult> LoginGoogle([FromBody] LoginGoogleRequest req, CancellationToken ct)
        {
            // Xác minh Google ID Token
            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(req.IdToken, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _config["Google:ClientId"] } // phải khớp ClientId
                });
            }
            catch
            {
                return Unauthorized("Google token không hợp lệ.");
            }

            var email = payload.Email;
            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized("Không lấy được email từ Google.");

            var user = await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email, ct);

            if (user is null)
            {
                // (Tuỳ chính sách) Có thể: TỪ CHỐI nếu chưa đăng ký trước
                // return NotFound("Tài khoản chưa đăng ký.");

                // Hoặc: tự tạo user Google-first (PasswordHash = null)
                // Tạo Tenant trước
                var tenant = new Tenant
                {
                    Name = $"{payload.GivenName} {payload.FamilyName}".Trim() ?? email,
                    Status = 1, // Active
                    CreatedAt = DateTime.UtcNow
                };
                _db.Tenants.Add(tenant);
                await _db.SaveChangesAsync(ct);

                user = new User
                {
                    Email = email,
                    FullName = $"{payload.GivenName} {payload.FamilyName}".Trim(),
                    PasswordHash = null,              // quan trọng: đánh dấu tài khoản Google
                    IsVerified = true,                // Google email đã xác minh
                    IsActive = true,
                    TenantId = tenant.Id,
                    RoleId = 3,                       // User
                    CreatedAt = DateTime.UtcNow
                };
                _db.Users.Add(user);
                await _db.SaveChangesAsync(ct);

                // Tạo wallet với 5 credits miễn phí cho user mới
                await _wallet.CreditAsync(user.TenantId, 5, "WELCOME_BONUS", "Chào mừng bạn đến với StyleGenie!");

                user = await _db.Users.Include(u => u.Role).FirstAsync(u => u.Id == user.Id, ct);
            }
            else
            {
                // Nếu là account password-only thì block Google login
                if (!string.IsNullOrEmpty(user.PasswordHash))
                    return Unauthorized("Tài khoản này đăng nhập bằng email + mật khẩu.");
            }

            var forbid = ForbiddenIfInactiveOrUnverified(user);
            if (forbid is not null) return forbid;

            return Ok(ToLoginResponse(user));
        }

        // ==== Check login method ====
        [HttpGet("check-method")]
        public async Task<IActionResult> CheckLoginMethod([FromQuery] string email, CancellationToken ct)
        {
            var user = await _db.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email, ct);

            if (user is null)
                return NotFound("Tài khoản chưa tồn tại");

            var method = string.IsNullOrEmpty(user.PasswordHash) ? "google" : "normal";
            return Ok(new { method });
        }
    }
}
