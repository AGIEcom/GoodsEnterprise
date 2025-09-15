using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace GoodsEnterprise.Web.Services
{
    public class GmailEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GmailEmailService> _logger;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly string _gmailPassword;
        private readonly string _smtpHost;
        private readonly int _smtpPort;

        public GmailEmailService(IConfiguration configuration, ILogger<GmailEmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _fromEmail = _configuration["Gmail:FromEmail"] ?? "noreply@redneval.com";
            _fromName = _configuration["Gmail:FromName"] ?? "Redneval Support";
            _gmailPassword = _configuration["Gmail:Password"];
            _smtpHost = _configuration["Gmail:SmtpHost"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(_configuration["Gmail:SmtpPort"] ?? "587");
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetLink)
        {
            try
            {
                if (string.IsNullOrEmpty(_gmailPassword))
                {
                    _logger.LogError("Gmail password is not configured");
                    return;
                }

                using var smtpClient = new SmtpClient(_smtpHost, _smtpPort)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(_fromEmail, _gmailPassword)
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_fromEmail, _fromName),
                    Subject = "Reset Your Password",
                    IsBodyHtml = true,
                    Body = GetPasswordResetEmailTemplate(resetLink)
                };

                mailMessage.To.Add(email);

                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation($"Password reset email sent successfully to {email}");
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError(smtpEx, $"SMTP error sending password reset email to {email}: {smtpEx.Message}");
                throw new InvalidOperationException("Failed to send email due to SMTP configuration issues", smtpEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending password reset email to {email}");
                throw;
            }
        }

        private string GetPasswordResetEmailTemplate(string resetLink)
        {
            return $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <div style='background-color: #2c5282; padding: 20px; text-align: center;'>
                        <h2 style='color: white; margin: 0;'>Redneval</h2>
                    </div>
                    <div style='padding: 20px;'>
                        <h3 style='color: #2d3748;'>Reset Your Password</h3>
                        <p>We received a request to reset the password for your account. Click the button below to set a new password:</p>
                        <div style='text-align: center; margin: 30px 0;'>
                            <a href='{resetLink}' 
                               style='background-color: #4299e1; 
                                      color: white; 
                                      padding: 12px 24px; 
                                      text-decoration: none; 
                                      border-radius: 4px; 
                                      font-weight: bold; 
                                      display: inline-block;'>
                                Reset Password
                            </a>
                        </div>
                        <p>If you didn't request this, you can safely ignore this email.</p>
                        <p>This password reset link will expire in 24 hours.</p>
                        <p>Thanks,<br>The Redneval Team</p>
                    </div>
                    <div style='background-color: #f7fafc; padding: 15px; text-align: center; font-size: 12px; color: #718096;'>
                        © {DateTime.UtcNow.Year} Redneval. All rights reserved.
                    </div>
                </div>";
        }
    }
}
