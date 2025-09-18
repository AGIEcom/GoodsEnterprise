using GoodsEnterprise.DataAccess.Interface;
using GoodsEnterprise.Model.Models;
using GoodsEnterprise.Web.Utilities;
using JqueryDataTables.ServerSide.AspNetCoreWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoodsEnterprise.Web.Pages
{
    public class ProductModel : PageModel
    {
        /// <summary>
        /// ProductModel
        /// </summary>
        /// <param name="product"></param>
        /// <param name="configuration"></param>
        public ProductModel(IGeneralRepository<Product> product, IGeneralRepository<Category> category,
            IGeneralRepository<SubCategory> subCategory, IGeneralRepository<Brand> brand,IGeneralRepository<Tax> tax, IGeneralRepository<Supplier> supplier, IConfiguration configuration)
        {
            _product = product;
            _category = category;
            _subCategory = subCategory;
            _brand = brand;
            _tax = tax;
            _supplier = supplier;
            _configuration = configuration;
        }

        private readonly IGeneralRepository<Product> _product;
        private readonly IGeneralRepository<Category> _category;
        private readonly IGeneralRepository<SubCategory> _subCategory;
        private readonly IGeneralRepository<Tax> _tax;
        private readonly IGeneralRepository<Brand> _brand;
        private readonly IGeneralRepository<Supplier> _supplier;
        private readonly IConfiguration _configuration;

        [BindProperty()]
        public Product objProduct { get; set; }

        [BindProperty]
        public IFormFile Upload { get; set; }

        public List<Product> lstproduct = new List<Product>();

        public Pagination PaginationModel { get; set; } = new Pagination();

        public SelectList selectBrands { get; set; } = new SelectList("");
        public SelectList selectCategories { get; set; } = new SelectList("");
        public SelectList selectSubCategories { get; set; } = new SelectList("");
        public SelectList selectTaxSlab { get; set; } = new SelectList("");
        public SelectList selectSupplier { get; set; } = new SelectList("");

        /// <summary>
        /// OnGetAsync
        /// </summary>
        /// <returns></returns>


        /// <summary>
        /// OnGetCreateAsync
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetCreateAsync()
        {
            try
            {
                await LoadBrand();
                await LoadCategory();
                await LoadSubCategoryByCategoryId();
                await LoadTaxSlab();
                await loadSupplier();
                if (objProduct == null)
                {
                    objProduct = new Product();
                }
                objProduct.IsActive = true;
                ViewData["IsTaxable"] = false;
                ViewData["PageType"] = "Edit";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetCreateAsync(), Product");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnGetEditAsync
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetEditAsync(int productId)
        {
            try
            {
                objProduct = await _product.GetAsync(filter: x => x.Id == productId && x.IsDelete != true);

                if (objProduct == null)
                {
                    return Redirect("~/all-product");
                }
                await LoadBrand();
                await LoadCategory();
                await LoadSubCategoryByCategoryId();
                await LoadTaxSlab();
                await loadSupplier();
                ViewData["PageType"] = "Edit";
                ViewData["PagePrimaryID"] = objProduct.Id;
                ViewData["ImagePath"] = GetImageUrl(objProduct.Image);
                if (objProduct.TaxslabId == 0)
                {
                    ViewData["IsTaxable"] = false;
                    objProduct.isTaxable = false;
                }
                else
                {
                    ViewData["IsTaxable"] = true;
                    objProduct.isTaxable = true;
                }
                   
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetEditAsync(), Product, ProductId: { productId }");
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
                objProduct = new Product();
                objProduct.IsActive = false;
                ViewData["PageType"] = "Edit";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetClear(), Product");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnResetClear
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetReset(int productId)
        {
            try
            {
                await LoadBrand();
                await LoadCategory();
                await LoadSubCategoryByCategoryId();
                await LoadTaxSlab();
                await loadSupplier();
                objProduct = await _product.GetAsync(filter: x => x.Id == productId && x.IsDelete != true);
                ViewData["PageType"] = "Edit";
                ViewData["PagePrimaryID"] = objProduct.Id;
                ViewData["ImagePath"] = GetImageUrl(objProduct.Image);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnResetClear(), Product, ProductId: { objProduct.Id }");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnGetDeleteProductAsync
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetDeleteProductAsync(int productId)
        {
            try
            {
                var product = await _product.GetAsync(filter: x => x.Id == productId);
                if (product != null)
                {
                    await _product.LogicalDeleteAsync(product);
                    ViewData["SuccessMsg"] = $"Product: {product.Code} {Constants.DeletedMessage}";
                    HttpContext.Session.SetString(Constants.StatusMessage, $"Product: {product.Code} {Constants.DeletedMessage}");
                }

                ViewData["PageType"] = "List";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetDeleteProductAsync(), Product, ProductId: { productId }");
                throw;
            }
            return Redirect("~/all-product");
        }

        /// <summary>
        /// OnPostSubmitAsync
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostSubmitAsync()
        {
            try
            {
                Product existingProduct = await _product.GetAsync(filter: x => x.Code == objProduct.Code && x.IsDelete != true);
                if (existingProduct != null)
                {
                    if ((objProduct.Id == 0) || (objProduct.Id != 0 && objProduct.Id != existingProduct.Id))
                    {
                        ViewData["PageType"] = "Edit";
                        if (objProduct.Id != 0)
                        {
                            ViewData["PagePrimaryID"] = objProduct.Id;
                            ViewData["ImagePath"] = GetImageUrl(objProduct.Image);
                        }
                        ViewData["SuccessMsg"] = $"Product: {objProduct.Code} {Constants.AlreadyExistMessage}";
                        return Page();
                    }
                }

                Tuple<string, string> tupleImagePath = await Common.UploadImages(Upload, objProduct.Code, Constants.Product, _configuration);

                if (ModelState.IsValid)
                {
                    if (objProduct.Id == 0)
                    {
                        objProduct.Image = tupleImagePath.Item1;
                        await _product.InsertAsync(objProduct);
                        HttpContext.Session.SetString(Constants.StatusMessage, Constants.SaveMessage);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(tupleImagePath.Item1))
                        {
                            objProduct.Image = tupleImagePath.Item1;
                        }
                        await _product.UpdateAsync(objProduct);
                        HttpContext.Session.SetString(Constants.StatusMessage, Constants.UpdateMessage);
                    }
                    return Redirect("all-product");
                }
                else
                {
                    ViewData["PageType"] = "Edit";
                    await LoadBrand();
                    await LoadCategory();
                    await LoadSubCategoryByCategoryId();
                    await LoadTaxSlab();
                    await loadSupplier();
                    return Page();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnPostUploadFileAsync(), Product, ProductId: { objProduct?.Id }");
                throw;
            }
        }

        /// <summary>
        /// LoadBrand
        /// </summary>
        /// <returns></returns>
        private async Task LoadBrand()
        {
            try
            {
                selectBrands = new SelectList(await _brand.GetAllAsync(filter: x => x.IsDelete != true),
                                          "Id", "Name", null);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in LoadBrand()");
                throw;
            }
        }

        /// <summary>
        /// LoadCategory
        /// </summary>
        /// <returns></returns>
        private async Task LoadCategory()
        {
            try
            {
                selectCategories = new SelectList(await _category.GetAllAsync(filter: x => x.IsDelete != true),
                                          "Id", "Name", null);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in LoadCategoryByBrandId(), Product");
                throw;
            }
        }

        /// <summary>
        /// LoadSubCategoryByCategoryId
        /// </summary>
        /// <returns></returns>
        private async Task LoadSubCategoryByCategoryId()
        {
            try
            {
                selectSubCategories = new SelectList(await _subCategory.GetAllAsync(filter: x => x.IsDelete != true),
                                          "Id", "Name", null);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in LoadSubCategoryByCategoryId(), Product");
                throw;
            }
        }
        private async Task LoadTaxSlab()
        {
            try
            {
                selectTaxSlab = new SelectList(await _tax.GetAllAsync(filter: x => x.IsDelete != true),
                                          "Id", "Name", null);


            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in LoadTaxSlab(), Product");
                throw;
            }
        }
        private async Task loadSupplier()
        {

            try
            {
                selectSupplier = new SelectList(
                            await _supplier.GetAllAsync(filter: x => !x.IsDelete),
                            "Id",
                            "Name",
                            objProduct?.SupplierId
                        );
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in loadSupplier(), Product");
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

