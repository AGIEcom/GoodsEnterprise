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
        private readonly IGeneralRepository<Category> _category;
        private readonly IGeneralRepository<SubCategory> _subCategory;
        private readonly IGeneralRepository<Brand> _brand;
       // public List<Product> lstproduct = new List<Product>();
        public DataBasePaginationController(IGeneralRepository<ProductList> product, IGeneralRepository<Category> category,
          IGeneralRepository<SubCategory> subCategory, IGeneralRepository<Brand> brand)
        {
            _product = product;
            _category = category;
            _subCategory = subCategory;
            _brand = brand;
        }
        [HttpPost]
        [Route("getproductdata")]
        public async Task<IActionResult> LoadProductTable([FromBody] JqueryDataTablesParameters param)
        {
            try
            {
                //ViewData["PageType"] = "List";
                //if (!string.IsNullOrEmpty(HttpContext.Session.GetString(Constants.StatusMessage)))
                //{
                //    ViewData["SuccessMsg"] = HttpContext.Session.GetString(Constants.StatusMessage);
                //    HttpContext.Session.SetString(Constants.StatusMessage, "");
                //}
                //ViewData["PagePrimaryID"] = 0;
                List<ProductList> lstproduct = new List<ProductList>();
                //var myProducts = param.AdditionalValues.ToList();
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

                //if (lstproduct == null || lstproduct?.Count == 0)
                //{
                //    ViewData["SuccessMsg"] = $"{Constants.NoRecordsFoundMessage}";
                //}
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
                Log.Error(ex, $"Error in OnGetAsync(), Product");
                return new JsonResult(new { error = "Internal Server Error" });
            }
        }

    }
}
