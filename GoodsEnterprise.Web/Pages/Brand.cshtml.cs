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
    /// <summary>
    /// BrandModel
    /// </summary>
    public class BrandModel : PageModel
    {
        /// <summary>
        /// BrandModel
        /// </summary>
        /// <param name="brand"></param>
        public BrandModel(IGeneralRepository<Brand> brand)
        {
            _brand = brand;
        }

        private readonly IGeneralRepository<Brand> _brand;

        [BindProperty()]
        public Brand objBrand { get; set; }

        [BindProperty]
        public IFormFile Upload { get; set; }

        public List<Brand> lstbrand = new List<Brand>();
        public Admin objAdmin = new Admin();

        public Pagination PaginationModel { get; set; } = new Pagination();

        /// <summary>
        /// OnGetAsync
        /// </summary>
        /// <param name="SearchByBrandName"></param>
        /// <param name="tablePageNo"></param>
        /// <param name="tablePageSize"></param>
        /// <returns></returns>
        public async Task OnGetAsync()
        {
            try
            {
                var _admin = HttpContext.Session.GetString(Constants.LoginSession);
                objAdmin = JsonConvert.DeserializeObject<Admin>(_admin);
                if (objAdmin != null)
                {
                    ViewData["PageType"] = "List";
                    if (!string.IsNullOrEmpty(HttpContext.Session.GetString(Constants.StatusMessage)))
                    {
                        ViewData["SuccessMsg"] = HttpContext.Session.GetString(Constants.StatusMessage);
                        HttpContext.Session.SetString(Constants.StatusMessage, "");
                    }
                    ViewData["PagePrimaryID"] = 0;
                    lstbrand = await _brand.GetAllAsync(filter: x => x.IsDelete != true, orderBy: mt => mt.OrderBy(m => m.ModifiedDate).ThenBy(m => m.CreatedDate));
                    if (lstbrand == null || lstbrand?.Count == 0)
                    {
                        ViewData["SuccessMsg"] = $"{Constants.NoRecordsFoundMessage}";
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetAsync(), Brand");
                throw;
            }
        }

        /// <summary>
        /// OnGetEditAsync
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetEditAsync(int brandId)
        {
            try
            {
                objBrand = await _brand.GetAsync(filter: x => x.Id == brandId && x.IsDelete != true);

                if (objBrand == null)
                {
                    return Redirect("~/all-brand");
                }
                ViewData["PageType"] = "Edit";
                ViewData["PagePrimaryID"] = objBrand.Id;
                ViewData["ImagePath"] = objBrand.ImageUrl500;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetEditAsync(), Brand, BrandId: { brandId }");
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
                objBrand = new Brand();
                objBrand.IsActive = false;
                ViewData["PageType"] = "Edit";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetClear(), Brand");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnResetClear
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetReset(int brandId)
        {
            try
            {
                objBrand = await _brand.GetAsync(filter: x => x.Id == brandId && x.IsDelete != true);
                ViewData["PageType"] = "Edit";
                ViewData["PagePrimaryID"] = objBrand.Id;
                ViewData["ImagePath"] = objBrand.ImageUrl500;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnResetClear(), Brand, BrandId: { objBrand.Id }");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnGetDeleteBrandAsync
        /// </summary>
        /// <param name="brandId"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetDeleteBrandAsync(int brandId)
        {
            try
            {
                var brand = await _brand.GetAsync(filter: x => x.Id == brandId);
                if (brand != null)
                {
                    await _brand.LogicalDeleteAsync(brand);
                    ViewData["SuccessMsg"] = $"Brand: {brand.Name} {Constants.DeletedMessage}";
                    HttpContext.Session.SetString(Constants.StatusMessage, $"Brand: {brand.Name} {Constants.DeletedMessage}");
                }

                ViewData["PageType"] = "List";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetDeleteBrandAsync(), Brand, BrandId: { brandId }");
                throw;
            }
            return Redirect("~/all-brand");
        }

        /// <summary>
        /// OnPostUploadFileAsync
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostUploadFileAsync()
        {
            try
            {
                Brand existingBrand = await _brand.GetAsync(filter: x => x.Name == objBrand.Name && x.IsDelete != true);
                if (existingBrand != null)
                {
                    if ((objBrand.Id == 0) || (objBrand.Id != 0 && objBrand.Id != existingBrand.Id))
                    {
                        ViewData["PageType"] = "Edit";
                        if (objBrand.Id != 0)
                        {
                            ViewData["PagePrimaryID"] = objBrand.Id;
                            ViewData["ImagePath"] = objBrand.ImageUrl500;
                        }
                        ViewData["SuccessMsg"] = $"Brand: {objBrand.Name} {Constants.AlreadyExistMessage}";
                        return Page();
                    }
                }

                Tuple<string, string> tupleImagePath = await Common.UploadImages(Upload, objBrand);

                if (ModelState.IsValid)
                {
                    var _admin = HttpContext.Session.GetString(Constants.LoginSession);
                    objAdmin = JsonConvert.DeserializeObject<Admin>(_admin);

                    if (objBrand.Id == 0)
                    {
                        objBrand.ImageUrl500 = tupleImagePath.Item1;
                        objBrand.ImageUrl200 = tupleImagePath.Item2;
                        objBrand.Createdby = objAdmin.Id;
                        objBrand.CreatedDate = DateTime.Now;
                        await _brand.InsertAsync(objBrand);
                        HttpContext.Session.SetString(Constants.StatusMessage, Constants.SaveMessage);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(tupleImagePath.Item1))
                        {
                            objBrand.ImageUrl500 = tupleImagePath.Item1;
                        }
                        if (!string.IsNullOrEmpty(tupleImagePath.Item2))
                        {
                            objBrand.ImageUrl200 = tupleImagePath.Item2;
                        }
                        objBrand.Modifiedby = objAdmin.Id;
                        objBrand.ModifiedDate = DateTime.Now;
                        await _brand.UpdateAsync(objBrand);
                        HttpContext.Session.SetString(Constants.StatusMessage, Constants.UpdateMessage);
                    }
                    return Redirect("all-brand");
                }
                else
                {
                    ViewData["PageType"] = "Edit";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnPostUploadFileAsync(), Brand, BrandId: { objBrand?.Id }");
                throw;
            }
        }
    }
}
