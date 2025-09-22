using GoodsEnterprise.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
                // This would generate an Excel file with the import results
                // For now, return a placeholder response
                return Ok(new
                {
                    success = false,
                    message = "Download functionality not yet implemented."
                });
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
