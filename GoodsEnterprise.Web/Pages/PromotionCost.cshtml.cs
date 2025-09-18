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
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                if (objpromotionCost == null)
                {
                    objpromotionCost = new PromotionCost();
                }
                await LoadProduct();
                await LoadSupplier();
                ViewData["PageType"] = "List";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetAsync(), PromotionCostModel");
                throw;
            }
            return Page();
        }

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
                        await LoadProduct();
                        await LoadSupplier();
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
                    return Redirect("all-promotion-cost");
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
                Log.Error(ex, $"Error in OnPostSubmitAsync(), PromotionCostModel, PromotionCostId: { objpromotionCost?.PromotionCostId}");
                // Ensure dropdown data is loaded in case of error and we need to return to the page
                try
                {
                    if (objpromotionCost == null)
                    {
                        objpromotionCost = new PromotionCost();
                    }
                    await LoadProduct();
                    await LoadSupplier();
                    ViewData["PageType"] = "Edit";
                    if (objpromotionCost?.PromotionCostId > 0)
                    {
                        ViewData["PagePrimaryID"] = objpromotionCost.PromotionCostId;
                    }
                }
                catch (Exception loadEx)
                {
                    Log.Error(loadEx, $"Error loading dropdown data in catch block, PromotionCostModel");
                }
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
                                          "Id", "ProductName", objpromotionCost?.ProductId);
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
                                          "Id", "Name", objpromotionCost?.SupplierId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in LoadSupplier(), PromotionCost");
                throw;
            }
        }


        public async Task<IActionResult> OnPostSubmitUploadAsync()
        {
            if (Upload == null || Upload.Length == 0)
            {
                ViewData["ValidationMsg"] = "Please select a valid Excel file to upload.";
                return Page();
            }

            try
            {
                // Validate file extension
                var allowedExtensions = new[] { ".xlsx", ".xls" };
                var fileExtension = Path.GetExtension(Upload.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ViewData["ValidationMsg"] = "Only Excel files (.xlsx, .xls) are allowed.";
                    return Page();
                }

                // Register encoding provider once
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                DataTable processedData = await ProcessExcelFileAsync();
                
                if (processedData == null || processedData.Rows.Count == 0)
                {
                    ViewData["ValidationMsg"] = "No valid data found in the uploaded file.";
                    return Page();
                }

                await SavePromotionCostDataAsync(processedData);

                ViewData["SuccessMsg"] = Constants.SuccessUpload;
                HttpContext.Session.SetString(Constants.StatusMessage, Constants.SuccessUpload);
                
                return Redirect("all-promotion-cost");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnPostSubmitUploadAsync(), PromotionCostModel, File: {Upload?.FileName}");
                ViewData["ValidationMsg"] = $"Error processing file: {ex.Message}";
                
                // Ensure dropdown data is loaded for error scenario
                try
                {
                    if (objpromotionCost == null) objpromotionCost = new PromotionCost();
                    await LoadProduct();
                    await LoadSupplier();
                    ViewData["PageType"] = "List";
                }
                catch (Exception loadEx)
                {
                    Log.Error(loadEx, "Error loading dropdown data in upload error scenario");
                }
                
                return Page();
            }
            finally
            {
                // Clean up static references
                Common.UploadBrands = null;
                Common.UploadCategories = null;
                Common.UploadSubCategories = null;
                Common.UploadSuppliers = null;
            }
        }

        /// <summary>
        /// Process Excel file and return cleaned DataTable
        /// </summary>
        private async Task<DataTable> ProcessExcelFileAsync()
        {
            using var stream = new MemoryStream();
            await Upload.CopyToAsync(stream);
            stream.Position = 0;

            using var reader = ExcelReaderFactory.CreateReader(stream);
            var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true
                }
            });

            var rawData = dataSet.Tables[0];
            
            // Filter out empty rows
            var filteredRows = rawData.Rows.Cast<DataRow>()
                .Where(row => !row.ItemArray.All(field => field is DBNull || string.IsNullOrWhiteSpace(field?.ToString())))
                .ToList();

            if (!filteredRows.Any())
                return null;

            // Create filtered DataTable
            var filteredData = rawData.Clone();
            foreach (var row in filteredRows)
            {
                filteredData.ImportRow(row);
            }

            return TransformDataTable(filteredData);
        }

        /// <summary>
        /// Transform and clean the DataTable structure
        /// </summary>
        private DataTable TransformDataTable(DataTable source)
        {
            var transformed = source.Clone();

            // Define column mappings and data types
            var columnMappings = new Dictionary<string, (string newName, Type dataType)>
            {
                { "Start", ("StartDate", typeof(DateTime)) },
                { "End", ("EndDate", typeof(DateTime)) },
                { "Sellout Start", ("SelloutStartDate", typeof(DateTime)) },
                { "Sellout End", ("SelloutEndDate", typeof(DateTime)) },
                { "Supplier:", ("Supplier", typeof(string)) },
                { "Bonus Description", ("BonusDescription", typeof(string)) },
                { "Sell Out Description", ("SellOutDescription", typeof(string)) },
                { "Outer Barcode", ("OuterBarcode", typeof(string)) },
                { "W/sale Nett Cost", ("PromotionCost", typeof(decimal)) }
            };

            // Update column data types and names
            foreach (var mapping in columnMappings)
            {
                if (transformed.Columns.Contains(mapping.Key))
                {
                    transformed.Columns[mapping.Key].DataType = mapping.Value.dataType;
                    transformed.Columns[mapping.Key].ColumnName = mapping.Value.newName;
                }
            }

            // Copy data to transformed table
            foreach (DataRow row in source.Rows)
            {
                transformed.ImportRow(row);
            }

            // Remove unwanted columns
            var columnsToDelete = new[]
            {
                "Type", "PACK & RSP", "Inner Barcode", "Pallet", "Layer", "VAT", "COST", "BONUS",
                "C&C Price", "Price & Deal in Promotion", "Contact:", "Telephone", "Email", "Comments",
                "Bonus Desc Value 1", "Bonus Desc Value 2", "Sellout Desc Value 1", "Sellout Desc Value 2"
            };

            foreach (var columnName in columnsToDelete)
            {
                if (transformed.Columns.Contains(columnName))
                {
                    transformed.Columns.Remove(columnName);
                }
            }

            return transformed;
        }

        /// <summary>
        /// Save processed promotion cost data to database
        /// </summary>
        private async Task SavePromotionCostDataAsync(DataTable processedData)
        {
            var reorderedData = ReorderPromotionCostTable(processedData);

            var currentUserSession = HttpContext.Session.GetString(Constants.LoginSession);
            if (string.IsNullOrEmpty(currentUserSession))
            {
                throw new UnauthorizedAccessException("User session not found. Please login again.");
            }

            var currentUser = JsonConvert.DeserializeObject<Admin>(currentUserSession);
            
            var parameters = new CommenParameters
            {
                SPName = "usp_INSERTPROMOTIONCOST",
                CreatedBy = currentUser.Id
            };

            await _promotionCost.PostValueUsingUDTT(reorderedData, parameters);
        }
        /// <summary>
        /// ReorderPromotionCostTable
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static DataTable ReorderPromotionCostTable(DataTable src)
        {
            // Exact column order from your UDTType_PromotionCost
            string[] expectedOrder = new[]
            {
        "Product",
        "OuterBarcode",
        "PromotionCost",
        "StartDate",
        "EndDate",
        "SelloutStartDate",
        "SelloutEndDate",
        "BonusDescription",
        "SelloutDescription",
        "Supplier"
    };

            var dst = new DataTable();

            // Create new columns in correct order, preserving type
            foreach (var colName in expectedOrder)
            {
                if (!src.Columns.Contains(colName))
                    throw new Exception($"Source DataTable missing column: {colName}");

                dst.Columns.Add(colName, src.Columns[colName].DataType);
            }

            // Copy rows in the new column order
            foreach (DataRow r in src.Rows)
            {
                var newRow = dst.NewRow();
                foreach (var colName in expectedOrder)
                {
                    newRow[colName] = r[colName];
                }
                dst.Rows.Add(newRow);
            }

            return dst;
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
