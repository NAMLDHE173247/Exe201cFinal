using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StyleGenie.Infrastructure.Data.Models;
using StyleGenie.Application.Wallet;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StyleGenie.Api.Author.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterNewAccountController : ControllerBase
    {
        private readonly TryOnDbContext _db;
        private readonly IConfiguration _config;
        private readonly PasswordHasher<User> _passwordHasher = new();
        private readonly IWalletService _wallet;

        public RegisterNewAccountController(TryOnDbContext db, IConfiguration config, IWalletService wallet)
        {
            _db = db;
            _config = config;
            _wallet = wallet;
        }

        // DTO input đăng ký
        public sealed class RegisterUserRequest
        {
            [Required, EmailAddress, StringLength(256)]
            public string Email { get; set; } = default!;

            [Required, StringLength(100, MinimumLength = 6)]
            public string Password { get; set; } = default!;

            [StringLength(128)]
            public string? FullName { get; set; }
        }

        // DTO output an toàn (không lộ hash)
        public sealed class UserResponse
        {
            public long Id { get; set; }
            public string Email { get; set; } = default!;
            public string? FullName { get; set; }
            public bool IsVerified { get; set; }
            public bool IsActive { get; set; }
            public long TenantId { get; set; }
            public string? Role { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        [HttpPost]
        public async Task<ActionResult<UserResponse>> Register([FromBody] RegisterUserRequest req, CancellationToken ct)
        {
            if (await _db.Users.AnyAsync(u => u.Email == req.Email, ct))
                return Conflict("Email already exists.");

            var role = await _db.Roles.FindAsync(new object?[] { 3L }, ct);
            if (role is null) return BadRequest("Default role (Id=3) not found. Seed roles first.");

            var user = new User
            {
                Email = req.Email.Trim(),
                FullName = string.IsNullOrWhiteSpace(req.FullName) ? null : req.FullName.Trim(),
                RoleId = role.Id,
                IsVerified = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, req.Password);

            // B1: Tạo Tenant trước
            var tenant = new Tenant
            {
                Name = user.FullName ?? user.Email,
                Status = 1, // Active
                CreatedAt = DateTime.UtcNow
            };
            _db.Tenants.Add(tenant);
            await _db.SaveChangesAsync(ct);

            // B2: Gán TenantId cho user
            user.TenantId = tenant.Id;
            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);

            // B3: Tạo wallet với 5 credits miễn phí cho user mới
            await _wallet.CreditAsync(user.TenantId, 5, "WELCOME_BONUS", "Chào mừng bạn đến với StyleGenie!");

            var resp = new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                IsVerified = user.IsVerified,
                IsActive = user.IsActive,
                TenantId = user.TenantId,
                Role = role.Name,
                CreatedAt = user.CreatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, resp);
        }

        // GET api/RegisterNewAccount/{id}
        [HttpGet("{id:long}")]
        public async Task<ActionResult<UserResponse>> GetById(long id, CancellationToken ct)
        {
            var user = await _db.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id, ct);

            if (user is null) return NotFound();

            return Ok(new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                IsVerified = user.IsVerified,
                IsActive = user.IsActive,
                TenantId = user.TenantId,
                Role = user.Role?.Name,
                CreatedAt = user.CreatedAt
            });
        }

        // POST api/RegisterNewAccount/verify
        public sealed class VerifyPasswordRequest
        {
            [Required, EmailAddress] public string Email { get; set; } = default!;
            [Required] public string Password { get; set; } = default!;
        }

        [HttpPost("verify")]
        public async Task<ActionResult> Verify([FromBody] VerifyPasswordRequest req, CancellationToken ct)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.Email, ct);
            if (user is null || string.IsNullOrEmpty(user.PasswordHash))
                return Unauthorized("Invalid email or password.");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, req.Password);
            return result == PasswordVerificationResult.Success
                ? Ok(new { ok = true })
                : Unauthorized("Invalid email or password.");
        }

        // POST api/RegisterNewAccount/google
        [HttpPost("google")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] GoogleLoginRequest req, CancellationToken ct)
        {
            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(req.Token, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { "418794469453-qevr8bhas6q4eokp13akoa87fb0utb77.apps.googleusercontent.com" }
                });
            }
            catch
            {
                return Unauthorized("Google token không hợp lệ");
            }

            var email = payload.Email;
            var name = payload.Name;

            var user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email, ct);

            if (user == null)
            {
                // Tạo Tenant trước
                var tenant = new Tenant
                {
                    Name = name ?? email,
                    Status = 1, // Active
                    CreatedAt = DateTime.UtcNow
                };
                _db.Tenants.Add(tenant);
                await _db.SaveChangesAsync(ct);

                user = new User
                {
                    Email = email,
                    FullName = name,
                    RoleId = 3,
                    IsVerified = true,
                    IsActive = true,
                    TenantId = tenant.Id,
                    CreatedAt = DateTime.UtcNow
                };
                _db.Users.Add(user);
                await _db.SaveChangesAsync(ct);

                // Tạo wallet với 5 credits miễn phí cho user mới
                await _wallet.CreditAsync(user.TenantId, 5, "WELCOME_BONUS", "Chào mừng bạn đến với StyleGenie!");
            }

            // Tạo JWT
            var token = JwtTokenHelper.GenerateToken(user,
                _config["Jwt:Key"]!, _config["Jwt:Issuer"]!);

            return Ok(new
            {
                token,
                user.Id,
                user.Email,
                user.FullName,
                roleId = user.RoleId,
                role = user.Role?.Name ?? "User"
            });
        }

        public class GoogleLoginRequest
        {
            [JsonPropertyName("token")]
            public string Token { get; set; } = default!;
        }
    }
}
