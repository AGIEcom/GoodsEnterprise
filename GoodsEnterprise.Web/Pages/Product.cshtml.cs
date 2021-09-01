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
        public ProductModel(IGeneralRepository<Product> product, IGeneralRepository<Category> category, 
            IGeneralRepository<SubCategory> subCategory, IGeneralRepository<Brand> brand)
        {
            _product = product;
            _category = category;
            _subCategory = subCategory;
            _brand = brand;
        }

        private readonly IGeneralRepository<Product> _product;
        private readonly IGeneralRepository<Category> _category;
        private readonly IGeneralRepository<SubCategory> _subCategory;
        private readonly IGeneralRepository<Brand> _brand;

        [BindProperty()]
        public Product objProduct { get; set; }

        [BindProperty]
        public IFormFile Upload { get; set; }

        public List<Product> lstproduct = new List<Product>();

        public Pagination PaginationModel { get; set; } = new Pagination();

        public SelectList selectBrands { get; set; } = new SelectList("");
        public SelectList selectCategories { get; set; } = new SelectList("");
        public SelectList selectSubCategories { get; set; } = new SelectList("");

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

                lstproduct = await _product.GetAllAsync(filter: x => x.IsDelete != true, orderBy: mt => mt.OrderByDescending(m => m.ModifiedDate == null ? m.CreatedDate : m.ModifiedDate));

                if (lstproduct == null || lstproduct?.Count == 0)
                {
                    ViewData["SuccessMsg"] = $"{Constants.NoRecordsFoundMessage}";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetAsync(), Product");
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
                await LoadBrand();
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
                await LoadCategoryByBrandId(objProduct.BrandId);
                await LoadSubCategoryByCategoryId(objProduct.CategoryId);
                ViewData["PageType"] = "Edit";
                ViewData["PagePrimaryID"] = objProduct.Id;
                ViewData["ImagePath"] = objProduct.Image;
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
                objProduct = await _product.GetAsync(filter: x => x.Id == productId && x.IsDelete != true);
                ViewData["PageType"] = "Edit";
                ViewData["PagePrimaryID"] = objProduct.Id;
                ViewData["ImagePath"] = objProduct.Image;
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
                            ViewData["ImagePath"] = objProduct.Image;
                        }
                        ViewData["SuccessMsg"] = $"Product: {objProduct.Code} {Constants.AlreadyExistMessage}";
                        return Page();
                    }
                }

                Tuple<string, string> tupleImagePath = await Common.UploadImages(Upload, objProduct.Code, Constants.Product);

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
        /// LoadCategoryByBrandId
        /// </summary>
        /// <param name="brandId"></param>
        /// <returns></returns>
        private async Task LoadCategoryByBrandId(int? brandId)
        {
            try
            {
                selectCategories = new SelectList(await _category.GetAllAsync(filter: x => x.IsDelete != true && x.BrandId == brandId),
                                          "Id", "Name", null);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in LoadCategoryByBrandId(), Product, brandId: { brandId }");
                throw;
            }
        }

        /// <summary>
        /// LoadCategoryByBrandId
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        private async Task LoadSubCategoryByCategoryId(int? categoryId)
        {
            try
            {
                selectSubCategories = new SelectList(await _subCategory.GetAllAsync(filter: x => x.IsDelete != true && x.CategoryId == categoryId),
                                          "Id", "Name", null);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in LoadSubCategoryByCategoryId(), Product, categoryId: { categoryId }");
                throw;
            }
        }
        /// <summary>
        /// OnGetSubCategories
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnGetSubCategories(int? categoryId)
        {
            try
            {
                return new JsonResult(await _subCategory.GetAllAsync(filter: x => x.IsDelete != true && x.CategoryId == categoryId));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetSubCategories(), Product, categoryId: { categoryId }");
                throw;
            }            
        }

        /// <summary>
        /// OnGetCategories
        /// </summary>
        /// <param name="brandId"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnGetCategories(int? brandId)
        {
            try
            {
                return new JsonResult(await _category.GetAllAsync(filter: x => x.IsDelete != true && x.BrandId == brandId));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetCategories(), Product, brandId: { brandId }");
                throw;
            }           
        }
    }
}

