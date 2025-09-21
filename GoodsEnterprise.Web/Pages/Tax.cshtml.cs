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
    /// TaxModel
    /// </summary>
    public class TaxModel : PageModel
    {
        /// <summary>
        /// TaxModel
        /// </summary>
        /// <param name="tax"></param>
        public TaxModel(IGeneralRepository<Tax> tax)
        {
            _tax = tax;
        }

        private readonly IGeneralRepository<Tax> _tax;

        [BindProperty()]
        public Tax objTax { get; set; }

        public List<Tax> lsttax = new List<Tax>();

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
                lsttax = await _tax.GetAllAsync(filter: x => x.IsDelete != true, orderBy: mt => mt.OrderByDescending(m => m.ModifiedDate == null ? m.CreatedDate : m.ModifiedDate));
                //if (lsttax == null || lsttax?.Count == 0)
                //{
                //    ViewData["SuccessMsg"] = $"{Constants.NoRecordsFoundMessage}";
                //}
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetAsync(), Tax");
                throw;
            }
        }

        /// <summary>
        /// OnGetEditAsync
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetEditAsync(int taxId)
        {
            try
            {
                objTax = await _tax.GetAsync(filter: x => x.Id == taxId && x.IsDelete != true);

                if (objTax == null)
                {
                    return Redirect("~/all-tax");
                }
                ViewData["PageType"] = "Edit";
                ViewData["PagePrimaryID"] = objTax.Id;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetEditAsync(), Tax, TaxId: { taxId }");
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
                objTax = new Tax();
                objTax.IsActive = false;
                ViewData["PageType"] = "Edit";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetClear(), Tax");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnResetClear
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetReset(int taxId)
        {
            try
            {
                objTax = await _tax.GetAsync(filter: x => x.Id == taxId && x.IsDelete != true);
                ViewData["PageType"] = "Edit";
                ViewData["PagePrimaryID"] = objTax.Id;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnResetClear(), Tax, TaxId: { objTax.Id }");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnGetDeleteTaxAsync
        /// </summary>
        /// <param name="taxId"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetDeleteTaxAsync(int taxId)
        {
            try
            {
                var tax = await _tax.GetAsync(filter: x => x.Id == taxId);
                if (tax != null)
                {
                    await _tax.LogicalDeleteAsync(tax);
                    ViewData["SuccessMsg"] = $"Tax: {tax.Name} {Constants.DeletedMessage}";
                    HttpContext.Session.SetString(Constants.StatusMessage, $"Tax: {tax.Name} {Constants.DeletedMessage}");
                }

                ViewData["PageType"] = "List";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetDeleteTaxAsync(), Tax, TaxId: { taxId }");
                throw;
            }
            return Redirect("~/all-tax");
        }

        /// <summary>
        /// OnPostSubmitAsync
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostSubmitAsync()
        {
            try
            {
                Tax existingTax = await _tax.GetAsync(filter: x => x.Name == objTax.Name && x.IsDelete != true);
                if (existingTax != null)
                {
                    if ((objTax.Id == 0) || (objTax.Id != 0 && objTax.Id != existingTax.Id))
                    {
                        ViewData["PageType"] = "Edit";
                        if (objTax.Id != 0)
                        {
                            ViewData["PagePrimaryID"] = objTax.Id;
                        }
                        ViewData["InfoMsg"] = $"Tax: {objTax.Name} {Constants.AlreadyExistMessage}";
                        return Page();
                    }
                }

                if (ModelState.IsValid)
                {
                    if (objTax.Id == 0)
                    {
                        await _tax.InsertAsync(objTax);
                        HttpContext.Session.SetString(Constants.StatusMessage, Constants.SaveMessage);
                    }
                    else
                    {
                        await _tax.UpdateAsync(objTax);
                        HttpContext.Session.SetString(Constants.StatusMessage, Constants.UpdateMessage);
                    }
                    return Redirect("all-tax");
                }
                else
                {
                    ViewData["PageType"] = "Edit";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnPostSubmitAsync(), Tax, TaxId: { objTax?.Id }");
                throw;
            }
        }
    }
}
