using GoodsEnterprise.Model.Models;
using GoodsEnterprise.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace GoodsEnterprise.Web.Controllers
{
    /// <summary>
    /// API Controller for supplier import operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SupplierImportController : ControllerBase
    {
        private readonly ISupplierImportService _importService;
        private readonly ILogger<SupplierImportController> _logger;

        public SupplierImportController(
            ISupplierImportService importService,
            ILogger<SupplierImportController> logger)
        {
            _importService = importService;
            _logger = logger;
        }

        /// <summary>
        /// Preview import data from Excel file
        /// </summary>
        /// <param name="file">Excel file to preview</param>
        /// <param name="validateData">Whether to perform validation (default: true)</param>
        /// <returns>Preview result with validation information</returns>
        [HttpPost("preview")]
        public async Task<IActionResult> PreviewImport(
            IFormFile file, 
            [FromForm] bool validateData = true)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { error = "No file provided or file is empty." });
                }

                _logger.LogInformation("Preview import request received. File: {FileName}, Size: {FileSize}", 
                    file.FileName, file.Length);

                var preview = await _importService.PreviewImportAsync(file, validateData);

                return Ok(new
                {
                    success = true,
                    data = preview,
                    message = preview.CanProceedWithImport 
                        ? "File is ready for import." 
                        : "File has issues that need to be resolved before import."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during import preview");
                return StatusCode(500, new { 
                    error = "An error occurred while previewing the import.", 
                    details = ex.Message 
                });
            }
        }

        /// <summary>
        /// Start supplier import process
        /// </summary>
        /// <param name="file">Excel file to import</param>
        /// <param name="validateData">Whether to perform validation (default: true)</param>
        /// <returns>Import result</returns>
        [HttpPost("import")]
        public async Task<IActionResult> ImportSuppliers(
            IFormFile file,
            [FromForm] bool validateData = true)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { error = "No file provided or file is empty." });
                }

                // Get user ID from session or claims (implement based on your auth system)
                int? userId = GetCurrentUserId();

                _logger.LogInformation("Import request received. File: {FileName}, Size: {FileSize}, UserId: {UserId}", 
                    file.FileName, file.Length, userId);

                var result = await _importService.ImportSuppliersAsync(file, validateData, userId);

                // Store import results in session for download functionality
                var importResultJson = JsonSerializer.Serialize(result);
                HttpContext.Session.SetString($"ImportResult_{result.ImportId}", importResultJson);

                var response = new
                {
                    success = result.IsSuccess,
                    importId = result.ImportId,
                    data = new
                    {
                        totalRecords = result.TotalRecords,
                        successfulImports = result.SuccessfulImports,
                        failedImports = result.FailedImports,
                        skippedRecords = result.SkippedRecords,
                        duration = result.Duration?.TotalSeconds,
                        status = result.Status
                    },
                    summary = new
                    {
                        successfulSuppliers = result.SuccessfulSuppliers.Take(10).Select(s => new
                        {
                            rowNumber = s.RowNumber,
                            supplierName = s.SupplierName,
                            skuCode = s.SKUCode,
                            email = s.Email,
                            id = s.Id
                        }),
                        failedSuppliers = result.FailedSuppliers.Take(10).Select(s => new
                        {
                            rowNumber = s.RowNumber,
                            supplierName = s.SupplierName,
                            skuCode = s.SKUCode,
                            email = s.Email,
                            errors = s.ValidationErrors,
                            warnings = s.ValidationWarnings
                        }),
                        duplicates = result.Duplicates.Take(10)
                    },
                    errors = result.GlobalErrors,
                    warnings = result.GlobalWarnings,
                    message = result.IsSuccess 
                        ? $"Successfully imported {result.SuccessfulImports} suppliers." 
                        : $"Import completed with {result.FailedImports} failures."
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during supplier import");
                return StatusCode(500, new { 
                    error = "An error occurred during the import process.", 
                    details = ex.Message 
                });
            }
        }

        /// <summary>
        /// Get import progress for a specific import session
        /// </summary>
        /// <param name="importId">Import session ID</param>
        /// <returns>Import progress information</returns>
        [HttpGet("progress/{importId}")]
        public async Task<IActionResult> GetImportProgress(string importId)
        {
            try
            {
                var progress = await _importService.GetImportProgressAsync(importId);
                
                if (progress.Status == "Not Found")
                {
                    return NotFound(new { error = "Import session not found." });
                }

                return Ok(new
                {
                    success = true,
                    data = progress,
                    message = $"Import is {progress.Status.ToLower()}."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting import progress for ImportId: {ImportId}", importId);
                return StatusCode(500, new { 
                    error = "An error occurred while retrieving import progress.", 
                    details = ex.Message 
                });
            }
        }

        /// <summary>
        /// Cancel an ongoing import operation
        /// </summary>
        /// <param name="importId">Import session ID</param>
        /// <returns>Cancellation result</returns>
        [HttpPost("cancel/{importId}")]
        public async Task<IActionResult> CancelImport(string importId)
        {
            try
            {
                var cancelled = await _importService.CancelImportAsync(importId);
                
                if (!cancelled)
                {
                    return NotFound(new { error = "Import session not found or cannot be cancelled." });
                }

                return Ok(new
                {
                    success = true,
                    message = "Import cancellation requested successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling import for ImportId: {ImportId}", importId);
                return StatusCode(500, new { 
                    error = "An error occurred while cancelling the import.", 
                    details = ex.Message 
                });
            }
        }

        /// <summary>
        /// Get detailed import results
        /// </summary>
        /// <param name="importId">Import session ID</param>
        /// <param name="includeSuccessful">Include successful records (default: false)</param>
        /// <param name="includeFailed">Include failed records (default: true)</param>
        /// <param name="pageSize">Number of records per page (default: 50)</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <returns>Detailed import results</returns>
        [HttpGet("results/{importId}")]
        public async Task<IActionResult> GetImportResults(
            string importId,
            [FromQuery] bool includeSuccessful = false,
            [FromQuery] bool includeFailed = true,
            [FromQuery] int pageSize = 50,
            [FromQuery] int page = 1)
        {
            try
            {
                // This would typically retrieve results from a database or cache
                // For now, return a placeholder response
                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        importId = importId,
                        page = page,
                        pageSize = pageSize,
                        totalPages = 0,
                        successfulRecords = new List<object>(),
                        failedRecords = new List<object>()
                    },
                    message = "Import results retrieved successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting import results for ImportId: {ImportId}", importId);
                return StatusCode(500, new { 
                    error = "An error occurred while retrieving import results.", 
                    details = ex.Message 
                });
            }
        }

        /// <summary>
        /// Download import results as Excel file
        /// </summary>
        /// <param name="importId">Import session ID</param>
        /// <param name="includeSuccessful">Include successful records</param>
        /// <param name="includeFailed">Include failed records</param>
        /// <returns>Excel file with import results</returns>
        [HttpGet("download/{importId}")]
        public async Task<IActionResult> DownloadImportResults(
            string importId,
            [FromQuery] bool includeSuccessful = true,
            [FromQuery] bool includeFailed = true)
        {
            try
            {
                _logger.LogInformation("Download import results request for ImportId: {ImportId}", importId);

                // Get import results from session or cache
                var importResultJson = HttpContext.Session.GetString($"ImportResult_{importId}");
                
                if (string.IsNullOrEmpty(importResultJson))
                {
                    return NotFound(new { 
                        success = false,
                        error = "Import results not found. The session may have expired." 
                    });
                }

                var importResult = JsonSerializer.Deserialize<SupplierImportResult>(importResultJson);
                
                if (importResult == null)
                {
                    return NotFound(new { 
                        success = false,
                        error = "Unable to retrieve import results." 
                    });
                }

                // Generate Excel file
                var excelBytes = GenerateImportResultsExcel(importResult, includeSuccessful, includeFailed);
                
                var fileName = $"Supplier_Import_Results_{importId}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                
                return File(excelBytes, 
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                    fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading import results for ImportId: {ImportId}", importId);
                return StatusCode(500, new { 
                    error = "An error occurred while generating the download.", 
                    details = ex.Message 
                });
            }
        }

        /// <summary>
        /// Generate Excel file with import results
        /// </summary>
        private byte[] GenerateImportResultsExcel(
            SupplierImportResult importResult, 
            bool includeSuccessful, 
            bool includeFailed)
        {
            using (var workbook = new NPOI.XSSF.UserModel.XSSFWorkbook())
            {
                // Create Summary Sheet
                var summarySheet = workbook.CreateSheet("Summary");
                CreateSummarySheet(summarySheet, importResult);

                // Create Failed Records Sheet
                if (includeFailed && importResult.FailedSuppliers.Any())
                {
                    var failedSheet = workbook.CreateSheet("Failed Records");
                    CreateFailedRecordsSheet(failedSheet, importResult.FailedSuppliers);
                }

                // Create Successful Records Sheet
                if (includeSuccessful && importResult.SuccessfulSuppliers.Any())
                {
                    var successSheet = workbook.CreateSheet("Successful Records");
                    CreateSuccessfulRecordsSheet(successSheet, importResult.SuccessfulSuppliers);
                }

                // Create Duplicates Sheet
                if (importResult.Duplicates.Any())
                {
                    var duplicatesSheet = workbook.CreateSheet("Duplicates");
                    CreateDuplicatesSheet(duplicatesSheet, importResult.Duplicates);
                }

                // Write to memory stream
                using (var ms = new System.IO.MemoryStream())
                {
                    workbook.Write(ms);
                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// Create summary sheet with import statistics
        /// </summary>
        private void CreateSummarySheet(NPOI.SS.UserModel.ISheet sheet, SupplierImportResult result)
        {
            var headerStyle = sheet.Workbook.CreateCellStyle();
            var headerFont = sheet.Workbook.CreateFont();
            headerFont.IsBold = true;
            headerFont.FontHeightInPoints = 12;
            headerStyle.SetFont(headerFont);

            int rowIndex = 0;

            // Title
            var titleRow = sheet.CreateRow(rowIndex++);
            var titleCell = titleRow.CreateCell(0);
            titleCell.SetCellValue("Supplier Import Results Summary");
            titleCell.CellStyle = headerStyle;
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, 1));

            rowIndex++; // Empty row

            // Import Details
            CreateSummaryRow(sheet, rowIndex++, "Import ID:", result.ImportId);
            CreateSummaryRow(sheet, rowIndex++, "Status:", result.Status);
            CreateSummaryRow(sheet, rowIndex++, "Start Time:", result.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));
            if (result.EndTime.HasValue)
            {
                CreateSummaryRow(sheet, rowIndex++, "End Time:", result.EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                CreateSummaryRow(sheet, rowIndex++, "Duration:", $"{result.Duration?.TotalSeconds:F2} seconds");
            }

            rowIndex++; // Empty row

            // Statistics
            CreateSummaryRow(sheet, rowIndex++, "Total Records:", result.TotalRecords.ToString());
            CreateSummaryRow(sheet, rowIndex++, "Successful Imports:", result.SuccessfulImports.ToString());
            CreateSummaryRow(sheet, rowIndex++, "Failed Imports:", result.FailedImports.ToString());
            CreateSummaryRow(sheet, rowIndex++, "Skipped Records:", result.SkippedRecords.ToString());
            CreateSummaryRow(sheet, rowIndex++, "Duplicates Found:", result.Duplicates.Count.ToString());

            rowIndex++; // Empty row

            // Global Errors
            if (result.GlobalErrors.Any())
            {
                var errorHeaderRow = sheet.CreateRow(rowIndex++);
                var errorHeaderCell = errorHeaderRow.CreateCell(0);
                errorHeaderCell.SetCellValue("Global Errors:");
                errorHeaderCell.CellStyle = headerStyle;

                foreach (var error in result.GlobalErrors)
                {
                    var errorRow = sheet.CreateRow(rowIndex++);
                    errorRow.CreateCell(0).SetCellValue("• " + error);
                }
                rowIndex++; // Empty row
            }

            // Global Warnings
            if (result.GlobalWarnings.Any())
            {
                var warningHeaderRow = sheet.CreateRow(rowIndex++);
                var warningHeaderCell = warningHeaderRow.CreateCell(0);
                warningHeaderCell.SetCellValue("Global Warnings:");
                warningHeaderCell.CellStyle = headerStyle;

                foreach (var warning in result.GlobalWarnings)
                {
                    var warningRow = sheet.CreateRow(rowIndex++);
                    warningRow.CreateCell(0).SetCellValue("• " + warning);
                }
            }

            // Auto-size columns
            sheet.AutoSizeColumn(0);
            sheet.AutoSizeColumn(1);
        }

        /// <summary>
        /// Create a summary row with label and value
        /// </summary>
        private void CreateSummaryRow(NPOI.SS.UserModel.ISheet sheet, int rowIndex, string label, string value)
        {
            var row = sheet.CreateRow(rowIndex);
            var labelCell = row.CreateCell(0);
            labelCell.SetCellValue(label);
            
            var labelStyle = sheet.Workbook.CreateCellStyle();
            var labelFont = sheet.Workbook.CreateFont();
            labelFont.IsBold = true;
            labelStyle.SetFont(labelFont);
            labelCell.CellStyle = labelStyle;

            var valueCell = row.CreateCell(1);
            valueCell.SetCellValue(value);
        }

        /// <summary>
        /// Create failed records sheet
        /// </summary>
        private void CreateFailedRecordsSheet(NPOI.SS.UserModel.ISheet sheet, List<SupplierImport> failedRecords)
        {
            var headerStyle = sheet.Workbook.CreateCellStyle();
            var headerFont = sheet.Workbook.CreateFont();
            headerFont.IsBold = true;
            headerFont.Color = NPOI.HSSF.Util.HSSFColor.White.Index;
            headerStyle.SetFont(headerFont);
            headerStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Red.Index;
            headerStyle.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;

            // Header row
            var headerRow = sheet.CreateRow(0);
            string[] headers = { "Row #", "Supplier Name", "SKU Code", "Email", "Phone", "Description", "Errors", "Warnings" };
            
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = headerRow.CreateCell(i);
                cell.SetCellValue(headers[i]);
                cell.CellStyle = headerStyle;
            }

            // Data rows
            int rowIndex = 1;
            foreach (var record in failedRecords)
            {
                var row = sheet.CreateRow(rowIndex++);
                row.CreateCell(0).SetCellValue(record.RowNumber);
                row.CreateCell(1).SetCellValue(record.SupplierName ?? "");
                row.CreateCell(2).SetCellValue(record.SKUCode ?? "");
                row.CreateCell(3).SetCellValue(record.Email ?? "");
                row.CreateCell(4).SetCellValue(record.Phone ?? "");
                row.CreateCell(5).SetCellValue(record.Description ?? "");
                row.CreateCell(6).SetCellValue(string.Join("; ", record.ValidationErrors));
                row.CreateCell(7).SetCellValue(string.Join("; ", record.ValidationWarnings));
            }

            // Auto-size columns
            for (int i = 0; i < headers.Length; i++)
            {
                sheet.AutoSizeColumn(i);
            }
        }

        /// <summary>
        /// Create successful records sheet
        /// </summary>
        private void CreateSuccessfulRecordsSheet(NPOI.SS.UserModel.ISheet sheet, List<SupplierImport> successfulRecords)
        {
            var headerStyle = sheet.Workbook.CreateCellStyle();
            var headerFont = sheet.Workbook.CreateFont();
            headerFont.IsBold = true;
            headerFont.Color = NPOI.HSSF.Util.HSSFColor.White.Index;
            headerStyle.SetFont(headerFont);
            headerStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Green.Index;
            headerStyle.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;

            // Header row
            var headerRow = sheet.CreateRow(0);
            string[] headers = { "Row #", "ID", "Supplier Name", "SKU Code", "Email", "Phone", "Description", "Status" };
            
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = headerRow.CreateCell(i);
                cell.SetCellValue(headers[i]);
                cell.CellStyle = headerStyle;
            }

            // Data rows
            int rowIndex = 1;
            foreach (var record in successfulRecords)
            {
                var row = sheet.CreateRow(rowIndex++);
                row.CreateCell(0).SetCellValue(record.RowNumber);
                row.CreateCell(1).SetCellValue(record.Id);
                row.CreateCell(2).SetCellValue(record.SupplierName ?? "");
                row.CreateCell(3).SetCellValue(record.SKUCode ?? "");
                row.CreateCell(4).SetCellValue(record.Email ?? "");
                row.CreateCell(5).SetCellValue(record.Phone ?? "");
                row.CreateCell(6).SetCellValue(record.Description ?? "");
                row.CreateCell(7).SetCellValue(record.IsActive ? "Active" : "Inactive");
            }

            // Auto-size columns
            for (int i = 0; i < headers.Length; i++)
            {
                sheet.AutoSizeColumn(i);
            }
        }

        /// <summary>
        /// Create duplicates sheet
        /// </summary>
        private void CreateDuplicatesSheet(NPOI.SS.UserModel.ISheet sheet, List<DuplicateInfo> duplicates)
        {
            var headerStyle = sheet.Workbook.CreateCellStyle();
            var headerFont = sheet.Workbook.CreateFont();
            headerFont.IsBold = true;
            headerFont.Color = NPOI.HSSF.Util.HSSFColor.White.Index;
            headerStyle.SetFont(headerFont);
            headerStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Orange.Index;
            headerStyle.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;

            // Header row
            var headerRow = sheet.CreateRow(0);
            string[] headers = { "Field", "Value", "Row Numbers", "Count", "Type" };
            
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = headerRow.CreateCell(i);
                cell.SetCellValue(headers[i]);
                cell.CellStyle = headerStyle;
            }

            // Data rows
            int rowIndex = 1;
            foreach (var duplicate in duplicates)
            {
                var row = sheet.CreateRow(rowIndex++);
                row.CreateCell(0).SetCellValue(duplicate.Field ?? "");
                row.CreateCell(1).SetCellValue(duplicate.Value ?? "");
                row.CreateCell(2).SetCellValue(string.Join(", ", duplicate.RowNumbers));
                row.CreateCell(3).SetCellValue(duplicate.Count);
                row.CreateCell(4).SetCellValue(duplicate.Type ?? "");
            }

            // Auto-size columns
            for (int i = 0; i < headers.Length; i++)
            {
                sheet.AutoSizeColumn(i);
            }
        }

        /// <summary>
        /// Get current user ID from session or claims
        /// </summary>
        /// <returns>User ID or null if not authenticated</returns>
        private int? GetCurrentUserId()
        {
            try
            {
                // Implement based on your authentication system
                // This could be from JWT claims, session, etc.
                
                // Example using session (adjust based on your implementation)
                var userIdString = HttpContext.Session.GetString("UserId");
                if (int.TryParse(userIdString, out int userId))
                {
                    return userId;
                }

                // Example using claims (if using JWT or similar)
                var userIdClaim = HttpContext.User.FindFirst("UserId")?.Value;
                if (int.TryParse(userIdClaim, out int claimUserId))
                {
                    return claimUserId;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting current user ID");
                return null;
            }
        }
    }
}
