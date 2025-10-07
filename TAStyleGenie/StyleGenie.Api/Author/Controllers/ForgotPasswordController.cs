using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StyleGenie.Api.Author.Service;
using StyleGenie.Infrastructure.Data.Models;

namespace StyleGenie.Api.Author.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ForgotPasswordController : ControllerBase
    {
        private readonly TryOnDbContext _db;
        private readonly EmailOtpService _otp;
        private readonly ILogger<ForgotPasswordController> _logger;
        private readonly IPasswordHasher<User> _passwordHasher;

        public ForgotPasswordController(
            TryOnDbContext db,
            EmailOtpService otp,
            ILogger<ForgotPasswordController> logger,
            IPasswordHasher<User> passwordHasher)
        {
            _db = db;
            _otp = otp;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email không hợp lệ.");

            var e = email.Trim();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == e);
            if (user == null)
                return NotFound("Không tìm thấy người dùng.");

            // Tài khoản Google OAuth (PasswordHash == null) -> không gửi OTP
            if (string.IsNullOrEmpty(user.PasswordHash))
                return BadRequest("Tài khoản này đăng nhập bằng Google. Vui lòng dùng nút 'Đăng nhập với Google'.");

            await _otp.SendOtpAsync(e);
            _logger.LogInformation("ForgotPassword OTP sent to {Email}", e);
            return Ok("Đã gửi mã OTP đến email.");
        }

        public sealed class ResetPasswordRequest
        {
            public string? Email { get; set; }
            public string? Otp { get; set; }
            public string? NewPassword { get; set; }
        }

        [HttpPost("reset")]
        public async Task<IActionResult> Reset([FromBody] ResetPasswordRequest req)
        {
            if (req is null) return BadRequest("Payload rỗng.");

            var email = (req.Email ?? "").Trim();
            var otp = (req.Otp ?? "").Trim();
            var newPw = req.NewPassword ?? "";

            var err = ValidatePassword(newPw);
            if (err is not null) return BadRequest(err);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return NotFound("Không tìm thấy người dùng.");

            // Tài khoản Google OAuth -> không reset
            if (string.IsNullOrEmpty(user.PasswordHash))
                return BadRequest("Tài khoản này đăng nhập bằng Google. Không thể đặt lại mật khẩu nội bộ.");

            if (!_otp.VerifyOtp(email, otp))
                return BadRequest("Mã OTP không chính xác.");

            // ✅ Băm bằng PasswordHasher<User> (đồng bộ với luồng đăng ký)
            user.PasswordHash = _passwordHasher.HashPassword(user, newPw);
            user.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            _otp.ClearOtp(email);
            return Ok("Đặt lại mật khẩu thành công. Bạn có thể đăng nhập bằng mật khẩu mới.");
        }

        public static string? ValidatePassword(string pw)
        {
            if (string.IsNullOrEmpty(pw)) return "Mật khẩu không được để trống.";
            if (pw.Length < 8) return "Mật khẩu phải có ít nhất 8 ký tự.";
            if (!pw.Any(char.IsLower)) return "Mật khẩu phải có ít nhất 1 chữ thường.";
            if (!pw.Any(char.IsUpper)) return "Mật khẩu phải có ít nhất 1 chữ hoa.";
            if (!pw.Any(char.IsDigit)) return "Mật khẩu phải có ít nhất 1 chữ số.";
            if (!pw.Any(c => !char.IsLetterOrDigit(c))) return "Mật khẩu phải có ít nhất 1 ký tự đặc biệt.";
            if (pw.Any(char.IsWhiteSpace)) return "Mật khẩu không được chứa khoảng trắng.";
            return null;
        }
    }
}
