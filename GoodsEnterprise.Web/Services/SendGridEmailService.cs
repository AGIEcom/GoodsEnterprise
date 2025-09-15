using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace GoodsEnterprise.Web.Services
{
    public class SendGridEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SendGridEmailService> _logger;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly string _sendGridApiKey;

        public SendGridEmailService(IConfiguration configuration, ILogger<SendGridEmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _sendGridApiKey = _configuration["SendGrid:ApiKey"];
            _fromEmail = _configuration["SendGrid:FromEmail"] ?? "noreply@redneval.com";
            _fromName = _configuration["SendGrid:FromName"] ?? "Redneval Support";
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetLink)
        {
            try
            {
                if (string.IsNullOrEmpty(_sendGridApiKey))
                {
                    _logger.LogError("SendGrid API key is not configured");
                    return;
                }

                var client = new SendGridClient(_sendGridApiKey);
                var from = new EmailAddress(_fromEmail, _fromName);
                var to = new EmailAddress(email);
                const string subject = "Reset Your Password";
                
                var plainTextContent = $"Please reset your password by clicking here: {resetLink}";
                var htmlContent = $@"
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

                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await client.SendEmailAsync(msg);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Body.ReadAsStringAsync();
                    _logger.LogError($"Failed to send password reset email. Status: {response.StatusCode}, Error: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending password reset email to {email}");
                throw; // Re-throw to be handled by the caller
            }
        }
    }
}
