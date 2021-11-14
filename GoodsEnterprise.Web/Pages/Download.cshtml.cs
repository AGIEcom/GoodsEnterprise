using ClosedXML.Excel;
using GoodsEnterprise.DataAccess.Interface;
using GoodsEnterprise.Model.Models;
using GoodsEnterprise.Web.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GoodsEnterprise.Web.Pages
{
    public class DownloadModel : PageModel
    {
        public DownloadModel(IGeneralRepository<Product> product, GoodsEnterpriseContext context)
        {
            _product = product;
            _context = context;
        }
        private readonly IGeneralRepository<Product> _product;
        private readonly GoodsEnterpriseContext _context;
        public List<Product> products { get; set; }

        /// <summary>
        /// OnPostSubmitAsync
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostSubmitAsync()
        {
            try
            {
                //products = await _product.GetAllAsync(filter: x => x.IsDelete != true);

                List<ProductDownload> ProductDownloads = (from pd in _context.Products.AsNoTracking()
                                                        join brd in _context.Brands.AsNoTracking() on pd.BrandId equals brd.Id into brds
                                                        from brd in brds.DefaultIfEmpty()
                                                        join cat in _context.Categories.AsNoTracking() on pd.CategoryId equals cat.Id into cats
                                                        from cat in cats.DefaultIfEmpty()
                                                        join subcat in _context.SubCategories.AsNoTracking() on pd.SubCategoryId equals subcat.Id into subcats
                                                        from subcat in subcats.DefaultIfEmpty()
                                                        join sup in _context.Suppliers.AsNoTracking() on pd.SupplierId equals sup.Id into sups
                                                        from sup in sups.DefaultIfEmpty()                                       
                                                        //orderby pd.ModifiedDate == null ? pd.CreatedDate : pd.ModifiedDate descending
                                                        select new ProductDownload
                                                        {                                           
                                                            Code = pd.Code,
                                                            Brand = brd.Name,
                                                            Category = cat.Name,                                          
                                                            SubCategory = subcat.Name,
                                                            Supplier = sup.Name,
                                                            InnerEan = pd.InnerEan,
                                                            OuterEan = pd.OuterEan,
                                                            PackSize = pd.PackSize,
                                                            Upc = pd.Upc,
                                                            LayerQuantity = pd.LayerQuantity,
                                                            PalletQuantity = pd.PalletQuantity,
                                                            Height = pd.Height,
                                                            Weight = pd.Weight,
                                                            Width = pd.Width,
                                                            NetWeight = pd.NetWeight,
                                                            Depth = pd.Depth,
                                                            CreatedDate = pd.CreatedDate,
                                                            Createdby = pd.Createdby,
                                                            ModifiedDate = pd.ModifiedDate,
                                                            Modifiedby = pd.Modifiedby,
                                                            IsActive = pd.IsActive,
                                                            IsDelete = pd.IsDelete,
                                                            ExpriyDate = pd.ExpriyDate
                                                        }).ToList();

                ExportToExcel(ProductDownloads.ToDataTable());
                ViewData["SuccessMsg"] = $"{Constants.SuccessUpload}";
                return Page();
            }
            catch (Exception ex)
            {
                ViewData["SuccessMsg"] = $"{Constants.FailureUpload}";
                Log.Error(ex, $"Error in OnPostSubmitAsync(), Download");
                throw;
            }
        }
        ///// <summary>
        ///// ExportToExcel
        ///// </summary>
        ///// <param name="products"></param>
        //private void ExportToExcel(DataTable products)
        //{
        //    try
        //    {
        //        using (var workbook = new XLWorkbook())
        //        {
        //            var worksheet = workbook.Worksheets.Add("Products");
        //            var currentRow = 1;

        //            for (int i = 0; i < Constants.ProductFieldsDownload.Count(); i++)
        //            {
        //                worksheet.Cell(currentRow, i + 1).Value = Constants.ProductFieldsDownload[i];
        //            }

        //            for (int i = 0; i < products.Rows.Count; i++)
        //            {
        //                {
        //                    currentRow++;
        //                    for (int j = 0; j < Constants.ProductFieldsDownload.Count(); j++)
        //                    {
        //                        worksheet.Cell(currentRow, j + 1).Value = products.Rows[i][Constants.ProductFieldsDownload[j]];
        //                    }
        //                }
        //            }
        //            using var stream = new MemoryStream();
        //            workbook.SaveAs(stream);
        //            var content = stream.ToArray();
        //            Response.Clear();
        //            Response.Headers.Add("content-disposition", $"attachment;filename=ProductDetails{DateTime.UtcNow}.xlsx");
        //            Response.ContentType = "application/xlsx";
        //            Response.Body.WriteAsync(content);
        //            Response.Body.Flush();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex, $"Error in ExportToExcel(), Download");
        //        throw;
        //    }
        //}

        /// <summary>
        /// ExportToExcel
        /// </summary>
        /// <param name="products"></param>
        private void ExportToExcel(DataTable products)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Products");
                    var currentRow = 1;
                    var currentHeaderColumn = 1;
                    var currentColumn = 1;
                    foreach (var p in typeof(ProductDownload).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        worksheet.Cell(currentRow, currentHeaderColumn).Value = p.Name;
                        currentHeaderColumn++;
                    }
                  
                    for (int i = 0; i < products.Rows.Count; i++)
                    {                       
                        currentRow++;
                        currentColumn = 1;
                        foreach (var p in typeof(ProductDownload).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                        {
                            worksheet.Cell(currentRow, currentColumn).Value = products.Rows[i][p.Name];
                            currentColumn++;
                        }                           
                    }
                    using var stream = new MemoryStream();
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    Response.Clear();
                    Response.Headers.Add("content-disposition", $"attachment;filename=ProductDetails{DateTime.UtcNow}.xlsx");
                    Response.ContentType = "application/xlsx";
                    Response.Body.WriteAsync(content);
                    Response.Body.Flush();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in ExportToExcel(), Download");
                throw;
            }
        }
    }
}
