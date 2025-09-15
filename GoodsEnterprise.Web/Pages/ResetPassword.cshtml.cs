using GoodsEnterprise.DataAccess.Interface;
using GoodsEnterprise.Model.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Serilog;
using AutoMapper.Configuration;

namespace GoodsEnterprise.Web.Pages
{
    public class ResetPasswordModel : PageModel
    {
        private readonly IGeneralRepository<Admin> _admin;
        private readonly ILogger<ResetPasswordModel> _logger;
        private readonly IConfiguration _configuration;

        public ResetPasswordModel(
            IGeneralRepository<Admin> admin, 
            ILogger<ResetPasswordModel> logger,
            IConfiguration configuration)
        {
            _admin = admin;
            _logger = logger;
            _configuration = configuration;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
        public bool ShowResetForm { get; set; } = true;
        public string Email { get; set; }
        public string Token { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Token { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string email, string token)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
                {
                    ErrorMessage = "Invalid password reset token.";
                    return Page();
                }

                var admin = await _admin.GetAsync(filter: x => x.Email == email && 
                                                             x.ResetPasswordToken == token && 
                                                             x.ResetPasswordExpiry > DateTime.UtcNow);

                if (admin == null)
                {
                    ErrorMessage = "The password reset link is invalid or has expired. Please request a new one.";
                    return Page();
                }

                // Store email and token in hidden fields for form submission
                Email = email;
                Token = token;
                
                // Clear any existing model state to prevent validation errors
                ModelState.Clear();
                
                return Page();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetAsync(), Email: {email}");
                ErrorMessage = "An error occurred while processing your request. Please try again.";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                if (string.IsNullOrEmpty(Input.Email) || string.IsNullOrEmpty(Input.Token))
                {
                    ErrorMessage = "Invalid password reset request.";
                    return Page();
                }

                var admin = await _admin.GetAsync(filter: x => x.Email == Input.Email && 
                                                             x.ResetPasswordToken == Input.Token && 
                                                             x.ResetPasswordExpiry > DateTime.UtcNow);

                if (admin == null)
                {
                    ErrorMessage = "The password reset link is invalid or has expired. Please request a new one.";
                    return Page();
                }

                // Additional validation
                if (string.IsNullOrWhiteSpace(Input.Password) || Input.Password.Length < 8)
                {
                    ModelState.AddModelError("Input.Password", "Password must be at least 8 characters long.");
                    return Page();
                }

                if (Input.Password != Input.ConfirmPassword)
                {
                    ModelState.AddModelError("Input.ConfirmPassword", "The password and confirmation password do not match.");
                    return Page();
                }

                // Update password (the repository will handle hashing)
                admin.Password = Input.Password;
                admin.ResetPasswordToken = null;
                admin.ResetPasswordExpiry = null;
                admin.ModifiedDate = DateTime.UtcNow;

                await _admin.UpdateAsync(admin);

                // Clear the form
                Input.Password = string.Empty;
                Input.ConfirmPassword = string.Empty;
                
                // Set success message
                SuccessMessage = "Your password has been reset successfully. You can now log in with your new password.";
                
                // Clear the model state to show the success message
                ModelState.Clear();
                
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for {Email}", Input?.Email);
                ErrorMessage = "An error occurred while resetting your password. Please try again.";
                return Page();
            }
        }
    }
}
