using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using UserManagementApi.Services.Interface;

namespace UserManagementApi.Services.Implement
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                // Đọc cấu hình email
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                var smtpUsername = _configuration["EmailSettings:SmtpUsername"];
                var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
                var enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "true");
                var fromEmail = _configuration["EmailSettings:FromEmail"];
                var fromName = _configuration["EmailSettings:FromName"];

                _logger.LogInformation($"Sending email to: {to}, SMTP: {smtpServer}:{smtpPort}");

                using (var client = new SmtpClient(smtpServer, smtpPort))
                {
                    // Cấu hình authentication
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    client.EnableSsl = enableSsl;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    // Tạo email message
                    using (var mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress(fromEmail, fromName);
                        mailMessage.To.Add(to);
                        mailMessage.Subject = subject;
                        mailMessage.Body = body;
                        mailMessage.IsBodyHtml = true;

                        await client.SendMailAsync(mailMessage);
                        _logger.LogInformation($"Email sent successfully to: {to}");
                    }
                }
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError(smtpEx, $"SMTP error sending email to {to}: {smtpEx.Message}");
                throw new Exception($"Failed to send email: {smtpEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"General error sending email to {to}: {ex.Message}");
                throw new Exception($"Failed to send email: {ex.Message}");
            }
        }

        public async Task SendOtpEmailAsync(string email, string otp)
        {
            var subject = "Password Reset OTP - BookStore Admin";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #333; text-align: center;'>Password Reset Request</h2>
                        <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; text-align: center;'>
                            <p>You have requested to reset your password for BookStore.</p>
                            <p>Your OTP code is:</p>
                            <h1 style='background-color: #007bff; color: white; padding: 15px; border-radius: 5px; letter-spacing: 3px;'>{otp}</h1>
                            <p style='color: #dc3545; font-weight: bold;'>This OTP will expire in 2 minutes.</p>
                        </div>
                        <p style='margin-top: 20px; font-size: 12px; color: #6c757d; text-align: center;'>
                            If you did not request this password reset, please ignore this email.
                        </p>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(email, subject, body);
        }
    }
}