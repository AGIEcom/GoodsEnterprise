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
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.Globalization;
using System.Text.Json;

namespace GoodsEnterprise.Web.Pages
{
    /// <summary>
    /// SupplierModel
    /// </summary>
    public class SupplierModel : PageModel
    {
        /// <summary>
        /// SupplierModel
        /// </summary>
        /// <param name="supplier"></param>
        public SupplierModel(IGeneralRepository<Supplier> supplier)
        {
            _supplier = supplier;
        }

        private readonly IGeneralRepository<Supplier> _supplier;

        [BindProperty()]
        public Supplier objSupplier { get; set; }

        public List<Supplier> lstsupplier = new List<Supplier>();

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
               // lstsupplier = await _supplier.GetAllAsync(filter: x => x.IsDelete != true, orderBy: mt => mt.OrderByDescending(m => m.ModifiedDate == null ? m.CreatedDate : m.ModifiedDate));
                //if (lstsupplier == null || lstsupplier?.Count == 0)
                //{
                //    ViewData["SuccessMsg"] = $"{Constants.NoRecordsFoundMessage}";
                //}
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetAsync(), Supplier");
                throw;
            }
        }
        /// <summary>
        /// OnGetCreateAsync
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetCreateAsync()
        {
            try
            {
              
                if (objSupplier == null)
                {
                    objSupplier = new Supplier();
                }
                objSupplier.IsActive = true; 
                ViewData["PageType"] = "Edit";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetCreateAsync(), Product");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnGetEditAsync
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetEditAsync(int supplierId)
        {
            try
            {
                objSupplier = await _supplier.GetAsync(filter: x => x.Id == supplierId && x.IsDelete != true);

                if (objSupplier == null)
                {
                    return Redirect("~/all-supplier");
                }
                ViewData["PageType"] = "Edit";
                ViewData["PagePrimaryID"] = objSupplier.Id;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetEditAsync(), Supplier, SupplierId: { supplierId }");
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
                objSupplier = new Supplier();
                objSupplier.IsActive = false;
                ViewData["PageType"] = "Edit";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetClear(), Supplier");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnResetClear
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetReset(int supplierId)
        {
            try
            {
                objSupplier = await _supplier.GetAsync(filter: x => x.Id == supplierId && x.IsDelete != true);
                ViewData["PageType"] = "Edit";
                ViewData["PagePrimaryID"] = objSupplier.Id;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnResetClear(), Supplier, SupplierId: { objSupplier.Id }");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnGetDeleteSupplierAsync
        /// </summary>
        /// <param name="supplierId"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetDeleteSupplierAsync(int supplierId)
        {
            try
            {
                var supplier = await _supplier.GetAsync(filter: x => x.Id == supplierId);
                if (supplier != null)
                {
                    await _supplier.LogicalDeleteAsync(supplier);
                    ViewData["SuccessMsg"] = $"Supplier: {supplier.Name} {Constants.DeletedMessage}";
                    HttpContext.Session.SetString(Constants.StatusMessage, $"Supplier: {supplier.Name} {Constants.DeletedMessage}");
                }

                ViewData["PageType"] = "List";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetDeleteSupplierAsync(), Supplier, SupplierId: { supplierId }");
                throw;
            }
            return Redirect("~/all-supplier");
        }

        /// <summary>
        /// OnPostSubmitAsync
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostSubmitAsync()
        {
            try
            {
                Supplier existingSupplier = await _supplier.GetAsync(filter: x => x.Name == objSupplier.Name && x.IsDelete != true);
                if (existingSupplier != null)
                {
                    if ((objSupplier.Id == 0) || (objSupplier.Id != 0 && objSupplier.Id != existingSupplier.Id))
                    {
                        ViewData["PageType"] = "Edit";
                        if (objSupplier.Id != 0)
                        {
                            ViewData["PagePrimaryID"] = objSupplier.Id;
                        }
                        ViewData["InfoMsg"] = $"Supplier: {objSupplier.Name} {Constants.AlreadyExistMessage}";
                        return Page();
                    }
                }

                if (ModelState.IsValid)
                {
                    if (objSupplier.Id == 0)
                    {
                        await _supplier.InsertAsync(objSupplier);
                        HttpContext.Session.SetString(Constants.StatusMessage, Constants.SaveMessage);
                    }
                    else
                    {
                        await _supplier.UpdateAsync(objSupplier);
                        HttpContext.Session.SetString(Constants.StatusMessage, Constants.UpdateMessage);
                    }
                    return Redirect("all-supplier");
                }
                else
                {
                    ViewData["PageType"] = "Edit";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnPostSubmitAsync(), Supplier, SupplierId: { objSupplier?.Id }");
                throw;
            }
        }

        /// <summary>
        /// OnPostImportExcelAsync - Handle Excel import for suppliers
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostImportExcelAsync(IFormFile excelFile, bool validateData = true)
        {
            try
            {
                if (excelFile == null || excelFile.Length == 0)
                {
                    return new JsonResult(new { success = false, message = "Please select a valid Excel file." });
                }

                // Validate file extension
                var allowedExtensions = new[] { ".xlsx", ".xls" };
                var fileExtension = Path.GetExtension(excelFile.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return new JsonResult(new { success = false, message = "Only Excel files (.xlsx, .xls) are allowed." });
                }

                // Validate file size (50MB limit)
                if (excelFile.Length > 50 * 1024 * 1024)
                {
                    return new JsonResult(new { success = false, message = "File size exceeds 50MB limit." });
                }

                var importResult = await ProcessExcelImport(excelFile, validateData);
                return new JsonResult(importResult);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in OnPostImportExcelAsync(), Supplier Excel Import");
                return new JsonResult(new { success = false, message = $"Import failed: {ex.Message}" });
            }
        }

        /// <summary>
        /// ProcessExcelImport - Process the Excel file and import suppliers using NPOI (Free)
        /// </summary>
        /// <param name="excelFile"></param>
        /// <param name="validateData"></param>
        /// <returns></returns>
        private async Task<object> ProcessExcelImport(IFormFile excelFile, bool validateData)
        {
            var suppliers = new List<Supplier>();
            var errors = new List<string>();
            var successfulRecords = new List<object>();
            int totalRecords = 0;
            int successCount = 0;
            int errorCount = 0;

            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);
                stream.Position = 0; // Reset stream position for reading

                IWorkbook workbook = null;
                try
                {
                    // Determine file type and create appropriate workbook
                    var fileExtension = Path.GetExtension(excelFile.FileName).ToLowerInvariant();
                    if (fileExtension == ".xlsx")
                    {
                        workbook = new XSSFWorkbook(stream); // For .xlsx files
                    }
                    else if (fileExtension == ".xls")
                    {
                        workbook = new HSSFWorkbook(stream); // For .xls files
                    }
                    else
                    {
                        return new { success = false, message = "Unsupported file format. Please use .xlsx or .xls files." };
                    }

                    // Get the first worksheet
                    var worksheet = workbook.GetSheetAt(0);
                    if (worksheet == null)
                    {
                        return new { success = false, message = "No worksheet found in the Excel file." };
                    }

                    // Get the used range
                    var startRow = 1; // Row 0 is header, start from row 1
                    var endRow = worksheet.LastRowNum;
                    totalRecords = Math.Max(0, endRow - startRow + 1);

                    if (totalRecords == 0)
                    {
                        return new { success = false, message = "No data rows found in the Excel file." };
                    }

                    // Create column mapping from header row
                    var headerRow = worksheet.GetRow(0);
                    if (headerRow == null)
                    {
                        return new { success = false, message = "Header row not found in the Excel file." };
                    }
                    
                    var columnMapping = CreateColumnMapping(headerRow);
                    
                    // Validate required columns exist
                    var requiredColumns = new[] { "SupplierName", "SKUCode" };
                    var missingColumns = requiredColumns.Where(col => !columnMapping.ContainsKey(col)).ToList();
                    if (missingColumns.Any())
                    {
                        return new { success = false, message = $"Missing required columns: {string.Join(", ", missingColumns)}" };
                    }

                    // Process each row
                    for (int rowIndex = startRow; rowIndex <= endRow; rowIndex++)
                    {
                        try
                        {
                            var row = worksheet.GetRow(rowIndex);
                            if (row == null) continue; // Skip null rows

                            var supplier = new Supplier();
                            var rowErrors = new List<string>();

                            // Extract data from Excel row using column names
                            var supplierName = GetCellValue(row, "SupplierName", columnMapping)?.Trim();
                            var skuCode = GetCellValue(row, "SKUCode", columnMapping)?.Trim();
                            var isActiveText = GetCellValue(row, "IsActive", columnMapping)?.Trim();
                            var isPreferredText = GetCellValue(row, "IsPreferred", columnMapping)?.Trim();
                            var leadTimeDaysText = GetCellValue(row, "LeadTimeDays", columnMapping)?.Trim();
                            var moqCaseText = GetCellValue(row, "MoqCase", columnMapping)?.Trim();
                            var lastCostText = GetCellValue(row, "LastCost", columnMapping)?.Trim();
                            var incoterm = GetCellValue(row, "Incoterm", columnMapping)?.Trim();
                            var validFromText = GetCellValue(row, "ValidFrom", columnMapping)?.Trim();
                            var validToText = GetCellValue(row, "ValidTo", columnMapping)?.Trim();

                            // Validate required fields
                            if (validateData)
                            {
                                if (string.IsNullOrEmpty(supplierName))
                                    rowErrors.Add($"Row {rowIndex + 1}: SupplierName is required");
                                if (string.IsNullOrEmpty(skuCode))
                                    rowErrors.Add($"Row {rowIndex + 1}: SKUCode is required");
                            }

                            // Skip empty rows
                            if (string.IsNullOrEmpty(supplierName) && string.IsNullOrEmpty(skuCode))
                                continue;

                            // Map to Supplier model (based on current model structure)
                            supplier.Name = supplierName;
                            supplier.Skucode = skuCode;
                            
                            // Handle additional fields with proper validation and error handling
                            try
                            {
                                // Incoterm - direct assignment
                                supplier.Incoterm = !string.IsNullOrEmpty(incoterm) ? incoterm : null;
                                
                                // IsPreferred - safe boolean parsing
                                supplier.IsPreferred = ParseBoolean(isPreferredText, false);
                                
                                // LeadTimeDays - safe integer parsing (non-nullable)
                                if (!string.IsNullOrEmpty(leadTimeDaysText) && int.TryParse(leadTimeDaysText, out int leadDays))
                                {
                                    supplier.LeadTimeDays = leadDays;
                                }
                                else
                                {
                                    supplier.LeadTimeDays = 0; // Default value for non-nullable int
                                    if (validateData && !string.IsNullOrEmpty(leadTimeDaysText))
                                        rowErrors.Add($"Row {rowIndex + 1}: Invalid LeadTimeDays value '{leadTimeDaysText}', using default 0");
                                }
                                
                                // MoqCase - direct assignment as string
                                supplier.MoqCase = !string.IsNullOrEmpty(moqCaseText) ? moqCaseText : null;
                                
                                // LastCost - safe decimal parsing (non-nullable)
                                if (!string.IsNullOrEmpty(lastCostText) && decimal.TryParse(lastCostText, out decimal lastCost))
                                {
                                    supplier.LastCost = lastCost;
                                }
                                else
                                {
                                    supplier.LastCost = 0.00m; // Default value for non-nullable decimal
                                    if (validateData && !string.IsNullOrEmpty(lastCostText))
                                        rowErrors.Add($"Row {rowIndex + 1}: Invalid LastCost value '{lastCostText}', using default 0.00");
                                }
                                
                                // ValidFrom - safe date parsing (non-nullable)
                                if (!string.IsNullOrEmpty(validFromText) && DateTime.TryParse(validFromText, out DateTime validFrom))
                                {
                                    supplier.ValidFrom = validFrom;
                                }
                                else
                                {
                                    supplier.ValidFrom = DateTime.UtcNow; // Default to current date for non-nullable DateTime
                                    if (validateData && !string.IsNullOrEmpty(validFromText))
                                        rowErrors.Add($"Row {rowIndex + 1}: Invalid ValidFrom date '{validFromText}', using current date");
                                }
                                
                                // ValidTo - safe date parsing (non-nullable)
                                if (!string.IsNullOrEmpty(validToText) && DateTime.TryParse(validToText, out DateTime validTo))
                                {
                                    supplier.ValidTo = validTo;
                                }
                                else
                                {
                                    supplier.ValidTo = DateTime.UtcNow.AddYears(1); // Default to 1 year from now for non-nullable DateTime
                                    if (validateData && !string.IsNullOrEmpty(validToText))
                                        rowErrors.Add($"Row {rowIndex + 1}: Invalid ValidTo date '{validToText}', using 1 year from now");
                                }
                                
                                // Validate date logic - ValidTo should be after ValidFrom (non-nullable DateTime)
                                if (validateData && supplier.ValidTo <= supplier.ValidFrom)
                                {
                                    rowErrors.Add($"Row {rowIndex + 1}: ValidTo date must be after ValidFrom date");
                                }
                            }
                            catch (Exception ex)
                            {
                                rowErrors.Add($"Row {rowIndex + 1}: Error parsing additional fields - {ex.Message}");
                            }
                            // Build description with additional fields
                            var descriptionParts = new List<string>();
                            if (!string.IsNullOrEmpty(incoterm)) descriptionParts.Add($"Incoterm: {incoterm}");
                            if (!string.IsNullOrEmpty(isPreferredText)) descriptionParts.Add($"Preferred: {isPreferredText}");
                            if (!string.IsNullOrEmpty(leadTimeDaysText)) descriptionParts.Add($"Lead Time: {leadTimeDaysText} days");
                            if (!string.IsNullOrEmpty(moqCaseText)) descriptionParts.Add($"MOQ: {moqCaseText}");
                            if (!string.IsNullOrEmpty(lastCostText)) descriptionParts.Add($"Last Cost: {lastCostText}");
                            if (!string.IsNullOrEmpty(validFromText)) descriptionParts.Add($"Valid From: {validFromText}");
                            if (!string.IsNullOrEmpty(validToText)) descriptionParts.Add($"Valid To: {validToText}");
                            
                            supplier.Description = descriptionParts.Count > 0 ? string.Join(" | ", descriptionParts) : "Imported supplier";
                            
                            // Parse boolean values
                            supplier.IsActive = ParseBoolean(isActiveText, true); // Default to true
                            supplier.IsDelete = false;

                            // Set audit fields
                            supplier.CreatedDate = DateTime.UtcNow;
                            supplier.Createdby = 1; // You may want to get this from session
                            supplier.ModifiedDate = DateTime.UtcNow;
                            supplier.Modifiedby = 1;

                            // Add validation errors if any
                            if (rowErrors.Count > 0)
                            {
                                errors.AddRange(rowErrors);
                                errorCount++;
                                continue;
                            }

                            suppliers.Add(supplier);
                        }
                        catch (Exception ex)
                        {
                            errors.Add($"Row {rowIndex + 1}: Error processing row - {ex.Message}");
                            errorCount++;
                        }
                    }
                }
                finally
                {
                    workbook?.Close();
                }
            }

            // Bulk insert suppliers
            if (suppliers.Count > 0)
            {
                try
                {
                    // Process in batches for better performance with large datasets
                    const int batchSize = 1000;
                    var batches = suppliers.Select((supplier, index) => new { supplier, index })
                                          .GroupBy(x => x.index / batchSize)
                                          .Select(g => g.Select(x => x.supplier).ToList())
                                          .ToList();

                    foreach (var batch in batches)
                    {
                        foreach (var supplier in batch)
                        {
                            try
                            {
                                // Check for duplicates based on Name and SKUCode
                                var existingSupplier = await _supplier.GetAsync(filter: x => 
                                    x.Name == supplier.Name && x.Skucode == supplier.Skucode && x.IsDelete != true);
                                
                                if (existingSupplier == null)
                                {
                                    await _supplier.InsertAsync(supplier);
                                    successCount++;
                                    
                                    // Track successful record
                                    successfulRecords.Add(new
                                    {
                                        rowNumber = suppliers.IndexOf(supplier) + 2, // +2 because Excel is 1-based and has header
                                        supplierName = supplier.Name,
                                        skuCode = supplier.Skucode,
                                        status = "Imported Successfully"
                                    });
                                }
                                else
                                {
                                    errors.Add($"Supplier '{supplier.Name}' with SKU '{supplier.Skucode}' already exists");
                                    errorCount++;
                                }
                            }
                            catch (Exception ex)
                            {
                                errors.Add($"Error inserting supplier '{supplier.Name}': {ex.Message}");
                                errorCount++;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error during bulk supplier insert");
                    return new { success = false, message = $"Bulk insert failed: {ex.Message}" };
                }
            }

            return new
            {
                success = true,
                totalRecords = totalRecords,
                successCount = successCount,
                errorCount = errorCount,
                errors = errors.Take(100).ToList(), // Limit errors to prevent large response
                successfulRecords = successfulRecords.Take(100).ToList() // Limit successful records for performance
            };
        }

        /// <summary>
        /// CreateColumnMapping - Helper method to create column name to index mapping from header row
        /// </summary>
        /// <param name="headerRow"></param>
        /// <returns></returns>
        private Dictionary<string, int> CreateColumnMapping(IRow headerRow)
        {
            var columnMapping = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            
            for (int i = 0; i < headerRow.LastCellNum; i++)
            {
                var cell = headerRow.GetCell(i);
                if (cell != null)
                {
                    var columnName = cell.StringCellValue?.Trim();
                    if (!string.IsNullOrEmpty(columnName))
                    {
                        columnMapping[columnName] = i;
                    }
                }
            }
            
            return columnMapping;
        }

        /// <summary>
        /// GetCellValue - Helper method to get cell value from Excel row by column index
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        private string GetCellValue(IRow row, int columnIndex)
        {
            var cell = row.GetCell(columnIndex);
            if (cell == null) return string.Empty;

            switch (cell.CellType)
            {
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Numeric:
                    if (DateUtil.IsCellDateFormatted(cell))
                    {
                        return Convert.ToDateTime(cell.DateCellValue).ToString("yyyy-MM-dd");
                    }
                    return cell.NumericCellValue.ToString();
                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();
                case CellType.Formula:
                    try
                    {
                        return cell.NumericCellValue.ToString();
                    }
                    catch
                    {
                        return cell.StringCellValue;
                    }
                   
                case CellType.Blank:
                    return string.Empty;
                default:
                    return cell.ToString();
            }
        }

        /// <summary>
        /// GetCellValue - Helper method to get cell value from Excel row by column name
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnName"></param>
        /// <param name="columnMapping"></param>
        /// <returns></returns>
        private string GetCellValue(IRow row, string columnName, Dictionary<string, int> columnMapping)
        {
            if (columnMapping.TryGetValue(columnName, out int columnIndex))
            {
                return GetCellValue(row, columnIndex);
            }
            
            return string.Empty; // Column not found
        }

        /// <summary>
        /// ParseBoolean - Helper method to parse boolean values from Excel
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private bool ParseBoolean(string value, bool defaultValue = false)
        {
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            value = value.ToLowerInvariant().Trim();
            
            if (value == "true" || value == "yes" || value == "1" || value == "y")
                return true;
            
            if (value == "false" || value == "no" || value == "0" || value == "n")
                return false;

            return defaultValue;
        }

        /// <summary>
        /// OnGetGenerateSampleExcelAsync - Generate sample Excel file with 50,000 records
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetGenerateSampleExcelAsync()
        {
            try
            {
                var excelBytes = await GenerateSampleExcelFile();
                var fileName = $"SampleSuppliers_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error generating sample Excel file");
                return new JsonResult(new { success = false, message = $"Failed to generate sample file: {ex.Message}" });
            }
        }

        /// <summary>
        /// GenerateSampleExcelFile - Create Excel file with 50,000 sample supplier records
        /// </summary>
        /// <returns></returns>
        private async Task<byte[]> GenerateSampleExcelFile()
        {
            using (var workbook = new XSSFWorkbook())
            {
                var worksheet = workbook.CreateSheet("Suppliers");
                
                // Create header row
                var headerRow = worksheet.CreateRow(0);
                var headers = new string[] 
                {
                    "SupplierName", "SKUCode", "IsActive", "IsPreferred", 
                    "LeadTimeDays", "MoqCase", "LastCost", "Incoterm", 
                    "ValidFrom", "ValidTo"
                };

                // Style header row
                var headerStyle = workbook.CreateCellStyle();
                var headerFont = workbook.CreateFont();
                headerFont.IsBold = true;
                headerFont.Color = NPOI.HSSF.Util.HSSFColor.White.Index;
                headerStyle.SetFont(headerFont);
                headerStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.DarkBlue.Index;
                headerStyle.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
                headerStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                headerStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                headerStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                headerStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;

                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = headerRow.CreateCell(i);
                    cell.SetCellValue(headers[i]);
                    cell.CellStyle = headerStyle;
                    worksheet.SetColumnWidth(i, 4000); // Set column width
                }

                // Sample data arrays for variety
                var supplierPrefixes = new[] { "Global", "Premium", "Elite", "Prime", "Superior", "Advanced", "Quality", "Reliable", "Trusted", "Professional" };
                var supplierSuffixes = new[] { "Supplies", "Trading", "Corp", "Industries", "Solutions", "Enterprises", "Group", "International", "Systems", "Partners" };
                var incoterms = new[] { "FOB", "CIF", "EXW", "DDP", "DAP", "FCA", "CPT", "CIP" };
                var booleanValues = new[] { "Yes", "No", "True", "False", "1", "0" };
                
                var random = new Random(12345); // Fixed seed for consistent data
                var baseDate = new DateTime(2024, 1, 1);

                // Generate 50,000 data rows
                for (int rowIndex = 1; rowIndex <= 50000; rowIndex++)
                {
                    var dataRow = worksheet.CreateRow(rowIndex);
                    
                    // SupplierName
                    var supplierName = $"{supplierPrefixes[random.Next(supplierPrefixes.Length)]} {supplierSuffixes[random.Next(supplierSuffixes.Length)]} {rowIndex:D5}";
                    dataRow.CreateCell(0).SetCellValue(supplierName);
                    
                    // SKUCode
                    var skuCode = $"SKU{rowIndex:D6}-{random.Next(100, 999)}";
                    dataRow.CreateCell(1).SetCellValue(skuCode);
                    
                    // IsActive
                    var isActive = booleanValues[random.Next(booleanValues.Length)];
                    dataRow.CreateCell(2).SetCellValue(isActive);
                    
                    // IsPreferred
                    var isPreferred = booleanValues[random.Next(booleanValues.Length)];
                    dataRow.CreateCell(3).SetCellValue(isPreferred);
                    
                    // LeadTimeDays
                    var leadTimeDays = random.Next(1, 90);
                    dataRow.CreateCell(4).SetCellValue(leadTimeDays);
                    
                    // MoqCase
                    var moqCase = random.Next(10, 1000);
                    dataRow.CreateCell(5).SetCellValue(moqCase);
                    
                    // LastCost
                    var lastCost = Math.Round(random.NextDouble() * 1000 + 10, 2);
                    dataRow.CreateCell(6).SetCellValue(lastCost);
                    
                    // Incoterm
                    var incoterm = incoterms[random.Next(incoterms.Length)];
                    dataRow.CreateCell(7).SetCellValue(incoterm);
                    
                    // ValidFrom
                    var validFrom = baseDate.AddDays(random.Next(0, 365));
                    var validFromCell = dataRow.CreateCell(8);
                    validFromCell.SetCellValue(validFrom);
                    
                    // ValidTo
                    var validTo = validFrom.AddDays(random.Next(30, 730));
                    var validToCell = dataRow.CreateCell(9);
                    validToCell.SetCellValue(validTo);

                    // Apply date formatting to date cells
                    var dateStyle = workbook.CreateCellStyle();
                    var dateFormat = workbook.CreateDataFormat();
                    dateStyle.DataFormat = dateFormat.GetFormat("yyyy-mm-dd");
                    validFromCell.CellStyle = dateStyle;
                    validToCell.CellStyle = dateStyle;

                    // Progress logging every 10,000 records
                    if (rowIndex % 10000 == 0)
                    {
                        Log.Information($"Generated {rowIndex} sample records");
                    }
                }

                // Auto-size columns for better readability
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.AutoSizeColumn(i);
                    // Set minimum width
                    if (worksheet.GetColumnWidth(i) < 3000)
                        worksheet.SetColumnWidth(i, 3000);
                }

                // Convert to byte array
                using (var stream = new MemoryStream())
                {
                    workbook.Write(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}
