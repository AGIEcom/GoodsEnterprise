using AutoMapper;
using ExcelDataReader;
using GoodsEnterprise.DataAccess.Interface;
using GoodsEnterprise.Model.Models;
using GoodsEnterprise.Web.Utilities;
using JqueryDataTables.ServerSide.AspNetCoreWeb.Models;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GoodsEnterprise.Web.Pages
{
    public class PromotionCostModel : PageModel
    {
        public PromotionCostModel(IGeneralRepository<PromotionCost> promotionCost, IGeneralRepository<Product> product,
            IGeneralRepository<Supplier> supplier, IUploadDownloadDA uploadDownloadDA, IMapper mapper)
        {
            _promotionCost = promotionCost;
            _product = product;
            _supplier = supplier;
            _uploadDownloadDA = uploadDownloadDA;
            _mapper = mapper;
        }
        Dictionary<string, bool> uploadFileFields = new Dictionary<string, bool>();
        private readonly IGeneralRepository<PromotionCost> _promotionCost;
        private readonly IGeneralRepository<Product> _product;
        private readonly IGeneralRepository<Supplier> _supplier;
        private readonly IUploadDownloadDA _uploadDownloadDA;
        private readonly IMapper _mapper;
        [BindProperty()]
        public PromotionCost objpromotionCost { get; set; }

        [BindProperty]
        public IFormFile Upload { get; set; }

        public List<PromotionCost> lstpromotionCost = new List<PromotionCost>();
        public List<Product> products { get; set; }
        public Pagination PaginationModel { get; set; } = new Pagination();

        public SelectList selectProduct { get; set; } = new SelectList("");
        public SelectList selectCategories { get; set; } = new SelectList("");
        public SelectList selectSupplier { get; set; } = new SelectList("");
        public SelectList selectTaxSlab { get; set; } = new SelectList("");

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
                await LoadProduct();
                await LoadSupplier();
                if (objpromotionCost == null)
                {
                    objpromotionCost = new PromotionCost();
                }
                objpromotionCost.IsActive = true;
                ViewData["IsTaxable"] = false;
                ViewData["PageType"] = "Edit";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetCreateAsync(), PromotionCostModel");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnGetEditAsync
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetEditAsync(int PromotionCostId)
        {
            try
            {
                objpromotionCost = await _promotionCost.GetAsync(filter: x => x.PromotionCostId == PromotionCostId);

                if (objpromotionCost == null)
                {
                    return Redirect("~/all-promotion-cost");
                }
                await LoadProduct();
                await LoadSupplier();
                ViewData["PageType"] = "Edit";
                ViewData["PagePrimaryID"] = objpromotionCost.PromotionCostId;

            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetEditAsync(), PromotionCostModel, PromotionCostId: { PromotionCostId }");
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
                objpromotionCost = new PromotionCost();
                objpromotionCost.IsActive = false;
                ViewData["PageType"] = "Edit";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetClear(), PromotionCostModel");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnResetClear
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetReset(int PromotionCostId)
        {
            try
            {
                await LoadProduct();
                await LoadSupplier();
                objpromotionCost = await _promotionCost.GetAsync(filter: x => x.PromotionCostId == PromotionCostId);
                ViewData["PageType"] = "Edit";
                ViewData["PagePrimaryID"] = objpromotionCost.PromotionCostId;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnResetClear(), PromotionCostModel, PromotionCostId: { objpromotionCost.PromotionCostId }");
                throw;
            }
            return Page();
        }
        /// <summary>
        /// OnGetDeleteAsync
        /// </summary>
        /// <param name="PromotionCostId"></param>
        /// <returns></returns>

        public async Task<IActionResult> OnGetDeleteAsync(int PromotionCostId)
        {
            try
            {
                var product = await _promotionCost.GetAsync(filter: x => x.PromotionCostId == PromotionCostId);
                if (product != null)
                {
                    await _promotionCost.LogicalDeleteAsync(product);
                    ViewData["SuccessMsg"] = $"{Constants.DeletedMessage}";
                    HttpContext.Session.SetString(Constants.StatusMessage, $"{Constants.DeletedMessage}");
                }

                ViewData["PageType"] = "List";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetDeleteAsync(), PromotionCostModel, PromotionCostId: { PromotionCostId }");
                throw;
            }
            return Redirect("~/all-promotion-cost");
        }

        /// <summary>
        /// OnPostSubmitAsync
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostSubmitAsync()
        {
            try
            {
                PromotionCost existingProduct = await _promotionCost.GetAsync(filter: x => x.ProductId == objpromotionCost.ProductId && x.SupplierId == objpromotionCost.SupplierId);
                if (existingProduct != null)
                {
                    if ((objpromotionCost.PromotionCostId == 0) || (objpromotionCost.PromotionCostId != 0 && objpromotionCost.PromotionCostId != existingProduct.PromotionCostId))
                    {
                        ViewData["PageType"] = "Edit";
                        if (objpromotionCost.PromotionCostId != 0)
                        {
                            ViewData["PagePrimaryID"] = objpromotionCost.PromotionCostId;
                        }
                        ViewData["SuccessMsg"] = $"Mapping of this Supplier and Product {Constants.AlreadyExistMessage}";
                        return Page();
                    }
                }
                if (ModelState.IsValid)
                {
                    if (objpromotionCost.PromotionCostId == 0)
                    {

                        await _promotionCost.InsertAsync(objpromotionCost);
                        HttpContext.Session.SetString(Constants.StatusMessage, Constants.SaveMessage);
                    }
                    else
                    {

                        await _promotionCost.UpdateAsync(objpromotionCost);
                        HttpContext.Session.SetString(Constants.StatusMessage, Constants.UpdateMessage);
                    }
                    return Redirect("all-product");
                }
                else
                {
                    ViewData["PageType"] = "Edit";
                    await LoadProduct();
                    await LoadSupplier();
                    return Page();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnPostUploadFileAsync(), PromotionCostModel, PromotionCostId: { objpromotionCost?.PromotionCostId}");
                throw;
            }
        }

        /// <summary>
        /// LoadProduct
        /// </summary>
        /// <returns></returns>


        private async Task LoadProduct()
        {
            try
            {
                selectProduct = new SelectList(await _product.GetAllAsync(filter: x => x.IsDelete != true),
                                          "Id", "ProductName", null);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in LoadProduct()");
                throw;
            }
        }

        /// <summary>
        /// LoadSupplier
        /// </summary>
        /// <returns></returns>
        private async Task LoadSupplier()
        {
            try
            {
                selectSupplier = new SelectList(await _supplier.GetAllAsync(filter: x => x.IsDelete != true),
                                          "Id", "Name", null);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in LoadSupplier(), PromotionCost");
                throw;
            }
        }


        public async Task<IActionResult> OnPostSubmitUploadAsync()
        {
            DataTable productUpload;
            try
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var stream = new MemoryStream())
                {
                    Upload.CopyTo(stream);
                    stream.Position = 0;
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        productUpload = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }
                        }).Tables[0];
                    }
                }
                productUpload = productUpload.Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is DBNull ||
                                     string.IsNullOrWhiteSpace(field as string))).CopyToDataTable();
                

                DataTable dtCloned = productUpload.Clone();
                dtCloned.Columns["Start"].DataType = typeof(DateTime);
                dtCloned.Columns["End"].DataType = typeof(DateTime);
                dtCloned.Columns["Sellout Start"].DataType = typeof(DateTime);
                dtCloned.Columns["Sellout End"].DataType = typeof(DateTime);
                dtCloned.Columns["Supplier:"].DataType = typeof(string);
                dtCloned.Columns["Bonus Description"].DataType = typeof(string);
                dtCloned.Columns["Sell Out Description"].DataType = typeof(string);
                dtCloned.Columns["Outer Barcode"].DataType = typeof(string);
                dtCloned.Columns["W/sale Nett Cost"].DataType = typeof(decimal); 
                foreach (DataRow row in productUpload.Rows)
                {
                    dtCloned.ImportRow(row);
                }

                dtCloned.Columns["Start"].ColumnName = "StartDate";
                dtCloned.Columns["End"].ColumnName = "EndDate";
                dtCloned.Columns["Sellout Start"].ColumnName = "SelloutStartDate";
                dtCloned.Columns["Sellout End"].ColumnName = "SelloutEndDate";
                dtCloned.Columns["Supplier:"].ColumnName = "Supplier";
                dtCloned.Columns["Bonus Description"].ColumnName = "BonusDescription";
                dtCloned.Columns["Sell Out Description"].ColumnName = "SellOutDescription";
                dtCloned.Columns["Outer Barcode"].ColumnName = "OuterBarcode";
                dtCloned.Columns["W/sale Nett Cost"].ColumnName = "PromotionCost";


                string[] ColumnsToBeDeleted = { "Type", "PACK & RSP", "Inner Barcode", "Pallet", "Layer", "VAT", "COST", "BONUS", "C&C Price", "Price & Deal in Promotion", "Contact:", "Telephone", "Email", "Comments", "Bonus Desc Value 1", "Bonus Desc Value 2", "Sellout Desc Value 1", "Sellout Desc Value 2" };

                foreach (string ColName in ColumnsToBeDeleted)
                {
                    if (dtCloned.Columns.Contains(ColName))
                        dtCloned.Columns.Remove(ColName);
                }


                CommenParameters commenParameters = new CommenParameters();
                commenParameters.SPName = "usp_INSERTPROMOTIONCOST";
                string CurrentUserIDSession = HttpContext.Session.GetString(Constants.LoginSession);
                var result = JsonConvert.DeserializeObject<Admin>(CurrentUserIDSession);
                commenParameters.CreatedBy = result.Id;
                await _promotionCost.PostValueUsingUDTT(dtCloned, commenParameters);

                //LoadFields();
                //foreach (DataColumn column in productUpload.Columns)
                //{
                //    if (uploadFileFields.ContainsKey(column.Caption.Trim()))
                //    {
                //        uploadFileFields[column.Caption.Trim()] = true;
                //    }
                //}


                //await LoadProductList();

                //if (products != null && products.Count > 0)
                //{
                //    //string[] existingProducts = productUpload.AsEnumerable().Where(a => products.Select(b => b.Code).Contains(Convert.ToString(a.Field<double>("Outer Barcode"))))
                //    //                          .GroupBy(product => product.Field<string>("Outer Barcode").Trim())
                //    //                          .Select(group => group.First().Field<string>("Outer Barcode")).ToArray();

                //    //if (existingProducts.Count() > 1)
                //    //{
                //    //    string messages = string.Empty;
                //    //    foreach (string msg in existingProducts)
                //    //    {
                //    //        messages += msg + "<br />";
                //    //    }
                //    //    ViewData["ValidationMsg"] = "Below Outer EAN are already exists, please correct it and retry" + "<br />" + messages;
                //    //    return Page();
                //    //}
                //}

                //await LoadSuppliers();

                //if (Common.UploadSuppliers != null && Common.UploadSuppliers.Count > 0)
                //{
                //    //string[] existingProducts = productUpload.AsEnumerable().Where(a => Common.UploadSuppliers.Select(b => b.Name).Contains(a.Field<string>("Supplier:")))
                //    //                          .GroupBy(product => product.Field<string>("Supplier:").Trim())
                //    //                          .Select(group => group.First().Field<string>("Supplier:")).ToArray();

                //    //if (existingProducts.Count() > 1)
                //    //{
                //    //    string messages = string.Empty;
                //    //    foreach (string msg in existingProducts)
                //    //    {
                //    //        messages += msg + "<br />";
                //    //    }
                //    //    ViewData["ValidationMsg"] = "Below Outer EAN are already exists, please correct it and retry" + "<br />" + messages;
                //    //    return Page();
                //    //}
                //}

                ////await InsertPromotionCost(productUpload); 


                //List<PromotionCost> bulkInsertProducts = _mapper.Map<List<DataRow>, List<PromotionCost>>(new List<DataRow>(productUpload.Rows.OfType<DataRow>()));

                //await _uploadDownloadDA.BulkInsertPromotionCost(bulkInsertProducts.ToList());

                ViewData["SuccessMsg"] = $"{Constants.SuccessUpload}";
                return Page();
            }
            catch (Exception ex)
            {
                ViewData["SuccessMsg"] = $"{Constants.FailureUpload}";
                Log.Error(ex, $"Error in OnPostSubmitAsync(), UploadDownload");
                return RedirectToPage("ErrorPage");
            }
            finally
            {
                Common.UploadBrands = null;
                Common.UploadCategories = null;
                Common.UploadSubCategories = null;
                Common.UploadSuppliers = null;
            }
        }
        public async Task LoadSuppliers()
        {
            try
            {
                Common.UploadSuppliers = await _supplier.GetAllAsync(filter: x => x.IsDelete != true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"LoadSuppliers(), UploadDownload");
                throw;
            }
        }
        public async Task LoadProductList()
        {
            try
            {
                products = await _product.GetAllAsync(filter: x => x.IsDelete != true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"LoadSuppliers(), UploadDownload");
                throw;
            }
        }
        private void LoadFields()
        {
            try
            {
                foreach (string field in Constants.PromotionCostFields)
                {
                    uploadFileFields.Add(field, false);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in LoadFields(), PromotionCostModel");
                throw;
            }
        }
        public async Task InsertPromotionCost(DataTable productUploadForSuppliers)
        {
            try
            {
                if (!productUploadForSuppliers.Columns.Contains("PromotionCost"))
                {
                    return;
                }
                var newSuppliers = productUploadForSuppliers.AsEnumerable()
                              .GroupBy(supplier => supplier.Field<string>("PromotionCost"))
                              .Select(group => group.First()).ToList();

                List<PromotionCost> bulkInsertSuppliers = new List<PromotionCost>();
                foreach (var supplier in newSuppliers)
                {
                    var bulkInsertSupplier = new PromotionCost()
                    {
                        ProductId = supplier.Field<int>("ProductID"),
                        PromotionCost1 = supplier.Field<decimal>("PromotionCost"),
                        StartDate = supplier.Field<DateTime>("StartDate"),
                        EndDate = supplier.Field<DateTime>("EndDate"),
                        SupplierId = supplier.Field<int>("SupplierId"),

                        CreatedDate = DateTime.UtcNow,
                        IsActive = true,
                    };
                    bulkInsertSuppliers.Add(bulkInsertSupplier);
                }

                await _uploadDownloadDA.BulkInsertPromotionCost(bulkInsertSuppliers);

                Common.UploadPromotionCosts.AddRange(bulkInsertSuppliers);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in InsertPromotionCost(), PromotionCostModel");
                throw;
            }
        }

    }
}
