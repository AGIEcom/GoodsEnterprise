using GoodsEnterprise.Web.Services;
using GoodsEnterprise.Model.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoodsEnterprise.Web.Controllers
{
    /// <summary>
    /// API Controller for promotion cost import operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PromotionCostImportController : ControllerBase
    {
        private readonly IPromotionCostImportService _importService;
        private readonly ILogger<PromotionCostImportController> _logger;

        public PromotionCostImportController(
            IPromotionCostImportService importService,
            ILogger<PromotionCostImportController> logger)
        {
            _importService = importService;
            _logger = logger;
        }

        /// <summary>
        /// Preview promotion cost import data from Excel file
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

                _logger.LogInformation("PromotionCost preview import request received. File: {FileName}, Size: {FileSize}", 
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
                _logger.LogError(ex, "Error during promotion cost import preview");
                return StatusCode(500, new { 
                    error = "An error occurred while previewing the import.", 
                    details = ex.Message 
                });
            }
        }

        /// <summary>
        /// Start promotion cost import process
        /// </summary>
        /// <param name="file">Excel file to import</param>
        /// <param name="validateData">Whether to perform validation (default: true)</param>
        /// <returns>Import result</returns>
        [HttpPost("import")]
        public async Task<IActionResult> ImportPromotionCosts(
            IFormFile file,
            [FromForm] bool validateData = true)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { error = "No file provided or file is empty." });
                }

                // Get user ID from session or claims
                int? userId = GetCurrentUserId();

                _logger.LogInformation("PromotionCost import request received. File: {FileName}, Size: {FileSize}, UserId: {UserId}", 
                    file.FileName, file.Length, userId);

                var result = await _importService.ImportPromotionCostsAsync(file, validateData, userId);

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
                        successfulPromotionCosts = result.SuccessfulPromotionCosts.Take(10).Select(pc => new
                        {
                            rowNumber = pc.RowNumber,
                            SupplierName = pc.SupplierName,
                            productName = pc.ProductName,
                            promotionCost = pc.PromotionCost,
                            startDate = pc.StartDate,
                            endDate = pc.EndDate,
                            id = pc.Id
                        }),
                        failedPromotionCosts = result.FailedPromotionCosts.Take(10).Select(pc => new
                        {
                            rowNumber = pc.RowNumber,
                            SupplierName = pc.SupplierName,
                            productName = pc.ProductName,
                            promotionCost = pc.PromotionCost,
                            errors = pc.ValidationErrors,
                            warnings = pc.ValidationWarnings
                        }),
                        duplicates = result.Duplicates.Take(10)
                    },
                    errors = result.GlobalErrors,
                    warnings = result.GlobalWarnings,
                    message = result.IsSuccess 
                        ? $"Successfully imported {result.SuccessfulImports} promotion costs." 
                        : $"Import completed with {result.FailedImports} failures."
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during promotion cost import");
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
        /// Get current user ID from session or claims
        /// </summary>
        /// <returns>User ID or null if not authenticated</returns>
        private int? GetCurrentUserId()
        {
            try
            {
                // Example using session
                var userIdString = HttpContext.Session.GetString("UserId");
                if (int.TryParse(userIdString, out int userId))
                {
                    return userId;
                }

                // Example using claims
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
