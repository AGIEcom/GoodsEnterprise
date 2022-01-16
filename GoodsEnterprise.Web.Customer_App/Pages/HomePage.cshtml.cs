using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoodsEnterprise.DataAccess.Interface;
using GoodsEnterprise.Model.CustomerModel;
using GoodsEnterprise.Model.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace GoodsEnterprise.Web.Customer.Pages
{
    public class HomePageModel : PageModel
    {
        /// <summary>
        /// CategoryModel
        /// </summary>
        /// <param name="category"></param>
        public HomePageModel(IAdoDA adoDA, IGeneralRepository<Product> product)
        {
            _adoDA = adoDA;
            _product = product;
        }
        private readonly IAdoDA _adoDA;

        public List<HomePageCategory> lstcategory = new List<HomePageCategory>();

        public List<HomePageBrand> lstbrand = new List<HomePageBrand>();

        private readonly IGeneralRepository<Product> _product;

        public List<Product> lstProduct = new List<Product>();
        public async Task OnGetAsync()
        {
            try
            {
                LoadBrand();
                LoadCategory();
                //lstcategory = await _category.GetAllAsync(filter: x => x.IsDelete != true, orderBy: mt => mt.OrderByDescending(m => m.ModifiedDate == null ? m.CreatedDate : m.ModifiedDate));
                //lstbrand = await _brand.GetAllAsync(filter: x => x.IsDelete != true, orderBy: mt => mt.OrderByDescending(m => m.ModifiedDate == null ? m.CreatedDate : m.ModifiedDate));
                lstProduct = await _product.GetAllAsync(filter: x => x.IsDelete != true, orderBy: mt => mt.OrderByDescending(m => m.ModifiedDate == null ? m.CreatedDate : m.ModifiedDate));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetAsync(), HomePage");
                throw;
            }
        }

        public async Task OnGetBrandFilterAsync(int brandId)
        {
            try
            {
                LoadBrand();
                LoadCategory();
                lstProduct = await _product.GetAllAsync(filter: x => x.BrandId == brandId && x.IsDelete != true, orderBy: mt => mt.OrderByDescending(m => m.ModifiedDate == null ? m.CreatedDate : m.ModifiedDate));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetBrandFilterAsync(), HomePage");
                throw;
            }
        }
        public async Task OnGetCategoryFilterAsync(int CategoryId)
        {
            try
            {
                LoadBrand();
                LoadCategory();
                lstProduct = await _product.GetAllAsync(filter: x => x.CategoryId == CategoryId && x.IsDelete != true, orderBy: mt => mt.OrderByDescending(m => m.ModifiedDate == null ? m.CreatedDate : m.ModifiedDate));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetBrandFilterAsync(), HomePage");
                throw;
            }
        }
        private async Task LoadBrand()
        {
            try
            {
                lstbrand = await _adoDA.GetHomePageBrandAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in LoadBrand(), HomePage");
                throw;
            }
        }

        private async Task LoadCategory()
        {
            try
            {
                lstcategory = await _adoDA.GetHomePageCategoryAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in LoadCategory(), HomePage");
                throw;
            }
        }
    }
}
