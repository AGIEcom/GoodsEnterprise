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
        private readonly IGeneralRepository<Category> _category;
        private readonly IGeneralRepository<SubCategory> _subCategory;
        private readonly IGeneralRepository<Brand> _brand;
       // public List<Product> lstproduct = new List<Product>();
        public DataBasePaginationController(IGeneralRepository<ProductList> product, IGeneralRepository<PromotionCostList> promotionCost)
        {
            _product = product;
            _promotionCost = promotionCost;
        }
        [HttpPost]
        [Route("getproductdata")]
        public async Task<IActionResult> LoadProductTable([FromBody] JqueryDataTablesParameters param)
        {
            try
            {
                
                List<ProductList> lstproduct = new List<ProductList>(); 
                DBPaginationParams dBPaginationParams = new DBPaginationParams()
                {

                    sortOrder = param.Order[0].Dir.ToString(),
                    sortColumn = param.SortOrder.Split(" ")[0],
                    OffsetValue = param.Draw,
                    PagingSize = param.Length,
                    SearchText = param.Search.Value,
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
                DBPaginationParams dBPaginationParams = new DBPaginationParams()
                {

                    sortOrder = param.Order[0].Dir.ToString(),
                    sortColumn = param.SortOrder.Split(" ")[0],
                    OffsetValue = param.Draw,
                    PagingSize = param.Length,
                    SearchText = param.Search.Value,
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

    }
}
