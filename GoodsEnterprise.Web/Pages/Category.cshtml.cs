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
    /// CategoryModel
    /// </summary>
    public class CategoryModel : PageModel
    {
        /// <summary>
        /// CategoryModel
        /// </summary>
        /// <param name="category"></param>
        public CategoryModel(IGeneralRepository<Category> category)
        {
            _category = category;
        }

        private readonly IGeneralRepository<Category> _category;

        [BindProperty()]
        public Category objCategory { get; set; }

        public List<Category> lstcategory = new List<Category>();

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
                lstcategory = await _category.GetAllAsync(filter: x => x.IsDelete != true, orderBy: mt => mt.OrderByDescending(m => m.ModifiedDate == null ? m.CreatedDate : m.ModifiedDate));
                if (lstcategory == null || lstcategory?.Count == 0)
                {
                    ViewData["SuccessMsg"] = $"{Constants.NoRecordsFoundMessage}";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetAsync(), Category");
                throw;
            }
        }

        /// <summary>
        /// OnGetEditAsync
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetEditAsync(int categoryId)
        {
            try
            {
                objCategory = await _category.GetAsync(filter: x => x.Id == categoryId && x.IsDelete != true); 
            
                if (objCategory == null)
                {
                    return Redirect("~/all-category");
                }
                ViewData["PageType"] = "Edit";
                ViewData["PagePrimaryID"] = objCategory.Id;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetEditAsync(), Category, CategoryId: { categoryId }");
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
                objCategory = new Category();
                objCategory.IsActive = false;
                ViewData["PageType"] = "Edit";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetClear(), Category");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnResetClear
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetReset(int categoryId)
        {
            try
            {
                objCategory = await _category.GetAsync(filter: x => x.Id == categoryId && x.IsDelete != true);
                ViewData["PageType"] = "Edit";
                ViewData["PagePrimaryID"] = objCategory.Id;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnResetClear(), Category, CategoryId: { objCategory.Id }");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnGetDeleteCategoryAsync
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetDeleteCategoryAsync(int categoryId)
        {
            try
            {
                var category = await _category.GetAsync(filter: x => x.Id == categoryId);
                if (category != null)
                {
                    await _category.LogicalDeleteAsync(category);
                    ViewData["SuccessMsg"] = $"Category: {category.Name} {Constants.DeletedMessage}";
                    HttpContext.Session.SetString(Constants.StatusMessage, $"Category: {category.Name} {Constants.DeletedMessage}");
                }

                ViewData["PageType"] = "List";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetDeleteCategoryAsync(), Category, CategoryId: { categoryId }");
                throw;
            }
            return Redirect("~/all-category");
        }

        /// <summary>
        /// OnPostSubmitAsync
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostSubmitAsync()
        {
            try
            {
                Category existingCategory = await _category.GetAsync(filter: x => x.Name == objCategory.Name && x.IsDelete != true);
                if (existingCategory != null)
                {
                    if ((objCategory.Id == 0) || (objCategory.Id != 0 && objCategory.Id != existingCategory.Id))
                    {
                        ViewData["PageType"] = "Edit";
                        if (objCategory.Id != 0)
                        {
                            ViewData["PagePrimaryID"] = objCategory.Id;
                        }
                        ViewData["SuccessMsg"] = $"Category: {objCategory.Name} {Constants.AlreadyExistMessage}";
                        return Page();
                    }
                }

                if (ModelState.IsValid)
                {
                    if (objCategory.Id == 0)
                    {
                        await _category.InsertAsync(objCategory);
                        HttpContext.Session.SetString(Constants.StatusMessage, Constants.SaveMessage);
                    }
                    else
                    {
                        await _category.UpdateAsync(objCategory);
                        HttpContext.Session.SetString(Constants.StatusMessage, Constants.UpdateMessage);
                    }
                    return Redirect("all-category");
                }
                else
                {
                    ViewData["PageType"] = "Edit";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnPostSubmitAsync(), Category, CategoryId: { objCategory?.Id }");
                throw;
            }
        }
    }
}
