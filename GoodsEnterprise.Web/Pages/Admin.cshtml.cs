using GoodsEnterprise.DataAccess.Interface;
using GoodsEnterprise.Model.Models;
using GoodsEnterprise.Web.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GoodsEnterprise.Web.Pages
{
    /// <summary>
    /// AdminModel
    /// </summary>
    public class AdminModel : PageModel
    {
        /// <summary>
        /// AdminModel
        /// </summary>
        /// <param name="admin"></param>
        public AdminModel(IGeneralRepository<Admin> admin, IGeneralRepository<Role> role)
        {
            _admin = admin;
            _role = role;
        }

        private readonly IGeneralRepository<Admin> _admin;
        private readonly IGeneralRepository<Role> _role;

        [BindProperty()]
        public Admin objAdmin { get; set; }

        public List<Admin> lstadmin = new List<Admin>();

        public Pagination PaginationModel { get; set; } = new Pagination();
        public SelectList Roles { get; set; } = new SelectList("");

        /// <summary>
        /// OnGetAsync
        /// </summary>
        /// <returns></returns>
        public async Task OnGetAsync()
        {
            try
            {
                ViewData["PageType"] = "List";
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString(Constants.StatusMessage)))
                {
                    ViewData["SuccessMsg"] = HttpContext.Session.GetString(Constants.StatusMessage);
                    HttpContext.Session.SetString(Constants.StatusMessage, "");
                }
                ViewData["PagePrimaryID"] = 0;
                lstadmin = await _admin.GetAllAsync(filter: x => x.IsDelete != true, orderBy: mt => mt.OrderByDescending(m => m.ModifiedDate == null ? m.CreatedDate : m.ModifiedDate));
                //if (lstadmin == null || lstadmin?.Count == 0)
                //{
                //    ViewData["SuccessMsg"] = $"{Constants.NoRecordsFoundMessage}";
                //}
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetAsync(), Admin");
                throw;
            }
        }

        /// <summary>
        /// OnGetCreateAsync
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetCreateAsync()
        {
            try
            {
                await LoadRole();
                ViewData["PageType"] = "Edit";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetCreateAsync(), Admin");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnGetEditAsync
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetEditAsync(int adminId)
        {
            try
            {
                objAdmin = await _admin.GetAsync(filter: x => x.Id == adminId && x.IsDelete != true);

                if (objAdmin == null)
                {
                    return Redirect("~/all-admin");
                }
                await LoadRole();
                ViewData["PageType"] = "Edit";
                ViewData["PagePrimaryID"] = objAdmin.Id;
                objAdmin.Password = objAdmin.Password.Decrypt(Constants.EncryptDecryptSecurity);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetEditAsync(), Admin, AdminId: { adminId }");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnGetClear
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetClear()
        {
            try
            {
                await LoadRole();
                objAdmin = new Admin();
                objAdmin.IsActive = false;
                ViewData["PageType"] = "Edit";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetClear(), Admin");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnResetClear
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetReset(int adminId)
        {
            try
            {
                await LoadRole();
                objAdmin = await _admin.GetAsync(filter: x => x.Id == adminId && x.IsDelete != true);
                ViewData["PageType"] = "Edit";
                ViewData["PagePrimaryID"] = objAdmin.Id;
                objAdmin.Password = objAdmin.Password.Decrypt(Constants.EncryptDecryptSecurity);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnResetClear(), Admin, AdminId: { objAdmin.Id }");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnGetDeleteAdminAsync
        /// </summary>
        /// <param name="adminId"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetDeleteAdminAsync(int adminId,string firstName)
        {
            try
            {
                var admin = await _admin.GetAsync(filter: x => x.Id == adminId);
                if (admin != null)
                {
                    await _admin.LogicalDeleteAsync(admin);
                    ViewData["SuccessMsg"] = $"Admin: {admin.Email} {Constants.DeletedMessage}";
                    HttpContext.Session.SetString(Constants.StatusMessage, $"Admin: {admin.Email} {Constants.DeletedMessage}");
                }

                ViewData["PageType"] = "List";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetDeleteAdminAsync(), Admin, AdminId: { adminId }");
                throw;
            }
            return Redirect("~/all-admin");
        }

        /// <summary>
        /// OnPostSubmitAsync
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostSubmitAsync()
        {
            try
            {
                Admin existingAdmin = await _admin.GetAsync(filter: x => x.Email == objAdmin.Email && x.IsDelete != true);
                if (existingAdmin != null)
                {
                    if ((objAdmin.Id == 0) || (objAdmin.Id != 0 && objAdmin.Id != existingAdmin.Id))
                    {
                        ViewData["PageType"] = "Edit";
                        if (objAdmin.Id != 0)
                        {
                            ViewData["PagePrimaryID"] = objAdmin.Id;
                        }
                        ViewData["InfoMsg"] = $"Admin: {objAdmin.Email} {Constants.AlreadyExistMessage}";
                        await LoadRole();
                        return Page();
                    }
                }

                if (ModelState.IsValid)
                {
                    if (objAdmin.Id == 0)
                    {
                        // New admin - password is required
                        if (objAdmin.Password != null)
                            objAdmin.Password = objAdmin.Password.Encrypt(Constants.EncryptDecryptSecurity);
                        await _admin.InsertAsync(objAdmin);
                        HttpContext.Session.SetString(Constants.StatusMessage, Constants.SaveMessage);
                    }
                    else
                    {
                        // Existing admin - handle password update
                        if (!string.IsNullOrEmpty(objAdmin.Password))
                        {
                            // Password is being changed
                            objAdmin.Password = objAdmin.Password.Encrypt(Constants.EncryptDecryptSecurity);
                        }
                        else
                        {
                            // Password is not being changed - preserve existing password
                            var existingAdminData = await _admin.GetAsync(filter: x => x.Id == objAdmin.Id);
                            if (existingAdminData != null)
                            {
                                objAdmin.Password = existingAdminData.Password;
                            }
                        }
                        await _admin.UpdateAsync(objAdmin);
                        HttpContext.Session.SetString(Constants.StatusMessage, Constants.UpdateMessage);
                    }
                    return Redirect("all-admin");
                }
                else
                {
                    await LoadRole();
                    ViewData["PageType"] = "Edit";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnPostSubmitAsync(), Admin, AdminId: { objAdmin?.Id }");
                throw;
            }
        }
      
        /// <summary>
        /// LoadBrand
        /// </summary>
        /// <returns></returns>
        private async Task LoadRole()
        {
            Roles = new SelectList(await _role.GetAllAsync(filter: x => x.IsDelete != true),
                           "Id", "Name", null);
        }
    }
}
