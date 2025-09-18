using GoodsEnterprise.DataAccess.Interface;
using GoodsEnterprise.Model.Models;
using GoodsEnterprise.Web.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
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
        /// <param name="configuration"></param>
        public BrandModel(IGeneralRepository<Brand> brand, IConfiguration configuration)
        {
            _brand = brand;
            _configuration = configuration;
        }

        private readonly IGeneralRepository<Brand> _brand;
        private readonly IConfiguration _configuration;

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
                ViewData["PageType"] = "List";
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString(Constants.StatusMessage)))
                {
                    ViewData["SuccessMsg"] = HttpContext.Session.GetString(Constants.StatusMessage);
                    HttpContext.Session.SetString(Constants.StatusMessage, "");
                }
                ViewData["PagePrimaryID"] = 0;

                lstbrand = await _brand.GetAllAsync(filter: x => x.IsDelete != true, orderBy: mt => mt.OrderByDescending(m => m.ModifiedDate == null ? m.CreatedDate : m.ModifiedDate));
                if (lstbrand == null || lstbrand?.Count == 0)
                {
                    ViewData["SuccessMsg"] = $"{Constants.NoRecordsFoundMessage}";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetAsync(), Brand");
                throw;
            }
        }

        public IActionResult OnPostBrandDetail()
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0; 
                var customerData = _brand.GetAllAsync(filter: x => x.IsDelete != true).Result.ToList();
                //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                //{
                //    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                //}
                //if (!string.IsNullOrEmpty(searchValue))
                //{
                //    customerData = customerData.Where(m => m.Name.Contains(searchValue));
                //}
                recordsTotal = customerData.Count();
                var data = customerData.Skip(skip).Take(pageSize).ToList();
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
                return new JsonResult(jsonData);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnPostBrandDetail(), Brand");
                return new JsonResult(new { error = "Internal Server Error" }); 
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
                ViewData["ImagePath"] = GetImageUrl(objBrand.ImageUrl500);
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
                ViewData["ImagePath"] = GetImageUrl(objBrand.ImageUrl500);
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
        /// OnPostSubmitAsync
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostSubmitAsync()
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
                            ViewData["ImagePath"] = GetImageUrl(objBrand.ImageUrl500);
                        }
                        ViewData["SuccessMsg"] = $"Brand: {objBrand.Name} {Constants.AlreadyExistMessage}";
                        return Page();
                    }
                }

                Tuple<string, string> tupleImagePath = await Common.UploadImages(Upload, objBrand.Name, Constants.Brand, _configuration);

                if (ModelState.IsValid)
                {
                    var _admin = HttpContext.Session.GetString(Constants.LoginSession);
                    objAdmin = JsonConvert.DeserializeObject<Admin>(_admin);

                    if (objBrand.Id == 0)
                    {
                        objBrand.ImageUrl500 = tupleImagePath.Item1;
                        objBrand.ImageUrl200 = tupleImagePath.Item2;
                        objBrand.Createdby = objAdmin.Id;
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
                Log.Error(ex, $"Error in OnPostSubmitAsync(), Brand, BrandId: { objBrand?.Id }");
                throw;
            }
        }

        /// <summary>
        /// Converts a file system path to an API URL for serving images
        /// </summary>
        /// <param name="filePath">The full file system path</param>
        /// <returns>The API URL to access the image</returns>
        private string GetImageUrl(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return string.Empty;

            try
            {
                string uploadPath = _configuration["Application:UploadPath"];
                
                // Check if the file path starts with the upload path
                if (filePath.StartsWith(uploadPath, StringComparison.OrdinalIgnoreCase))
                {
                    // Extract the relative path from the upload directory
                    string relativePath = filePath.Substring(uploadPath.Length).TrimStart('\\', '/');
                    
                    // Split into folder and filename
                    string[] pathParts = relativePath.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
                    
                    if (pathParts.Length >= 2)
                    {
                        string folder = pathParts[0];
                        string filename = pathParts[1];
                        return $"/api/Image/{folder}/{filename}";
                    }
                }
                
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
