using StyleGenie.Api.Author.Service;

public class EmailOtpService
{
    private readonly EmailService _emailService;
    private readonly ILogger<EmailOtpService> _logger;

    // Make this static for debug testing
    private static readonly Dictionary<string, string> _otpStore = new();

    public EmailOtpService(EmailService emailService, ILogger<EmailOtpService> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<string> SendOtpAsync(string email)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var otp = new Random().Next(100000, 999999).ToString();

        _otpStore[normalizedEmail] = otp;
        _logger.LogWarning("👉 [SENT] OTP for {Email} = {OTP}", normalizedEmail, otp);

        var subject = "Mã xác minh StyleGenie của bạn";
        var htmlBody = $"<p>Mã OTP của bạn là: <strong>{otp}</strong></p>";

        await _emailService.SendAsync(email, subject, htmlBody);
        return otp;
    }

    public bool VerifyOtp(string email, string inputOtp)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var exists = _otpStore.TryGetValue(normalizedEmail, out var correctOtp);

        _logger.LogWarning("🔎 [VERIFY] Email={Email}, Input={InputOtp}, Found={Found}, Expected={Expected}",
            normalizedEmail, inputOtp, exists, correctOtp);

        return exists && correctOtp == inputOtp;
    }

    public void ClearOtp(string email)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        _otpStore.Remove(normalizedEmail);
    }
}
