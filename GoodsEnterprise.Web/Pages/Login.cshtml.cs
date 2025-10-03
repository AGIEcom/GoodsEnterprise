using GoodsEnterprise.DataAccess.Interface;
using GoodsEnterprise.Model.Models;
using GoodsEnterprise.Web.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using GoodsEnterprise.Web.Services;
using Microsoft.Extensions.Configuration;

namespace GoodsEnterprise.Web.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IGeneralRepository<Admin> _admin;
        private readonly IEmailService _emailService;
        public IConfiguration Configuration { get; }
        /// <summary>
        /// LoginModel
        /// </summary>
        /// <param name="admin"></param>
        /// <param name="emailService"></param>
        public LoginModel(IGeneralRepository<Admin> admin, IEmailService emailService, IConfiguration configuration)
        {
            _admin = admin;
            _emailService = emailService;
            Configuration = configuration;
        }

        [BindProperty]
        public ForgotPasswordModel ForgotPasswordModel { get; set; }
        [BindProperty()]
        public Admin objAdmin { get; set; }
        [BindProperty]
        public bool RememberMe { get; set; }

        /// <summary>
        /// OnGet - Check for remember me cookie
        /// </summary>
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Check if user is already logged in via session
                var sessionUser = HttpContext.Session.GetString(Constants.LoginSession);
                if (!string.IsNullOrEmpty(sessionUser))
                {
                    return RedirectToPage("Product");
                }

                // Check for remember me cookie
                if (Request.Cookies.TryGetValue("RememberMeToken", out string rememberToken) && 
                    !string.IsNullOrEmpty(rememberToken))
                {
                    var admin = await _admin.GetAsync(filter: x => x.RememberMeToken == rememberToken && 
                                                                   x.RememberMeExpiry > DateTime.UtcNow &&
                                                                   x.IsDelete != true);
                    if (admin != null)
                    {
                        // Auto-login the user
                        HttpContext.Session.SetString(Constants.LoginSession, JsonConvert.SerializeObject(admin));
                        
                        // Refresh the remember me token and extend expiry
                        await RefreshRememberMeToken(admin);
                        
                        return RedirectToPage("Product");
                    }
                    else
                    {
                        // Invalid or expired token, remove the cookie
                        Response.Cookies.Delete("RememberMeToken");
                    }
                }

                return Page();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in OnGetAsync() for Login page");
                return Page();
            }
        }

        /// <summary>
        /// OnGetLogOut
        /// </summary>
        public void OnGetLogOut()
        {
            HttpContext.Session.Clear();
            
            // Clear remember me cookie if it exists
            if (Request.Cookies.ContainsKey("RememberMeToken"))
            {
                Response.Cookies.Delete("RememberMeToken");
            }
        }
        /// <summary>
        /// Login Post method
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostLoginAsync()
        {
            try
            {
                Admin existingAdmin = await _admin.GetAsync(filter: x => x.Email == objAdmin.Email && x.IsDelete != true);
                
                if (existingAdmin != null)
                {
                    if (existingAdmin.Password.Decrypt(Constants.EncryptDecryptSecurity) == objAdmin.Password)
                    {
                        HttpContext.Session.SetString(Constants.LoginSession, JsonConvert.SerializeObject(existingAdmin));
                        
                        // Handle Remember Me functionality
                        if (RememberMe)
                        {
                            await SetRememberMeToken(existingAdmin);
                        }
                        else
                        {
                            // Clear any existing remember me token
                            await ClearRememberMeToken(existingAdmin);
                        }
                        
                        return RedirectToPage("Product");
                    }
                    else
                    {
                        ViewData["SuccessMsg"] = Constants.UserNamePasswordincorrect;
                        return Page();
                    }
                }
                else
                {                    
                     ViewData["SuccessMsg"] = Constants.UserNameNotavailable;
                    return Page();
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnPostLoginAsync(), Admin, AdminId: { objAdmin?.Id }");
                throw;
            }
        }

        /// <summary>
        /// Handles the forgot password request
        /// </summary>
        public async Task<IActionResult> OnPostForgotPasswordAsync()
        {
            try
            {
                var email = Request.Form["email"].ToString();
                
                if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                {
                    return new JsonResult(new { success = false, message = "Please enter a valid email address." });
                }

                var admin = await _admin.GetAsync(filter: x => x.Email == email && !x.IsDelete);
                
                if (admin == null)
                {
                    // Don't reveal that the user doesn't exist
                    return new JsonResult(new { 
                        success = true, 
                        message = "If your email is registered with us, you will receive a password reset link." 
                    });
                }

                // Generate a password reset token
                var token = Guid.NewGuid().ToString();
                admin.ResetPasswordToken = token;
                admin.ResetPasswordExpiry = DateTime.UtcNow.AddHours(24); // Token valid for 24 hours
                admin.ModifiedDate = DateTime.UtcNow;
                
                await _admin.UpdateAsync(admin);

                // Get the base URL from configuration or use the current request                
                var baseUrl = Configuration["Application:ResetPasswordUrl"] ??
                             $"{Request.Scheme}://{Request.Host}";

                // Send email with reset link
                var resetLink = $"{baseUrl}/reset-password?email={Uri.EscapeDataString(admin.Email)}&token={Uri.EscapeDataString(token)}";

                try
                {
                    await _emailService.SendPasswordResetEmailAsync(admin.Email, resetLink);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error sending password reset email to {Email}", admin.Email);
                    // Continue anyway to not reveal if the email exists
                }

                return new JsonResult(new { 
                    success = true, 
                    message = "If your email is registered with us, you will receive a password reset link." 
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnPostForgotPasswordAsync(), Email: {ForgotPasswordModel?.Email}");
                return new JsonResult(new { 
                    success = false, 
                    message = "An error occurred while processing your request. Please try again later." 
                });
            }
        }

        /// <summary>
        /// Set remember me token and cookie
        /// </summary>
        private async Task SetRememberMeToken(Admin admin)
        {
            try
            {
                // Generate a secure random token
                var token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString();
                
                // Set token and expiry in database (30 days)
                admin.RememberMeToken = token;
                admin.RememberMeExpiry = DateTime.UtcNow.AddDays(30);
                admin.ModifiedDate = DateTime.UtcNow;
                
                await _admin.UpdateAsync(admin);
                
                // Set secure HTTP-only cookie (30 days)
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddDays(30),
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Lax,
                    Path = "/"
                };
                
                Response.Cookies.Append("RememberMeToken", token, cookieOptions);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error setting remember me token for admin {AdminId}", admin.Id);
            }
        }

        /// <summary>
        /// Clear remember me token from database and cookie
        /// </summary>
        private async Task ClearRememberMeToken(Admin admin)
        {
            try
            {
                // Clear token from database
                admin.RememberMeToken = null;
                admin.RememberMeExpiry = null;
                admin.ModifiedDate = DateTime.UtcNow;
                
                await _admin.UpdateAsync(admin);
                
                // Clear cookie
                Response.Cookies.Delete("RememberMeToken");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error clearing remember me token for admin {AdminId}", admin.Id);
            }
        }

        /// <summary>
        /// Refresh remember me token to extend expiry
        /// </summary>
        private async Task RefreshRememberMeToken(Admin admin)
        {
            try
            {
                // Generate new token
                var newToken = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString();
                
                // Update database
                admin.RememberMeToken = newToken;
                admin.RememberMeExpiry = DateTime.UtcNow.AddDays(30);
                admin.ModifiedDate = DateTime.UtcNow;
                
                await _admin.UpdateAsync(admin);
                
                // Update cookie
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddDays(30),
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Lax,
                    Path = "/"
                };
                
                Response.Cookies.Append("RememberMeToken", newToken, cookieOptions);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error refreshing remember me token for admin {AdminId}", admin.Id);
            }
        }
    }

    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
