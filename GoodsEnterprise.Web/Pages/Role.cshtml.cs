using GoodsEnterprise.DataAccess.Interface;
using GoodsEnterprise.Model.Models;
using GoodsEnterprise.Web.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Serilog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GoodsEnterprise.Web.Pages
{
    /// <summary>
    /// RoleModel
    /// </summary>
    public class RoleModel : PageModel
    {
        /// <summary>
        /// RoleModel
        /// </summary>
        /// <param name="role"></param>
        public RoleModel(IGeneralRepository<Role> role)
        {
            _role = role;
        }

        private readonly IGeneralRepository<Role> _role;

        [BindProperty()]
        public Role objRole { get; set; }

        public List<Role> lstrole = new List<Role>();

        public Pagination PaginationModel { get; set; } = new Pagination();

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
                lstrole = await _role.GetAllAsync(filter: x => x.IsDelete != true, orderBy: mt => mt.OrderByDescending(m => m.ModifiedDate == null ? m.CreatedDate : m.ModifiedDate));
                //if (lstrole == null || lstrole?.Count == 0)
                //{
                //    ViewData["SuccessMsg"] = $"{Constants.NoRecordsFoundMessage}";
                //}
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetAsync(), Role");
                throw;
            }
        }

        /// <summary>
        /// OnGetEditAsync
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetEditAsync(int roleId)
        {
            try
            {
                objRole = await _role.GetAsync(filter: x => x.Id == roleId && x.IsDelete != true);

                if (objRole == null)
                {
                    return Redirect("~/all-role");
                }
                ViewData["PageType"] = "Edit";
                ViewData["PagePrimaryID"] = objRole.Id;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetEditAsync(), Role, RoleId: { roleId }");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnGetClear
        /// </summary>
        /// <returns></returns>
        public IActionResult OnGetClear()
        {
            try
            {
                objRole = new Role();
                objRole.IsActive = false;
                ViewData["PageType"] = "Edit";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetClear(), Role");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnResetClear
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetReset(int roleId)
        {
            try
            {
                objRole = await _role.GetAsync(filter: x => x.Id == roleId && x.IsDelete != true);
                ViewData["PageType"] = "Edit";
                ViewData["PagePrimaryID"] = objRole.Id;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnResetClear(), Role, RoleId: { objRole.Id }");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnGetDeleteRoleAsync
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetDeleteRoleAsync(int roleId)
        {
            try
            {
                var role = await _role.GetAsync(filter: x => x.Id == roleId);
                if (role != null)
                {
                    await _role.LogicalDeleteAsync(role);
                    ViewData["SuccessMsg"] = $"Role: {role.Name} {Constants.DeletedMessage}";
                    HttpContext.Session.SetString(Constants.StatusMessage, $"Role: {role.Name} {Constants.DeletedMessage}");
                }

                ViewData["PageType"] = "List";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetDeleteRoleAsync(), Role, RoleId: { roleId }");
                throw;
            }
            return Redirect("~/all-role");
        }

        /// <summary>
        /// OnPostSubmitAsync
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostSubmitAsync()
        {
            try
            {
                Role existingRole = await _role.GetAsync(filter: x => x.Name == objRole.Name && x.IsDelete != true);
                if (existingRole != null)
                {
                    if ((objRole.Id == 0) || (objRole.Id != 0 && objRole.Id != existingRole.Id))
                    {
                        ViewData["PageType"] = "Edit";
                        if (objRole.Id != 0)
                        {
                            ViewData["PagePrimaryID"] = objRole.Id;
                        }
                        ViewData["InfoMsg"] = $"Role: {objRole.Name} {Constants.AlreadyExistMessage}";
                        return Page();
                    }
                }

                if (ModelState.IsValid)
                {
                    if (objRole.Id == 0)
                    {
                        await _role.InsertAsync(objRole);
                        HttpContext.Session.SetString(Constants.StatusMessage, Constants.SaveMessage);
                    }
                    else
                    {
                        await _role.UpdateAsync(objRole);
                        HttpContext.Session.SetString(Constants.StatusMessage, Constants.UpdateMessage);
                    }
                    return Redirect("all-role");
                }
                else
                {
                    ViewData["PageType"] = "Edit";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnPostSubmitAsync(), Role, RoleId: { objRole?.Id }");
                throw;
            }
        }
    }
}
