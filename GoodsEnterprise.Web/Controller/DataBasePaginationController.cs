using GoodsEnterprise.DataAccess.Interface;
using GoodsEnterprise.Model.Models;
using JqueryDataTables.ServerSide.AspNetCoreWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoodsEnterprise.Web.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataBasePaginationController : ControllerBase
    {
        private readonly IGeneralRepository<ProductList> _product;
        private readonly IGeneralRepository<PromotionCostList> _promotionCost;
        private readonly IGeneralRepository<BaseCostList> _baseCost;
        private readonly IGeneralRepository<Category> _category;
        private readonly IGeneralRepository<SubCategory> _subCategory;
        private readonly IGeneralRepository<Brand> _brand;
       // public List<Product> lstproduct = new List<Product>();
        public DataBasePaginationController(IGeneralRepository<ProductList> product, IGeneralRepository<PromotionCostList> promotionCost, IGeneralRepository<BaseCostList> baseCost)
        {
            _product = product;
            _promotionCost = promotionCost;
            _baseCost = baseCost;
        }
        [HttpPost]
        [Route("getproductdata")]
        public async Task<IActionResult> LoadProductTable([FromBody] JqueryDataTablesParameters param)
        {
            try
            {
                
                List<ProductList> lstproduct = new List<ProductList>();
                
                // Extract SearchBy parameter from AdditionalValues
                string searchBy = "All";
                if (param.AdditionalValues != null && param.AdditionalValues.Any())
                {
                    searchBy = param.AdditionalValues.FirstOrDefault()?.ToString() ?? "All";
                }
                
                // Map column index to actual database column names for Product (removed ModifiedDate column)
                string[] productColumns = { "code", "productName", "categoryName", "brandName", "outerEan", "status" };
                int columnIndex = param.Order != null && param.Order.Any() ? param.Order[0].Column : -1;
                string sortColumn = columnIndex >= 0 && columnIndex < productColumns.Length ? productColumns[columnIndex] : "DefaultSort";

                DBPaginationParams dBPaginationParams = new DBPaginationParams()
                {
                    sortOrder = param.Order != null && param.Order.Any() ? param.Order[0].Dir.ToString() : "desc",
                    sortColumn = sortColumn,
                    OffsetValue = param.Start,
                    PagingSize = param.Length,
                    SearchText = param.Search.Value,
                    SearchBy = searchBy,
                    StoredProcuder= "SPUI_GetProductDetails"
                };

                lstproduct = await _product.GetAllWithPaginationAsync(dBPaginationParams);
                 
                if (lstproduct.Count == 0)
                {
                    var returnvaule = new JsonResult(new JqueryDataTablesResult<ProductList>
                    {
                        Draw = param.Draw,
                        Data = lstproduct,
                        RecordsFiltered = 0,
                        RecordsTotal = 0
                    });
                    return returnvaule;
                }
                else
                {
                    var returnvaule = new JsonResult(new JqueryDataTablesResult<ProductList>
                    {
                        Draw = param.Draw,
                        Data = lstproduct,
                        RecordsFiltered = lstproduct[0].FilterTotalCount,
                        RecordsTotal = lstproduct[0].FilterTotalCount
                    });
                    return returnvaule;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in LoadProductTable(), DataBasePaginationController");
                return new JsonResult(new { error = "Internal Server Error" });
            }
        }

        [HttpPost]
        [Route("getpromotioncostdata")]
        public async Task<IActionResult> LoadPromotionCostTable([FromBody] JqueryDataTablesParameters param)
        {
            try
            {

                List<PromotionCostList> lstproduct = new List<PromotionCostList>();
                
                // Extract SearchBy parameter from AdditionalValues
                string searchBy = "All";
                if (param.AdditionalValues != null && param.AdditionalValues.Any())
                {
                    searchBy = param.AdditionalValues.FirstOrDefault()?.ToString() ?? "All";
                }
                
                // Map column index to actual database column names for PromotionCost
                string[] promotionCostColumns = { "SupplierName", "ProductName", "PromotionCost", "StartDate", "EndDate", "Status", "PromotionCostID" };
                int columnIndex = param.Order != null && param.Order.Any() ? param.Order[0].Column : -1;
                string sortColumn = columnIndex >= 0 && columnIndex < promotionCostColumns.Length ? promotionCostColumns[columnIndex] : "DefaultSort";

                DBPaginationParams dBPaginationParams = new DBPaginationParams()
                {

                    sortOrder = param.Order != null && param.Order.Any() ? param.Order[0].Dir.ToString() : "desc",
                    sortColumn = sortColumn,
                    OffsetValue = param.Start,
                    PagingSize = param.Length,
                    SearchText = param.Search.Value,
                    SearchBy = searchBy,
                    StoredProcuder = "SPUI_GetPromotionCostDetails"

                };

                lstproduct = await _promotionCost.GetAllWithPaginationAsync(dBPaginationParams);

                if (lstproduct == null || lstproduct.Count == 0)
                {
                    var returnvaule = new JsonResult(new JqueryDataTablesResult<PromotionCostList>
                    {
                        Draw = param.Draw,
                        Data = lstproduct,
                        RecordsFiltered = 0,
                        RecordsTotal = 0
                    });
                    return returnvaule;
                }
                else
                {
                    var returnvaule = new JsonResult(new JqueryDataTablesResult<PromotionCostList>
                    {
                        Draw = param.Draw,
                        Data = lstproduct,
                        RecordsFiltered = lstproduct[0].FilterTotalCount,
                        RecordsTotal = lstproduct[0].FilterTotalCount
                    });
                    return returnvaule;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in LoadPromotionCostTable(), DataBasePaginationController");
                return new JsonResult(new { error = "Internal Server Error" });
            }
        }

        [HttpPost]
        [Route("getbasecostdata")]
        public async Task<IActionResult> LoadBaseCostTable([FromBody] JqueryDataTablesParameters param)
        {
            try
            {

                List<BaseCostList> lstproduct = new List<BaseCostList>();

                // Extract SearchBy parameter from AdditionalValues
                string searchBy = "All";
                if (param.AdditionalValues != null && param.AdditionalValues.Any())
                {
                    searchBy = param.AdditionalValues.FirstOrDefault()?.ToString() ?? "All";
                }

                // Map column index to actual database column names for BaseCost
                string[] baseCostColumns = { "SupplierName", "ProductName", "BaseCost", "StartDate", "EndDate", "Status", "BaseCostID" };
                int columnIndex = param.Order != null && param.Order.Any() ? param.Order[0].Column : -1;
                string sortColumn = columnIndex >= 0 && columnIndex < baseCostColumns.Length ? baseCostColumns[columnIndex] : "DefaultSort";

                DBPaginationParams dBPaginationParams = new DBPaginationParams()
                {

                    sortOrder = param.Order != null && param.Order.Any() ? param.Order[0].Dir.ToString() : "desc",
                    sortColumn = sortColumn,
                    OffsetValue = param.Start,
                    PagingSize = param.Length,
                    SearchText = param.Search.Value,
                    SearchBy = searchBy,
                    StoredProcuder = "SPUI_GetBaseCostDetails"

                };

                lstproduct = await _baseCost.GetAllWithPaginationAsync(dBPaginationParams);

                if (lstproduct == null || lstproduct.Count == 0)
                {
                    var returnvaule = new JsonResult(new JqueryDataTablesResult<BaseCostList>
                    {
                        Draw = param.Draw,
                        Data = lstproduct,
                        RecordsFiltered = 0,
                        RecordsTotal = 0
                    });
                    return returnvaule;
                }
                else
                {
                    var returnvaule = new JsonResult(new JqueryDataTablesResult<BaseCostList>
                    {
                        Draw = param.Draw,
                        Data = lstproduct,
                        RecordsFiltered = lstproduct[0].FilterTotalCount,
                        RecordsTotal = lstproduct[0].FilterTotalCount
                    });
                    return returnvaule;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in BaseCostTable(), DataBasePaginationController");
                return new JsonResult(new { error = "Internal Server Error" });
            }
        }
        

    }
}
