using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoodsEnterprise.DataAccess.Interface;
using GoodsEnterprise.Model.Models.CustomerModel;
using GoodsEnterprise.Web.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;

namespace GoodsEnterprise.Web.Customer.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IGeneralRepository<GoodsEnterprise.Model.Models.Customer> _customer;

        private readonly IOptions<EmailSettings> _options;

        [BindProperty()]
        public GoodsEnterprise.Model.Models.Customer objCustomer { get; set; }

        [BindProperty]
        public IFormFile Upload { get; set; }

        public List<GoodsEnterprise.Model.Models.Customer> lstCustomer = new List<GoodsEnterprise.Model.Models.Customer>();

        public LoginModel(IGeneralRepository<GoodsEnterprise.Model.Models.Customer> customer, IOptions<EmailSettings> options)
        {
            _customer = customer;
            _options = options;
        }

        public IActionResult OnGetUserLogin()
        {
            return Page();
        }
        public async Task<IActionResult> OnPostValidateLoginAsync()
        {
            try
            {

                GoodsEnterprise.Model.Models.Customer existingLogin = await _customer.GetAsync(filter: x => x.Email == objCustomer.Email && x.Password == objCustomer.Password);
                if (existingLogin != null)
                {
                    if (existingLogin.Password.Decrypt(Constants.EncryptDecryptSecurity) == objCustomer.Password)
                    {
                        HttpContext.Session.SetString(Constants.LoginSession, JsonConvert.SerializeObject(existingLogin));
                        return RedirectToPage("UploadDownload");
                    }
                    else
                    {
                        ViewData["SuccessMsg"] = Constants.UserNamePasswordincorrect;
                        return Page();
                    }

                }
                else
                {
                    ViewData["SuccessMsg"] = Constants.UserNamePasswordincorrect;
                    return Page();
                }


            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnPostValidateLoginAsync()");
                throw;
            }
        }
        public async Task<IActionResult> OnPostRegisterAsync()
        {
            try
            {

                GoodsEnterprise.Model.Models.Customer existingLogin = await _customer.GetAsync(filter: x => x.Email == objCustomer.Email && x.Password == objCustomer.Password);
                if (existingLogin != null)
                {
                    if (existingLogin.Password.Decrypt(Constants.EncryptDecryptSecurity) == objCustomer.Password)
                    {
                        HttpContext.Session.SetString(Constants.LoginSession, JsonConvert.SerializeObject(existingLogin));
                        return RedirectToPage("UploadDownload");
                    }
                    else
                    {
                        ViewData["SuccessMsg"] = Constants.UserNamePasswordincorrect;
                        return Page();
                    }

                }
                else
                {
                    objCustomer.Password = objCustomer.Password.Encrypt(Constants.EncryptDecryptSecurity);
                    objCustomer.RoleId = 5;
                    if (ModelState.IsValid)
                    {

                        await _customer.InsertAsync(objCustomer);
                        HttpContext.Session.SetString(Constants.StatusMessage, Constants.SaveMessage);
                        string SubJect = "Confirmation: your account has been created";

                        Common.SendEmail(_options.Value, objCustomer.Email, SubJect, "");
                        return Redirect("user-index");
                    }
                    else
                    {

                        return Page();
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnPostValidateLoginAsync()");
                throw;
            }
        }
        private string RegisterBodyContent(string FirstName)
        {
            string rtrnMsg = "";
            return "";
        }

    }
}
