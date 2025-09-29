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
    public class BaseCostModel : PageModel
    {
        public BaseCostModel(IGeneralRepository<BaseCost> baseCost, IGeneralRepository<Product> product,
            IGeneralRepository<Supplier> supplier, IUploadDownloadDA uploadDownloadDA, IMapper mapper)
        {
            _baseCost = baseCost;
            _product = product;
            _supplier = supplier;
            _uploadDownloadDA = uploadDownloadDA;
            _mapper = mapper;
        }

        Dictionary<string, bool> uploadFileFields = new Dictionary<string, bool>();
        private readonly IGeneralRepository<BaseCost> _baseCost;
        private readonly IGeneralRepository<Product> _product;
        private readonly IGeneralRepository<Supplier> _supplier;
        private readonly IUploadDownloadDA _uploadDownloadDA;
        private readonly IMapper _mapper;

        [BindProperty()]
        public BaseCost objBaseCost { get; set; }

        [BindProperty]
        public IFormFile Upload { get; set; }

        public List<BaseCost> lstBaseCost = new List<BaseCost>();
        public List<Product> products { get; set; }
        public Pagination PaginationModel { get; set; } = new Pagination();

        public SelectList selectProduct { get; set; }
        public SelectList selectSupplier { get; set; }

        /// <summary>
        /// OnGetAsync
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                //if (objBaseCost == null)
                //{
                //    objBaseCost = new BaseCost()
                //    {
                //        IsActive = true // Default to active
                //    };
                //}
                if (objBaseCost == null)
                {
                    objBaseCost = new BaseCost();
                }
                ViewData["PageType"] = "List";
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString(Constants.StatusMessage)))
                {
                    ViewData["SuccessMsg"] = HttpContext.Session.GetString(Constants.StatusMessage);
                    HttpContext.Session.SetString(Constants.StatusMessage, "");
                }
                ViewData["PagePrimaryID"] = 0;

                await LoadProduct();
                await LoadSupplier();
                ViewData["PageType"] = "List";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetAsync(), BaseCostModel");
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
                if (objBaseCost == null)
                {
                    objBaseCost = new BaseCost();
                }
                objBaseCost.IsActive = true;
                ViewData["PageType"] = "Edit";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetCreateAsync(), BaseCostModel");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnGetEditAsync
        /// </summary>
        /// <param name="baseCostId"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetEditAsync(int baseCostId)
        {
            try
            {
                objBaseCost = await _baseCost.GetAsync(filter: x => x.BaseCostId == baseCostId);

                if (objBaseCost == null)
                {
                    return Redirect("~/all-base-cost");
                }
                await LoadProduct();
                await LoadSupplier();
                ViewData["PageType"] = "Edit";
                ViewData["PagePrimaryID"] = objBaseCost.BaseCostId;

            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetEditAsync(), BaseCostModel, baseCostId: { baseCostId }");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnPostSubmitAsync
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostSubmitAsync()
        {
            try
            {
                BaseCost existingBaseCost = await _baseCost.GetAsync(filter: x => x.ProductId == objBaseCost.ProductId && x.SupplierId == objBaseCost.SupplierId);
                if (existingBaseCost != null)
                {
                    if ((objBaseCost.BaseCostId == 0) || (objBaseCost.BaseCostId != 0 && objBaseCost.BaseCostId != existingBaseCost.BaseCostId))
                    {
                        ViewData["PageType"] = "Edit";
                        if (objBaseCost.BaseCostId != 0)
                        {
                            ViewData["PagePrimaryID"] = objBaseCost.BaseCostId;
                        }
                        ViewData["SuccessMsg"] = $"Mapping of this Supplier and Product {Constants.AlreadyExistMessage}";
                        await LoadProduct();
                        await LoadSupplier();
                        return Page();
                    }

                }

                if (ModelState.IsValid)
                {
                    if (objBaseCost.BaseCostId == 0)
                    {
                        // Ensure default values are set for new records
                        objBaseCost.CreatedDate = DateTime.Now;

                        await _baseCost.InsertAsync(objBaseCost);
                        HttpContext.Session.SetString(Constants.StatusMessage, Constants.SaveMessage);
                    }
                    else
                    {

                        await _baseCost.UpdateAsync(objBaseCost);
                        HttpContext.Session.SetString(Constants.StatusMessage, Constants.UpdateMessage);
                    }
                    return Redirect("all-base-cost");
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
                Log.Error(ex, $"Error in OnPostSubmitAsync(), BaseCostModel");
                ViewData["ValidationMsg"] = "An error occurred while processing your request.";
                ViewData["PageType"] = "Edit";
                
                try
                {
                    if (objBaseCost == null)
                    {
                        objBaseCost = new BaseCost();
                    }
                    await LoadProduct();
                    await LoadSupplier();
                    ViewData["PageType"] = "Edit";
                    if (objBaseCost?.BaseCostId > 0)
                    {
                        ViewData["PagePrimaryID"] = objBaseCost.BaseCostId;
                    }
                }
                catch (Exception loadEx)
                {
                    Log.Error(loadEx, $"Error loading dropdown data in catch block, BaseCostModel");
                }
                throw;
            }
        }

        /// <summary>
        /// OnPostSubmitUploadAsync - Optimized Excel import method
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostSubmitUploadAsync()
        {
            try
            {
                // Input validation
                if (Upload == null || Upload.Length == 0)
                {
                    ViewData["ValidationMsg"] = "Please select a file to upload.";
                    return Page();
                }

                var fileExtension = Path.GetExtension(Upload.FileName).ToLower();
                if (fileExtension != ".xlsx" && fileExtension != ".xls")
                {
                    ViewData["ValidationMsg"] = "Invalid file format. Please upload an Excel file (.xlsx or .xls).";
                    return Page();
                }

                const int MaxFileSize = 5 * 1024 * 1024; // 5MB
                if (Upload.Length > MaxFileSize)
                {
                    ViewData["ValidationMsg"] = "File size exceeds the maximum allowed limit of 5MB.";
                    return Page();
                }

                // Process Excel file
                var dataTable = await ProcessExcelFileAsync(Upload);
                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    ViewData["ValidationMsg"] = "No data found in the Excel file.";
                    return Page();
                }

                // Transform and validate data
                var transformedData = await TransformDataTableAsync(dataTable);
                if (!transformedData.IsValid)
                {
                    ViewData["ValidationMsg"] = transformedData.ErrorMessage;
                    return Page();
                }

                // Save to database
                var result = await SaveBaseCostDataAsync(transformedData.DataTable);
                
                ViewData["SuccessMsg"] = $"Successfully imported {result.SuccessCount} base cost records.";
                if (result.FailedCount > 0)
                {
                    ViewData["ValidationMsg"] = $"Failed to import {result.FailedCount} records: {string.Join(", ", result.FailedRows)}";
                }
                return RedirectToPage("./BaseCost");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in OnPostSubmitUploadAsync(), BaseCostModel");
                ViewData["ErrorMsg"] = $"Error importing data: {ex.Message}";
                return Page();
            }
        }

        /// <summary>
        /// Process Excel file and return DataTable
        /// </summary>
        private async Task<DataTable> ProcessExcelFileAsync(IFormFile file)
        {
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                
                // Register encoding provider for .NET Core
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration
                        {
                            UseHeaderRow = true
                        }
                    });
                    
                    return result.Tables.Count > 0 ? result.Tables[0] : new DataTable();
                }
            }
        }

        /// <summary>
        /// Transform and validate DataTable
        /// </summary>
        private async Task<(bool IsValid, string ErrorMessage, DataTable DataTable)> TransformDataTableAsync(DataTable originalTable)
        {
            try
            {
                // Column mapping
                var columnMappings = new Dictionary<string, string>
                {
                    { "ProductCode", "ProductCode" },
                    { "SupplierName", "SupplierName" },
                    { "Supplier", "SupplierName" }, // Alternative column name
                    { "BaseCost", "BaseCost" },
                    { "StartDate", "StartDate" },
                    { "EndDate", "EndDate" },
                    { "Remark", "Remark" }
                };

                // Check required columns
                var requiredColumns = new[] { "ProductCode", "SupplierName", "BaseCost", "StartDate" };
                var availableColumns = originalTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
                
                // Find missing required columns
                var missingColumns = new List<string>();
                foreach (var required in requiredColumns)
                {
                    bool found = false;
                    foreach (var mapping in columnMappings)
                    {
                        if (mapping.Value == required && availableColumns.Contains(mapping.Key))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        missingColumns.Add(required);
                    }
                }

                if (missingColumns.Any())
                {
                    return (false, $"Missing required columns: {string.Join(", ", missingColumns)}", null);
                }

                // Create new DataTable with standardized column names
                var transformedTable = new DataTable();
                transformedTable.Columns.Add("ProductCode", typeof(string));
                transformedTable.Columns.Add("SupplierName", typeof(string));
                transformedTable.Columns.Add("BaseCost", typeof(decimal));
                transformedTable.Columns.Add("StartDate", typeof(DateTime));
                transformedTable.Columns.Add("EndDate", typeof(DateTime));
                transformedTable.Columns.Add("Remark", typeof(string));

                var errors = new List<string>();
                int rowIndex = 2; // Start from 2 (1-based + header row)

                foreach (DataRow row in originalTable.Rows)
                {
                    try
                    {
                        var newRow = transformedTable.NewRow();
                        var rowErrors = new List<string>();

                        // ProductCode
                        var productCode = GetColumnValue(row, originalTable, "ProductCode", columnMappings)?.ToString()?.Trim();
                        if (string.IsNullOrWhiteSpace(productCode))
                        {
                            rowErrors.Add("ProductCode is required");
                        }
                        newRow["ProductCode"] = productCode ?? "";

                        // SupplierName
                        var supplierName = GetColumnValue(row, originalTable, "SupplierName", columnMappings)?.ToString()?.Trim();
                        if (string.IsNullOrWhiteSpace(supplierName))
                        {
                            rowErrors.Add("SupplierName is required");
                        }
                        newRow["SupplierName"] = supplierName ?? "";

                        // BaseCost
                        decimal baseCost = 0;
                        if (!decimal.TryParse(row["BaseCost"]?.ToString(), out baseCost) || baseCost <= 0)
                        {
                            rowErrors.Add("Base Cost must be a positive number");
                        }
                        newRow["BaseCost"] = baseCost;

                        // StartDate
                        DateTime startDate = DateTime.MinValue;
                        if (!DateTime.TryParse(row["StartDate"]?.ToString(), out startDate))
                        {
                            rowErrors.Add("Invalid Start Date format");
                        }
                        newRow["StartDate"] = startDate;

                        // EndDate (optional)
                        DateTime? endDate = null;
                        var endDateValue = GetColumnValue(row, originalTable, "EndDate", columnMappings);
                        if (endDateValue != null && !string.IsNullOrWhiteSpace(endDateValue.ToString()))
                        {
                            if (DateTime.TryParse(endDateValue.ToString(), out DateTime parsedEndDate))
                            {
                                if (parsedEndDate <= startDate)
                                {
                                    rowErrors.Add("EndDate must be after StartDate");
                                }
                                else
                                {
                                    endDate = parsedEndDate;
                                }
                            }
                            else
                            {
                                rowErrors.Add("EndDate must be a valid date");
                            }
                        }
                        newRow["EndDate"] = endDate ?? (object)DBNull.Value;

                        // Remark (optional)
                        var remark = GetColumnValue(row, originalTable, "Remark", columnMappings)?.ToString()?.Trim();
                        newRow["Remark"] = remark ?? "";

                        if (rowErrors.Any())
                        {
                            errors.Add($"Row {rowIndex}: {string.Join(", ", rowErrors)}");
                        }
                        else
                        {
                            transformedTable.Rows.Add(newRow);
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Row {rowIndex}: Error processing row - {ex.Message}");
                    }

                    rowIndex++;

                    // Limit error reporting
                    if (errors.Count >= 10)
                    {
                        errors.Add("... and more errors not shown");
                        break;
                    }
                }

                if (errors.Any())
                {
                    return (false, "Validation failed:\n" + string.Join("\n", errors), null);
                }

                return (true, "", transformedTable);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in TransformDataTableAsync");
                return (false, $"Error processing data: {ex.Message}", null);
            }
        }

        /// <summary>
        /// Get column value with mapping support
        /// </summary>
        private object GetColumnValue(DataRow row, DataTable table, string targetColumn, Dictionary<string, string> mappings)
        {
            foreach (var mapping in mappings.Where(m => m.Value == targetColumn))
            {
                if (table.Columns.Contains(mapping.Key))
                {
                    return row[mapping.Key];
                }
            }
            return null;
        }

        /// <summary>
        /// Save base cost data to database
        /// </summary>
        private async Task<(int SuccessCount, int FailedCount, List<string> FailedRows)> SaveBaseCostDataAsync(DataTable dataTable)
        {
            var successCount = 0;
            var failedCount = 0;
            var failedRows = new List<string>();
            var currentUserSession = HttpContext.Session.GetString(Constants.LoginSession);
            var userId = 0;
            if (!string.IsNullOrEmpty(currentUserSession))
            {
                var currentUser = JsonConvert.DeserializeObject<Admin>(currentUserSession);
                userId = currentUser?.Id ?? 0;
            }

            // Get all products and suppliers for validation
            var allProducts = await _product.GetAllAsync(x => x.IsActive == true && (x.IsDelete == null || x.IsDelete != true));
            var allSuppliers = await _supplier.GetAllAsync(x => x.IsActive == true && (x.IsDelete == null || x.IsDelete != true));

            // Create lookups with null checks to prevent exceptions
            Dictionary<string, Product> productLookup;
            Dictionary<string, Supplier> supplierLookup;
            
            try
            {
                // Handle duplicate product codes by taking the first occurrence
                var productGroups = allProducts
                    .Where(p => !string.IsNullOrEmpty(p.Code))
                    .GroupBy(p => p.Code.ToUpper())
                    .ToList();
                
                // Log duplicate product codes for data cleanup
                var duplicateProducts = productGroups.Where(g => g.Count() > 1).ToList();
                if (duplicateProducts.Any())
                {
                    Log.Warning($"Found {duplicateProducts.Count} duplicate product codes: {string.Join(", ", duplicateProducts.Select(g => g.Key))}");
                }
                
                productLookup = productGroups.ToDictionary(g => g.Key, g => g.First());
                
                // Handle duplicate supplier names by taking the first occurrence
                var supplierGroups = allSuppliers
                    .Where(s => !string.IsNullOrEmpty(s.Name))
                    .GroupBy(s => s.Name.ToUpper())
                    .ToList();
                
                // Log duplicate supplier names for data cleanup
                var duplicateSuppliers = supplierGroups.Where(g => g.Count() > 1).ToList();
                if (duplicateSuppliers.Any())
                {
                    Log.Warning($"Found {duplicateSuppliers.Count} duplicate supplier names: {string.Join(", ", duplicateSuppliers.Select(g => g.Key))}");
                }
                
                supplierLookup = supplierGroups.ToDictionary(g => g.Key, g => g.First());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating lookup dictionaries in SaveBaseCostDataAsync");
                return (0, 0, new List<string> { $"Error creating lookup dictionaries: {ex.Message}" });
            }

            Log.Information($"SaveBaseCostDataAsync: Found {productLookup.Count} products and {supplierLookup.Count} suppliers for validation");
            Log.Information($"SaveBaseCostDataAsync: Processing {dataTable.Rows.Count} rows from Excel");

            foreach (DataRow row in dataTable.Rows)
            {
                try
                {
                    var productCode = row["ProductCode"].ToString().ToUpper();
                    var supplierName = row["SupplierName"].ToString().ToUpper();

                    // Validate product exists
                    if (!productLookup.TryGetValue(productCode, out Product product))
                    {
                        failedCount++;
                        failedRows.Add($"Row {dataTable.Rows.IndexOf(row) + 2}: Product with code '{row["ProductCode"]}' not found");
                        continue;
                    }

                    // Validate supplier exists
                    if (!supplierLookup.TryGetValue(supplierName, out Supplier supplier))
                    {
                        failedCount++;
                        failedRows.Add($"Row {dataTable.Rows.IndexOf(row) + 2}: Supplier '{row["SupplierName"]}' not found");
                        continue;
                    }

                    var startDate = (DateTime)row["StartDate"];
                    var endDate = row["EndDate"] == DBNull.Value ? (DateTime?)null : (DateTime)row["EndDate"];

                    // Check for overlapping date ranges
                    var existingBaseCosts = await _baseCost.GetAllAsync(x => 
                        x.ProductId == product.Id && 
                        x.SupplierId == supplier.Id && 
                        (x.IsDelete == null || x.IsDelete != true));

                    bool hasOverlap = false;
                    foreach (var existing in existingBaseCosts)
                    {
                        if (endDate == null && existing.EndDate == null)
                        {
                            hasOverlap = true;
                        }
                        else if (endDate == null)
                        {
                            hasOverlap = startDate <= existing.EndDate;
                        }
                        else if (existing.EndDate == null)
                        {
                            hasOverlap = existing.StartDate <= endDate;
                        }
                        else
                        {
                            hasOverlap = startDate <= existing.EndDate && existing.StartDate <= endDate;
                        }

                        if (hasOverlap)
                        {
                            break;
                        }
                    }

                    if (hasOverlap)
                    {
                        failedCount++;
                        failedRows.Add($"Row {dataTable.Rows.IndexOf(row) + 2}: Date range overlaps with existing base cost for this product and supplier");
                        continue;
                    }

                    // Create new base cost
                    var baseCost = new BaseCost
                    {
                        ProductId = product.Id,
                        SupplierId = supplier.Id,
                        BaseCost1 = (decimal)row["BaseCost"],
                        StartDate = startDate,
                        EndDate = endDate,
                        Remark = row["Remark"].ToString(),
                        IsActive = true,
                        IsDelete = false,
                        CreatedDate = DateTime.Now,
                        CreatedBy = userId
                    };

                    await _baseCost.InsertAsync(baseCost);
                    successCount++;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Error saving base cost row {dataTable.Rows.IndexOf(row) + 2}");
                    failedCount++;
                    failedRows.Add($"Row {dataTable.Rows.IndexOf(row) + 2}: {ex.Message}");
                }
            }

            return (successCount, failedCount, failedRows);
        }

        /// <summary>
        /// OnGetDeleteAsync
        /// </summary>
        /// <param name="baseCostId"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetDeleteAsync(int baseCostId)
        {
            try
            {
                var baseCost = await _baseCost.GetAsync(filter: x => x.BaseCostId == baseCostId);
                if (baseCost != null)
                {
                    await _baseCost.LogicalDeleteAsync(baseCost);
                    ViewData["SuccessMsg"] = $"{Constants.DeletedMessage}";
                    HttpContext.Session.SetString(Constants.StatusMessage, $"{Constants.DeletedMessage}");
                }

                ViewData["PageType"] = "List";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetDeleteAsync(), BaseCostModel, baseCostId: { baseCostId }");
                throw;
            }
            return Redirect("~/all-base-cost");
        }

        /// <summary>
        /// OnGetClear
        /// </summary>
        /// <returns></returns>
        public IActionResult OnGetClear()
        {
            try
            {
                objBaseCost = new BaseCost();
                objBaseCost.IsActive = false;
                ViewData["PageType"] = "Edit";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetClear(), BaseCostModel");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnGetReset
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetReset(int baseCostId)
        {
            try
            {
                await LoadProduct();
                await LoadSupplier();
                objBaseCost = await _baseCost.GetAsync(filter: x => x.BaseCostId == baseCostId);
                ViewData["PageType"] = "Edit";
                ViewData["PagePrimaryID"] = objBaseCost.BaseCostId;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetReset(), BaseCostModel, baseCostId: { baseCostId }");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// LoadProduct
        /// </summary>
        /// <returns></returns>
        private async Task LoadProduct()
        {
            try
            {
                var products = await _product.GetAllAsync(filter: x => x.IsDelete != true);
                var productList = products.Select(p => new {
                    Id = p.Id,
                    DisplayText = $"{p.ProductName} ({p.Code})"
                }).ToList();

                selectProduct = new SelectList(productList, "Id", "DisplayText", objBaseCost?.ProductId);
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
                                          "Id", "Name", objBaseCost?.SupplierId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in LoadSupplier(), BaseCost");
                throw;
            }
        }
    }
}
