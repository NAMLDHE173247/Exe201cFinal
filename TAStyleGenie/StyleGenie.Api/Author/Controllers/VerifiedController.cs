using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StyleGenie.Api.Author.Service;
using StyleGenie.Infrastructure.Data.Models;

namespace StyleGenie.Api.Author.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VerifiedController : ControllerBase
    {
        private readonly EmailOtpService _otpService;
        private readonly ILogger<VerifiedController> _logger;
        private readonly TryOnDbContext _db;

        public VerifiedController(
            EmailOtpService otpService,
            ILogger<VerifiedController> logger,
            TryOnDbContext db)
        {
            _otpService = otpService;
            _logger = logger;
            _db = db;
        }

        // ✅ Gửi OTP chỉ khi IsVerified == false
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromQuery] string email)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return NotFound("❌ Không tìm thấy người dùng.");

            if (user.IsVerified)
                return BadRequest("✅ Email này đã được xác minh.");

            var otp = await _otpService.SendOtpAsync(email);
            _logger.LogInformation("✅ Gửi OTP cho {Email}: {OTP}", email, otp);

            return Ok("📩 Đã gửi mã OTP đến email.");
        }

        // ✅ Xác minh OTP → cập nhật IsVerified = true
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromQuery] string email, [FromQuery] string otp)
        {
            var isValid = _otpService.VerifyOtp(email, otp);
            if (!isValid)
                return BadRequest("❌ Mã OTP không chính xác.");

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return NotFound("❌ Không tìm thấy người dùng.");

            user.IsVerified = true;
            await _db.SaveChangesAsync();

            _otpService.ClearOtp(email);

            return Ok("✅ Xác minh email thành công!");
        }
    }
}
