using GoodsEnterprise.DataAccess.Interface;
using GoodsEnterprise.Model.Models;
using GoodsEnterprise.Web.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
        public BrandModel(IBrandDA brand)
        {
            _brand = brand;
        }

        [BindProperty(SupportsGet = true)]
        public Brand objBrand { get; set; }
        [BindProperty]
        public IFormFile Upload { get; set; }
        public List<Brand> lstbrand = new List<Brand>();
        private readonly IBrandDA _brand;
        public Pagination PaginationModel { get; set; } = new Pagination();

        /// <summary>
        /// OnGetAsync
        /// </summary>
        /// <param name="SearchByBrandName"></param>
        /// <param name="tablePageNo"></param>
        /// <param name="tablePageSize"></param>
        /// <returns></returns>
        public async Task OnGetAsync(string SearchByBrandName, int tablePageNo = 1, int tablePageSize = 5)
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

                PaginationModel.PageNumber = tablePageNo;
                PaginationModel.PageSize = tablePageSize;
                PaginationModel.CurrentFilter = SearchByBrandName;

                lstbrand = await _brand.GetBrandListAsync(PaginationModel);
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

        /// <summary>
        /// OnGetEditAsync
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetEditAsync(int brandId)
        {
            try
            {
                objBrand = await _brand.GetBrandAsync(brandId);
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
                objBrand = await _brand.GetBrandAsync(brandId);
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
                var brand = await _brand.GetBrandAsync(brandId);
                if (brand != null)
                {
                    brand.IsDelete = true;
                    await _brand.UpdateAsync(brand);
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
                Brand existingBrand = await _brand.GetBrandNameAsync(objBrand.Name);
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
                    if (objBrand.Id == 0)
                    {
                        objBrand.ImageUrl500 = tupleImagePath.Item1;
                        objBrand.ImageUrl200 = tupleImagePath.Item2;
                        await _brand.SaveAsync(objBrand);
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

        ///// <summary>
        ///// UploadImages
        ///// </summary>
        ///// <param name="Upload"></param>
        ///// <returns></returns>
        //private async Task<Tuple<string, string>> UploadImages(IFormFile Upload)
        //{
        //    string saveImage500 = string.Empty;
        //    string saveImage200 = string.Empty;

        //    try
        //    {
        //        if (Upload != null)
        //        {
        //            string uploadImageFolderPath = Path.Combine($"{Directory.GetCurrentDirectory()}{Constants.UploadPath + Constants.Brand}");
        //            string fileName500 = $"{objBrand.Name}_500_{DateTime.Now.ToString("yyyyMMddHHmmss")}{Path.GetExtension(Upload.FileName)}";
        //            string fileName200 = $"{objBrand.Name}_200_{DateTime.Now.ToString("yyyyMMddHHmmss")}{Path.GetExtension(Upload.FileName)}";
        //            string uploadImage500 = Path.Combine(uploadImageFolderPath, fileName500);
        //            string uploadImage200 = Path.Combine(uploadImageFolderPath, fileName200);

        //            using (var memoryStream = new MemoryStream())
        //            {
        //                await Upload.CopyToAsync(memoryStream);
        //                using (var image = Image.FromStream(memoryStream))
        //                {
        //                    Image imageResized500 = image.ResizeImage(250, 250);
        //                    imageResized500.Save(uploadImage500);
        //                    saveImage500 = Path.Combine(Constants.SavePath, Constants.Brand, fileName500);
        //                    //image.CompressImage(500, uploadImage);

        //                    Image imageResized200 = image.ResizeToThumbnail();
        //                    imageResized200.Save(uploadImage200);
        //                    saveImage200 = Path.Combine(Constants.SavePath, Constants.Brand, fileName200);
        //                }
        //            }
        //        }
        //        return Tuple.Create(saveImage500, saveImage200);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex, $"Error in UploadImages(), Brand");
        //        throw;
        //    }
        //}
    }
}
