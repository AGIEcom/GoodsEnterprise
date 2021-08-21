using GoodsEnterprise.DataAccess.Interface;
using GoodsEnterprise.Model.Models;
using GoodsEnterprise.Web.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
    /// SubCategoryModel
    /// </summary>
    public class SubCategoryModel : PageModel
    {
        /// <summary>
        /// SubCategoryModel
        /// </summary>
        /// <param name="subCategory"></param>
        public SubCategoryModel(IGeneralRepository<SubCategory> subCategory, IGeneralRepository<Category> Category)
        {
            _subCategory = subCategory;
            _category = Category;
        }

        private readonly IGeneralRepository<SubCategory> _subCategory;
        private readonly IGeneralRepository<Category> _category;

        [BindProperty()]
        public SubCategory objSubCategory { get; set; }

        [BindProperty]
        public IFormFile Upload { get; set; }

        public List<SubCategory> lstsubCategory = new List<SubCategory>();

        public Pagination PaginationModel { get; set; } = new Pagination();
        [BindProperty]
        public SelectList Categories { get; set; }

        /// <summary>
        /// OnGetAsync
        /// </summary>
        /// <param name="SearchBySubCategoryName"></param>
        /// <param name="tablePageNo"></param>
        /// <param name="tablePageSize"></param>
        /// <returns></returns>
        public async Task OnGetAsync(string SearchBySubCategoryName, int tablePageNo = 1, int tablePageSize = 5)
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
                PaginationModel.CurrentFilter = SearchBySubCategoryName;
                PaginationModel.StoreProcedure = "[dbo].[USP_GetSubCategories]";
                lstsubCategory = await _subCategory.GetAllWithPaginationAsync(PaginationModel);
               
                if (lstsubCategory == null || lstsubCategory?.Count == 0)
                {
                    ViewData["SuccessMsg"] = $"{Constants.NoRecordsFoundMessage}";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetAsync(), SubCategory");
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
                Categories = new SelectList(await _category.GetAllAsync(filter: x => x.IsDelete != true),
                           "Id", "Name", null);
                ViewData["PageType"] = "Edit";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetCreateAsync(), SubCategory");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnGetEditAsync
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetEditAsync(int subCategoryId)
        {
            try
            {
                objSubCategory = await _subCategory.GetAsync(filter: x => x.Id == subCategoryId && x.IsDelete != true); 
            
                if (objSubCategory == null)
                {
                    return Redirect("~/all-subCategory");
                }
                Categories = new SelectList(await _subCategory.GetAllAsync(filter: x => x.IsDelete != true),
                           "CategoryId", "Name", null);
                ViewData["PageType"] = "Edit";
                ViewData["PagePrimaryID"] = objSubCategory.Id;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetEditAsync(), SubCategory, SubCategoryId: { subCategoryId }");
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
                objSubCategory = new SubCategory();
                objSubCategory.IsActive = false;
                ViewData["PageType"] = "Edit";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetClear(), SubCategory");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnResetClear
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetReset(int subCategoryId)
        {
            try
            {
                objSubCategory = await _subCategory.GetAsync(filter: x => x.Id == subCategoryId && x.IsDelete != true);
                ViewData["PageType"] = "Edit";
                ViewData["PagePrimaryID"] = objSubCategory.Id;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnResetClear(), SubCategory, SubCategoryId: { objSubCategory.Id }");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnGetDeleteSubCategoryAsync
        /// </summary>
        /// <param name="subCategoryId"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetDeleteSubCategoryAsync(int subCategoryId)
        {
            try
            {
                var subCategory = await _subCategory.GetAsync(filter: x => x.Id == subCategoryId);
                if (subCategory != null)
                {
                    await _subCategory.LogicalDeleteAsync(subCategory);
                    ViewData["SuccessMsg"] = $"SubCategory: {subCategory.Name} {Constants.DeletedMessage}";
                    HttpContext.Session.SetString(Constants.StatusMessage, $"SubCategory: {subCategory.Name} {Constants.DeletedMessage}");
                }

                ViewData["PageType"] = "List";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetDeleteSubCategoryAsync(), SubCategory, SubCategoryId: { subCategoryId }");
                throw;
            }
            return Redirect("~/all-subCategory");
        }

        /// <summary>
        /// OnPostUploadFileAsync
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostUploadFileAsync()
        {
            try
            {
                SubCategory existingSubCategory = await _subCategory.GetAsync(filter: x => x.Name == objSubCategory.Name && x.IsDelete != true);
                if (existingSubCategory != null)
                {
                    if ((objSubCategory.Id == 0) || (objSubCategory.Id != 0 && objSubCategory.Id != existingSubCategory.Id))
                    {
                        ViewData["PageType"] = "Edit";
                        if (objSubCategory.Id != 0)
                        {
                            ViewData["PagePrimaryID"] = objSubCategory.Id;
                        }
                        ViewData["SuccessMsg"] = $"SubCategory: {objSubCategory.Name} {Constants.AlreadyExistMessage}";
                        return Page();
                    }
                }

                if (ModelState.IsValid)
                {
                    if (objSubCategory.Id == 0)
                    {
                        await _subCategory.InsertAsync(objSubCategory);
                        HttpContext.Session.SetString(Constants.StatusMessage, Constants.SaveMessage);
                    }
                    else
                    {
                        await _subCategory.UpdateAsync(objSubCategory);
                        HttpContext.Session.SetString(Constants.StatusMessage, Constants.UpdateMessage);
                    }
                    return Redirect("all-subCategory");
                }
                else
                {
                    ViewData["PageType"] = "Edit";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnPostUploadFileAsync(), SubCategory, SubCategoryId: { objSubCategory?.Id }");
                throw;
            }
        }
    }
}
