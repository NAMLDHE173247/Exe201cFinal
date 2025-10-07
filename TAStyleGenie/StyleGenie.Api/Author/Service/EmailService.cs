using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;

namespace StyleGenie.Api.Author.Service
{
    public class EmailService
    {
        private readonly IConfiguration _cfg;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration cfg, ILogger<EmailService> logger)
        {
            _cfg = cfg;
            _logger = logger;
        }

        /// <summary>
        /// Gửi email HTML đơn giản (có kèm Text fallback).
        /// </summary>
        public async Task SendAsync(
            string toEmail,
            string subject,
            string htmlBody,
            string? textBody = null,
            CancellationToken ct = default)
        {
            // Đọc cấu hình "Smtp"
            var s = _cfg.GetSection("Smtp");
            var host = s["Host"] ?? "smtp.gmail.com";
            var port = int.TryParse(s["Port"], out var p) ? p : 587;
            var useStartTls = bool.TryParse(s["UseStartTls"], out var tls) ? tls : true;
            var username = s["UserName"]!;
            var password = s["Password"]!;
            var from = s["From"] ?? username;
            var display = s["DisplayName"] ?? "Mailer";

            // Soạn message
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(display, from));
            msg.To.Add(new MailboxAddress(toEmail, toEmail));
            msg.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = htmlBody,
                TextBody = textBody ?? StripTags(htmlBody)
            };
            msg.Body = builder.ToMessageBody();

            // Gửi SMTP
            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(host, port, useStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto, ct);
                await smtp.AuthenticateAsync(username, password, ct);
                await smtp.SendAsync(msg, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Send email failed to {To}", toEmail);
                throw;
            }
            finally
            {
                try { await smtp.DisconnectAsync(true, ct); } catch { /* ignore */ }
            }
        }

        // Fallback nhanh cho TextBody
        private static string StripTags(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;
            return System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty);
        }
    }
}
