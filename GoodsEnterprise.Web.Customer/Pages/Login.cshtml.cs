using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoodsEnterprise.DataAccess.Interface;
using GoodsEnterprise.Model.Models.UserModel;
using GoodsEnterprise.Web.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Serilog;

namespace GoodsEnterprise.Web.Customer.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IGeneralRepository<Register> _register;

        [BindProperty()]
        public Register objRegister { get; set; }

        [BindProperty]
        public IFormFile Upload { get; set; }

        public List<Register> lstRegister = new List<Register>();

        public LoginModel(IGeneralRepository<Register> register)
        {
            _register = register;
        }

        public IActionResult OnGetUserLogin()
        {
            return Page();
        }
        public async Task<IActionResult> OnPostValidateLoginAsync()
        {
            try
            {

                Register existingLogin = await _register.GetAsync(filter: x => x.Email == objRegister.Email && x.Password == objRegister.Password);
                if (existingLogin != null)
                {
                    if (existingLogin.Password.Decrypt(Constants.EncryptDecryptSecurity) == objRegister.Password)
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

                Register existingLogin = await _register.GetAsync(filter: x => x.Email == objRegister.Email && x.Password == objRegister.Password);
                if (existingLogin != null)
                {
                    if (existingLogin.Password.Decrypt(Constants.EncryptDecryptSecurity) == objRegister.Password)
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
                    objRegister.Password = objRegister.Password.Encrypt(Constants.EncryptDecryptSecurity);

                    if (ModelState.IsValid)
                    {

                        await _register.InsertAsync(objRegister);
                        HttpContext.Session.SetString(Constants.StatusMessage, Constants.SaveMessage);

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

    }
}
