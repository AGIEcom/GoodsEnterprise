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
    /// SupplierModel
    /// </summary>
    public class SupplierModel : PageModel
    {
        /// <summary>
        /// SupplierModel
        /// </summary>
        /// <param name="supplier"></param>
        public SupplierModel(IGeneralRepository<Supplier> supplier)
        {
            _supplier = supplier;
        }

        private readonly IGeneralRepository<Supplier> _supplier;

        [BindProperty()]
        public Supplier objSupplier { get; set; }

        public List<Supplier> lstsupplier = new List<Supplier>();

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
                lstsupplier = await _supplier.GetAllAsync(filter: x => x.IsDelete != true, orderBy: mt => mt.OrderByDescending(m => m.ModifiedDate == null ? m.CreatedDate : m.ModifiedDate));
                if (lstsupplier == null || lstsupplier?.Count == 0)
                {
                    ViewData["SuccessMsg"] = $"{Constants.NoRecordsFoundMessage}";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetAsync(), Supplier");
                throw;
            }
        }

        /// <summary>
        /// OnGetEditAsync
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetEditAsync(int supplierId)
        {
            try
            {
                objSupplier = await _supplier.GetAsync(filter: x => x.Id == supplierId && x.IsDelete != true);

                if (objSupplier == null)
                {
                    return Redirect("~/all-supplier");
                }
                ViewData["PageType"] = "Edit";
                ViewData["PagePrimaryID"] = objSupplier.Id;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetEditAsync(), Supplier, SupplierId: { supplierId }");
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
                objSupplier = new Supplier();
                objSupplier.IsActive = false;
                ViewData["PageType"] = "Edit";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetClear(), Supplier");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnResetClear
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetReset(int supplierId)
        {
            try
            {
                objSupplier = await _supplier.GetAsync(filter: x => x.Id == supplierId && x.IsDelete != true);
                ViewData["PageType"] = "Edit";
                ViewData["PagePrimaryID"] = objSupplier.Id;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnResetClear(), Supplier, SupplierId: { objSupplier.Id }");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnGetDeleteSupplierAsync
        /// </summary>
        /// <param name="supplierId"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetDeleteSupplierAsync(int supplierId)
        {
            try
            {
                var supplier = await _supplier.GetAsync(filter: x => x.Id == supplierId);
                if (supplier != null)
                {
                    await _supplier.LogicalDeleteAsync(supplier);
                    ViewData["SuccessMsg"] = $"Supplier: {supplier.Name} {Constants.DeletedMessage}";
                    HttpContext.Session.SetString(Constants.StatusMessage, $"Supplier: {supplier.Name} {Constants.DeletedMessage}");
                }

                ViewData["PageType"] = "List";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetDeleteSupplierAsync(), Supplier, SupplierId: { supplierId }");
                throw;
            }
            return Redirect("~/all-supplier");
        }

        /// <summary>
        /// OnPostSubmitAsync
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostSubmitAsync()
        {
            try
            {
                Supplier existingSupplier = await _supplier.GetAsync(filter: x => x.Name == objSupplier.Name && x.IsDelete != true);
                if (existingSupplier != null)
                {
                    if ((objSupplier.Id == 0) || (objSupplier.Id != 0 && objSupplier.Id != existingSupplier.Id))
                    {
                        ViewData["PageType"] = "Edit";
                        if (objSupplier.Id != 0)
                        {
                            ViewData["PagePrimaryID"] = objSupplier.Id;
                        }
                        ViewData["SuccessMsg"] = $"Supplier: {objSupplier.Name} {Constants.AlreadyExistMessage}";
                        return Page();
                    }
                }

                if (ModelState.IsValid)
                {
                    if (objSupplier.Id == 0)
                    {
                        await _supplier.InsertAsync(objSupplier);
                        HttpContext.Session.SetString(Constants.StatusMessage, Constants.SaveMessage);
                    }
                    else
                    {
                        await _supplier.UpdateAsync(objSupplier);
                        HttpContext.Session.SetString(Constants.StatusMessage, Constants.UpdateMessage);
                    }
                    return Redirect("all-supplier");
                }
                else
                {
                    ViewData["PageType"] = "Edit";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnPostSubmitAsync(), Supplier, SupplierId: { objSupplier?.Id }");
                throw;
            }
        }
    }
}
