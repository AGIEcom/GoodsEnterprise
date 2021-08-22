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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GoodsEnterprise.Web.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IGeneralRepository<Admin> _admin;
        /// <summary>
        /// LoginModel
        /// </summary>
        /// <param name="admin"></param>
        public LoginModel(IGeneralRepository<Admin> admin)
        {
            _admin = admin;
        }
        [BindProperty()]
        public Admin objAdmin { get; set; }

        public void OnGet()
        {
        }
        /// <summary>
        /// Login Post method
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostLoginAsync()
        {
            try
            {
                Admin existingBrand = await _admin.GetAsync(filter: x => x.Email == objAdmin.Email && x.IsDelete != true);
                if (existingBrand != null)
                {
                    if(existingBrand.Password== objAdmin.Password)
                    {
                        HttpContext.Session.SetString(Constants.LoginSession, JsonConvert.SerializeObject(existingBrand));
                        //return RedirectPreserveMethod("~/all-brand"); 
                        return RedirectToPage("Brand");
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
    }
}
