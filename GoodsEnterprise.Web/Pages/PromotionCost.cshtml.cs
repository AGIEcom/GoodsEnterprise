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

                // Validate data size and warn user for large datasets
                if (processedData.Rows.Count > 10000)
                {
                    Log.Information($"Large dataset detected: {processedData.Rows.Count} rows. Processing may take several minutes.");
                }

                // Validate products and suppliers exist before importing
                var validationResult = await ValidateProductsAndSuppliersAsync(processedData);
                if (!validationResult.IsValid)
                {
                    ViewData["ValidationMsg"] = validationResult.ErrorMessage;
                    
                    // Ensure dropdown data is loaded for validation error scenario
                    try
                    {
                        if (objpromotionCost == null) objpromotionCost = new PromotionCost();
                        await LoadProduct();
                        await LoadSupplier();
                        ViewData["PageType"] = "List";
                    }
                    catch (Exception loadEx)
                    {
                        Log.Error(loadEx, "Error loading dropdown data in validation error scenario");
                    }
                    
                    return Page();
                }

                await SavePromotionCostDataAsync(processedData);

                var successMessage = $"Successfully uploaded {processedData.Rows.Count:N0} promotion cost records.";
                ViewData["SuccessMsg"] = successMessage;
                HttpContext.Session.SetString(Constants.StatusMessage, successMessage);
                
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
        /// Process Excel file and return cleaned DataTable optimized for large datasets
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
            
            // Log the actual Excel headers for debugging
            var actualHeaders = rawData.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray();
            Log.Information($"Excel headers found: {string.Join(", ", actualHeaders)}");
            
            // Create result table with UDT structure
            var resultTable = CreateUDTDataTable();
            
            // Process each row and map to UDT columns
            foreach (DataRow sourceRow in rawData.Rows)
            {
                // Skip empty rows
                if (sourceRow.ItemArray.All(field => field is DBNull || string.IsNullOrWhiteSpace(field?.ToString())))
                    continue;
                    
                var newRow = resultTable.NewRow();
                
                // Map Excel columns to UDT columns based on your actual Excel structure
                try
                {
                    // Map based on actual Excel column names from your screenshot
                    newRow["Product"] = GetExcelValue(sourceRow, rawData, "Product");
                    newRow["OuterBarcode"] = GetExcelValue(sourceRow, rawData, "Outer Barcode");
                    newRow["PromotionCost"] = ConvertToDecimal(GetExcelValue(sourceRow, rawData, "W/sale Nett Cost"));
                    newRow["StartDate"] = ConvertToDateTime(GetExcelValue(sourceRow, rawData, "Start"));
                    newRow["EndDate"] = ConvertToDateTime(GetExcelValue(sourceRow, rawData, "End"));
                    newRow["SelloutStartDate"] = ConvertToDateTime(GetExcelValue(sourceRow, rawData, "Sellout Start"));
                    newRow["SelloutEndDate"] = ConvertToDateTime(GetExcelValue(sourceRow, rawData, "Sellout End"));
                    newRow["BonusDescription"] = GetExcelValue(sourceRow, rawData, "Bonus Description");
                    newRow["SellOutDescription"] = GetExcelValue(sourceRow, rawData, "Sell Out Description");
                    newRow["Supplier"] = GetExcelValue(sourceRow, rawData, "Supplier:");
                    
                    // Log first row for debugging
                    if (resultTable.Rows.Count == 0)
                    {
                        Log.Information($"First row mapping - Product: '{newRow["Product"]}', OuterBarcode: '{newRow["OuterBarcode"]}', PromotionCost: '{newRow["PromotionCost"]}', Supplier: '{newRow["Supplier"]}'");
                    }
                    
                    resultTable.Rows.Add(newRow);
                }
                catch (Exception ex)
                {
                    Log.Warning($"Error processing row: {ex.Message}");
                    continue;
                }
            }

            Log.Information($"Excel processing completed. Total rows processed: {resultTable.Rows.Count}");
            return resultTable.Rows.Count > 0 ? resultTable : null;
        }

        /// <summary>
        /// Create DataTable with exact UDT structure
        /// </summary>
        private DataTable CreateUDTDataTable()
        {
            var table = new DataTable();
            
            // Create columns in exact UDT order
            table.Columns.Add("Product", typeof(string));
            table.Columns.Add("OuterBarcode", typeof(string));
            table.Columns.Add("PromotionCost", typeof(decimal));
            table.Columns.Add("StartDate", typeof(DateTime));
            table.Columns.Add("EndDate", typeof(DateTime));
            table.Columns.Add("SelloutStartDate", typeof(DateTime));
            table.Columns.Add("SelloutEndDate", typeof(DateTime));
            table.Columns.Add("BonusDescription", typeof(string));
            table.Columns.Add("SellOutDescription", typeof(string));
            table.Columns.Add("Supplier", typeof(string));
            
            return table;
        }

        /// <summary>
        /// Get value from Excel row by column name
        /// </summary>
        private object GetExcelValue(DataRow row, DataTable table, string columnName)
        {
            if (table.Columns.Contains(columnName))
            {
                var value = row[columnName];
                return value == DBNull.Value ? null : value;
            }
            return null;
        }

        /// <summary>
        /// Convert value to decimal safely
        /// </summary>
        private object ConvertToDecimal(object value)
        {
            if (value == null || value == DBNull.Value)
                return DBNull.Value;
                
            var stringValue = value.ToString().Trim();
            if (string.IsNullOrEmpty(stringValue))
                return DBNull.Value;
                
            if (decimal.TryParse(stringValue, out decimal result))
                return result;
                
            if (double.TryParse(stringValue, out double doubleResult))
                return Convert.ToDecimal(doubleResult);
                
            return DBNull.Value;
        }

        /// <summary>
        /// Convert value to DateTime safely
        /// </summary>
        private object ConvertToDateTime(object value)
        {
            if (value == null || value == DBNull.Value)
                return DBNull.Value;
                
            if (value is DateTime dateTime)
                return dateTime;
                
            var stringValue = value.ToString().Trim();
            if (string.IsNullOrEmpty(stringValue))
                return DBNull.Value;
                
            if (DateTime.TryParse(stringValue, out DateTime result))
                return result;
                
            // Handle Excel serial date numbers
            if (double.TryParse(stringValue, out double serialDate))
            {
                try
                {
                    return DateTime.FromOADate(serialDate);
                }
                catch
                {
                    return DBNull.Value;
                }
            }
                
            return DBNull.Value;
        }

        /// <summary>
        /// Create optimized DataTable with proper column types
        /// </summary>
        private DataTable CreateOptimizedDataTable(string[] headers)
        {
            var table = new DataTable();
            
            // Define expected columns with proper data types (must match UDTType_PromotionCost exactly)
            var columnDefinitions = new Dictionary<string, Type>
            {
                { "Product", typeof(string) },
                { "OuterBarcode", typeof(string) },
                { "PromotionCost", typeof(decimal) },
                { "StartDate", typeof(DateTime) },
                { "EndDate", typeof(DateTime) },
                { "SelloutStartDate", typeof(DateTime) },
                { "SelloutEndDate", typeof(DateTime) },
                { "BonusDescription", typeof(string) },
                { "SellOutDescription", typeof(string) }, // Fixed: was "SelloutDescription"
                { "Supplier", typeof(string) }
            };

            // Map Excel headers to our expected columns (based on actual Excel file structure)
            var headerMappings = new Dictionary<string, string>
            {
                { "Start", "StartDate" },
                { "End", "EndDate" },
                { "Sellout Start", "SelloutStartDate" },
                { "Sellout End", "SelloutEndDate" },
                { "Supplier:", "Supplier" },
                { "Bonus Description", "BonusDescription" },
                { "Sell Out Description", "SellOutDescription" },
                { "Outer Barcode", "OuterBarcode" },
                { "W/sale Nett Cost", "PromotionCost" },
                { "Product", "Product" },
                // Add missing mappings from actual Excel file
                { "Type", "Type" },
                { "PACK & RSP", "PackRSP" },
                { "Inner Barcode", "InnerBarcode" },
                { "Pallet", "Pallet" },
                { "Layer", "Layer" },
                { "VAT", "VAT" },
                { "COST", "Cost" },
                { "BONUS", "Bonus" },
                { "C&C Price", "CandCPrice" },
                { "Price & Deal in Promotion", "PriceDealPromotion" },
                { "Contact:", "Contact" },
                { "Telephone", "Telephone" },
                { "Email", "Email" },
                { "Comments", "Comments" }
            };

            // Create all UDT columns first (in correct order)
            foreach (var columnDef in columnDefinitions)
            {
                table.Columns.Add(columnDef.Key, columnDef.Value);
            }

            // Store header mapping for later use during data population
            table.ExtendedProperties["HeaderMappings"] = headerMappings;
            table.ExtendedProperties["OriginalHeaders"] = headers;

            return table;
        }

        /// <summary>
        /// Add batch of rows to DataTable efficiently with proper column mapping
        /// </summary>
        private void AddBatchToDataTable(DataTable table, List<object[]> batchRows)
        {
            var headerMappings = (Dictionary<string, string>)table.ExtendedProperties["HeaderMappings"];
            var originalHeaders = (string[])table.ExtendedProperties["OriginalHeaders"];

            foreach (var rowData in batchRows)
            {
                var newRow = table.NewRow();
                
                // Map Excel columns to DataTable columns using header mappings
                for (int excelColIndex = 0; excelColIndex < Math.Min(rowData.Length, originalHeaders.Length); excelColIndex++)
                {
                    try
                    {
                        var excelHeader = originalHeaders[excelColIndex];
                        var value = rowData[excelColIndex];
                        
                        // Skip if no mapping exists or value is null/empty
                        if (!headerMappings.ContainsKey(excelHeader) || value == null || value == DBNull.Value)
                            continue;
                            
                        var targetColumnName = headerMappings[excelHeader];
                        
                        // Skip if target column doesn't exist in our UDT table
                        if (!table.Columns.Contains(targetColumnName))
                            continue;
                        
                        var columnType = table.Columns[targetColumnName].DataType;
                        var convertedValue = ConvertValue(value, columnType);
                        
                        if (convertedValue != DBNull.Value)
                        {
                            newRow[targetColumnName] = convertedValue;
                        }
                    }
                    catch (Exception ex)
                    {
                        var excelHeader = excelColIndex < originalHeaders.Length ? originalHeaders[excelColIndex] : $"Column{excelColIndex}";
                        Log.Warning($"Error converting value at Excel column '{excelHeader}' (index {excelColIndex}): {ex.Message}");
                    }
                }
                
                table.Rows.Add(newRow);
            }
        }

        /// <summary>
        /// Convert value to target type safely with enhanced handling for Excel data
        /// </summary>
        private object ConvertValue(object value, Type targetType)
        {
            if (value == null || value == DBNull.Value)
                return DBNull.Value;

            var stringValue = value.ToString().Trim();
            if (string.IsNullOrEmpty(stringValue))
                return DBNull.Value;

            try
            {
                if (targetType == typeof(decimal))
                {
                    // Handle various decimal formats
                    if (decimal.TryParse(stringValue, out decimal decimalResult))
                        return decimalResult;
                    
                    // Try parsing as double first (Excel sometimes returns doubles)
                    if (double.TryParse(stringValue, out double doubleResult))
                        return Convert.ToDecimal(doubleResult);
                        
                    return DBNull.Value;
                }
                else if (targetType == typeof(DateTime))
                {
                    // Handle Excel date formats
                    if (DateTime.TryParse(stringValue, out DateTime dateResult))
                        return dateResult;
                    
                    // Handle Excel serial date numbers
                    if (double.TryParse(stringValue, out double serialDate))
                    {
                        try
                        {
                            return DateTime.FromOADate(serialDate);
                        }
                        catch
                        {
                            return DBNull.Value;
                        }
                    }
                    
                    return DBNull.Value;
                }
                else if (targetType == typeof(string))
                {
                    return stringValue;
                }
                
                return Convert.ChangeType(value, targetType);
            }
            catch (Exception ex)
            {
                Log.Warning($"Failed to convert value '{stringValue}' to type {targetType.Name}: {ex.Message}");
                return DBNull.Value;
            }
        }


        /// <summary>
        /// Save processed promotion cost data to database with batch processing for large datasets
        /// </summary>
        private async Task SavePromotionCostDataAsync(DataTable processedData)
        {
            const int DB_BATCH_SIZE = 5000; // Optimal batch size for database operations
            
            var currentUserSession = HttpContext.Session.GetString(Constants.LoginSession);
            if (string.IsNullOrEmpty(currentUserSession))
            {
                throw new UnauthorizedAccessException("User session not found. Please login again.");
            }

            var currentUser = JsonConvert.DeserializeObject<Admin>(currentUserSession);
            var totalRows = processedData.Rows.Count;
            
            Log.Information($"Starting database insertion for {totalRows} records in batches of {DB_BATCH_SIZE}");

            // Process in batches to avoid timeout and memory issues
            for (int i = 0; i < totalRows; i += DB_BATCH_SIZE)
            {
                var batchSize = Math.Min(DB_BATCH_SIZE, totalRows - i);
                var batchData = CreateBatchDataTable(processedData, i, batchSize);
                // No need to reorder since data is already in correct UDT format

                var parameters = new CommenParameters
                {
                    SPName = "usp_INSERTPROMOTIONCOST",
                    CreatedBy = currentUser.Id
                };

                try
                {
                    // Log batch details for debugging
                    Log.Information($"Inserting batch {(i / DB_BATCH_SIZE) + 1} with {batchSize} rows. Columns: {string.Join(", ", batchData.Columns.Cast<DataColumn>().Select(c => c.ColumnName))}");
                    
                    // Log sample data from first row for debugging
                    if (batchData.Rows.Count > 0)
                    {
                        var firstRow = batchData.Rows[0];
                        Log.Information($"Sample row data - Product: '{firstRow["Product"]}', OuterBarcode: '{firstRow["OuterBarcode"]}', PromotionCost: '{firstRow["PromotionCost"]}', StartDate: '{firstRow["StartDate"]}', EndDate: '{firstRow["EndDate"]}', Supplier: '{firstRow["Supplier"]}'");
                        
                        // Log all non-null values for debugging
                        var nonNullValues = new List<string>();
                        foreach (DataColumn col in batchData.Columns)
                        {
                            var value = firstRow[col.ColumnName];
                            if (value != null && value != DBNull.Value && !string.IsNullOrEmpty(value.ToString()))
                            {
                                nonNullValues.Add($"{col.ColumnName}='{value}'");
                            }
                        }
                        Log.Information($"All non-null values: {string.Join(", ", nonNullValues)}");
                    }
                    
                    // Validate DataTable structure before sending to SP
                    ValidateDataTableStructure(batchData);
                    
                    await _promotionCost.PostValueUsingUDTT(batchData, parameters);
                    Log.Information($"Successfully inserted batch {(i / DB_BATCH_SIZE) + 1}: rows {i + 1} to {i + batchSize}");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Error inserting batch {(i / DB_BATCH_SIZE) + 1}: rows {i + 1} to {i + batchSize}. Batch columns: {string.Join(", ", batchData.Columns.Cast<DataColumn>().Select(c => c.ColumnName))}");
                    
                    // Log detailed error information
                    if (batchData.Rows.Count > 0)
                    {
                        var firstRow = batchData.Rows[0];
                        Log.Error($"Failed batch sample data - Product: '{firstRow["Product"]}', OuterBarcode: '{firstRow["OuterBarcode"]}', PromotionCost: '{firstRow["PromotionCost"]}', Supplier: '{firstRow["Supplier"]}'");
                    }
                    
                    throw new Exception($"Database insertion failed at batch {(i / DB_BATCH_SIZE) + 1}. Error: {ex.Message}", ex);
                }

                // Add small delay between batches to prevent overwhelming the database
                if (i + DB_BATCH_SIZE < totalRows)
                {
                    await Task.Delay(100); // 100ms delay between batches
                }
            }

            Log.Information($"Database insertion completed successfully. Total records inserted: {totalRows}");
        }

        /// <summary>
        /// Create a batch DataTable from the main DataTable
        /// </summary>
        private DataTable CreateBatchDataTable(DataTable source, int startIndex, int batchSize)
        {
            var batchTable = source.Clone();
            
            for (int i = startIndex; i < startIndex + batchSize && i < source.Rows.Count; i++)
            {
                batchTable.ImportRow(source.Rows[i]);
            }
            
            return batchTable;
        }

        /// <summary>
        /// Validate DataTable structure matches UDT exactly
        /// </summary>
        private void ValidateDataTableStructure(DataTable table)
        {
            var expectedColumns = new[]
            {
                "Product", "OuterBarcode", "PromotionCost", "StartDate", "EndDate",
                "SelloutStartDate", "SelloutEndDate", "BonusDescription", "SellOutDescription", "Supplier"
            };

            var actualColumns = table.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray();
            
            Log.Information($"Expected UDT columns: {string.Join(", ", expectedColumns)}");
            Log.Information($"Actual DataTable columns: {string.Join(", ", actualColumns)}");

            // Check for missing columns
            var missingColumns = expectedColumns.Except(actualColumns).ToArray();
            if (missingColumns.Any())
            {
                Log.Error($"Missing columns in DataTable: {string.Join(", ", missingColumns)}");
                throw new Exception($"DataTable is missing required columns: {string.Join(", ", missingColumns)}");
            }

            // Check for extra columns (not critical but good to know)
            var extraColumns = actualColumns.Except(expectedColumns).ToArray();
            if (extraColumns.Any())
            {
                Log.Warning($"Extra columns in DataTable (will be ignored): {string.Join(", ", extraColumns)}");
            }

            // Validate data types
            var columnTypes = new Dictionary<string, Type>
            {
                { "Product", typeof(string) },
                { "OuterBarcode", typeof(string) },
                { "PromotionCost", typeof(decimal) },
                { "StartDate", typeof(DateTime) },
                { "EndDate", typeof(DateTime) },
                { "SelloutStartDate", typeof(DateTime) },
                { "SelloutEndDate", typeof(DateTime) },
                { "BonusDescription", typeof(string) },
                { "SellOutDescription", typeof(string) },
                { "Supplier", typeof(string) }
            };

            foreach (var expectedColumn in expectedColumns)
            {
                if (table.Columns.Contains(expectedColumn))
                {
                    var actualType = table.Columns[expectedColumn].DataType;
                    var expectedType = columnTypes[expectedColumn];
                    
                    if (actualType != expectedType)
                    {
                        Log.Warning($"Column '{expectedColumn}' has type {actualType.Name}, expected {expectedType.Name}");
                    }
                }
            }

            Log.Information($"DataTable structure validation completed. Rows: {table.Rows.Count}");
        }

        /// <summary>
        /// Validation result for products and suppliers
        /// </summary>
        public class ValidationResult
        {
            public bool IsValid { get; set; }
            public string ErrorMessage { get; set; }
            public List<string> MissingProducts { get; set; } = new List<string>();
            public List<string> MissingSuppliers { get; set; } = new List<string>();
        }

        /// <summary>
        /// Validate that all products and suppliers in the Excel file exist in the database
        /// </summary>
        private async Task<ValidationResult> ValidateProductsAndSuppliersAsync(DataTable data)
        {
            var result = new ValidationResult
            {
                IsValid = true,
                MissingProducts = new List<string>(),
                MissingSuppliers = new List<string>()
            };

            try
            {
                // Get all existing products and suppliers from database
                var existingProducts = (await _product.GetAllAsync(filter: x => x.IsDelete != true)).ToList();
                var existingSuppliers = (await _supplier.GetAllAsync(filter: x => x.IsDelete != true)).ToList();

                // Create case-insensitive hash sets for lookup
                var productNames = new HashSet<string>(
                    existingProducts
                        .Where(p => !string.IsNullOrWhiteSpace(p.ProductName))
                        .Select(p => p.ProductName.Trim()),
                    StringComparer.OrdinalIgnoreCase);

                var supplierNames = new HashSet<string>(
                    existingSuppliers
                        .Where(s => !string.IsNullOrWhiteSpace(s.Name))
                        .Select(s => s.Name.Trim()),
                    StringComparer.OrdinalIgnoreCase);

                // Track missing items
                var missingProducts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var missingSuppliers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                // Check each row in the Excel data
                foreach (DataRow row in data.Rows)
                {
                    var supplierName = (row["Supplier"]?.ToString() ?? "").Trim();
                    var productName = (row["Product"]?.ToString() ?? "").Trim();

                    // Skip empty rows
                    if (string.IsNullOrWhiteSpace(productName) && string.IsNullOrWhiteSpace(supplierName))
                        continue;

                    // Check if product exists by name (case-insensitive)
                    if (!string.IsNullOrWhiteSpace(productName) &&
                        !productNames.Contains(productName))
                    {
                        missingProducts.Add(productName);
                    }

                    // Check if supplier exists by name (case-insensitive)
                    if (!string.IsNullOrWhiteSpace(supplierName) &&
                        !supplierNames.Contains(supplierName))
                    {
                        missingSuppliers.Add(supplierName);
                    }
                }

                // Rest of the method remains the same...
                if (missingProducts.Any() || missingSuppliers.Any())
                {
                    result.IsValid = false;
                    result.MissingProducts = missingProducts.Take(20).ToList();
                    result.MissingSuppliers = missingSuppliers.Take(20).ToList();

                    var errorMessage = "Cannot import data. The following items were not found in the database:\n\n";

                    if (missingProducts.Any())
                    {
                        errorMessage += $"Missing Products ({missingProducts.Count}):\n";
                        errorMessage += string.Join("\n", result.MissingProducts.Select(p => $"• {p}"));

                        if (missingProducts.Count > 20)
                        {
                            errorMessage += $"\n... and {missingProducts.Count - 20} more products";
                        }
                        errorMessage += "\n\n";
                    }

                    if (missingSuppliers.Any())
                    {
                        errorMessage += $"Missing Suppliers ({missingSuppliers.Count}):\n";
                        errorMessage += string.Join("\n", result.MissingSuppliers.Select(s => $"• {s}"));

                        if (missingSuppliers.Count > 20)
                        {
                            errorMessage += $"\n... and {missingSuppliers.Count - 20} more suppliers";
                        }
                        errorMessage += "\n\n";
                    }

                    errorMessage += "Please ensure all products and suppliers exist in the system before importing.";
                    errorMessage += "\n\nNote: Products are matched by 'Product' field in your Excel file (case-insensitive).";
                    errorMessage += "\nNote: Suppliers are matched by 'Supplier' field in your Excel file (case-insensitive).";

                    result.ErrorMessage = errorMessage;

                    Log.Warning($"Validation failed: {missingProducts.Count} missing products, {missingSuppliers.Count} missing suppliers");
                }
                else
                {
                    Log.Information($"Validation passed: All {data.Rows.Count} rows have valid products and suppliers");
                }

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during product/supplier validation");
                result.IsValid = false;
                result.ErrorMessage = $"Error validating data: {ex.Message}";
                return result;
            }
        }

        /// <summary>
        /// Get available products and suppliers for reference (useful for debugging)
        /// </summary>
        private async Task<string> GetAvailableProductsAndSuppliersAsync()
        {
            try
            {
                var products = await _product.GetAllAsync(filter: x => x.IsDelete != true);
                var suppliers = await _supplier.GetAllAsync(filter: x => x.IsDelete != true);
                
                var message = "Available Products (first 10):\n";
                message += string.Join("\n", products.Take(10).Select(p => $"• {p.ProductName} (Barcode: {p.OuterEan})"));
                
                if (products.Count() > 10)
                {
                    message += $"\n... and {products.Count() - 10} more products";
                }
                
                message += "\n\nAvailable Suppliers (first 10):\n";
                message += string.Join("\n", suppliers.Take(10).Select(s => $"• {s.Name}"));
                
                if (suppliers.Count() > 10)
                {
                    message += $"\n... and {suppliers.Count() - 10} more suppliers";
                }
                
                return message;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting available products and suppliers");
                return "Error retrieving available products and suppliers.";
            }
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
        "SellOutDescription", // Fixed: was "SelloutDescription"
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
