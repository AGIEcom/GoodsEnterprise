using GoodsEnterprise.Model.Models;
using GoodsEnterprise.DataAccess.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace GoodsEnterprise.Web.Services
{
    /// <summary>
    /// Service for importing promotion costs from Excel files
    /// </summary>
    public class PromotionCostImportService : IPromotionCostImportService
    {
        private readonly IExcelReaderService _excelReaderService;
        private readonly IPromotionCostValidationService _validationService;
        private readonly IGeneralRepository<PromotionCost> _promotionCostRepository;
        private readonly ILogger<PromotionCostImportService> _logger;
        private readonly IWebHostEnvironment _environment;
        private PromotionCostImportConfig _config;
        private readonly IGeneralRepository<Supplier> _supplierRepository;
        private readonly IGeneralRepository<Product> _productRepository;


        // In-memory storage for import progress (in production, use Redis or database)
        private static readonly ConcurrentDictionary<string, ImportProgress> _importProgress = new ConcurrentDictionary<string, ImportProgress>();
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _cancellationTokens = new ConcurrentDictionary<string, CancellationTokenSource>();

        public PromotionCostImportService(
            IExcelReaderService excelReaderService,
            IPromotionCostValidationService validationService,
            IGeneralRepository<PromotionCost> promotionCostRepository,
            ILogger<PromotionCostImportService> logger,
            IWebHostEnvironment environment,
             IGeneralRepository<Supplier> supplierRepository,
              IGeneralRepository<Product> productRepository)
        {
            _excelReaderService = excelReaderService;
            _validationService = validationService;
            _promotionCostRepository = promotionCostRepository;
            _supplierRepository = supplierRepository;
            _productRepository = productRepository;
            _logger = logger;
            _environment = environment;
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            try
            {
                var configPath = Path.Combine(_environment.WebRootPath, "config", "promotioncost-import-columns.json");
                if (File.Exists(configPath))
                {
                    var jsonContent = File.ReadAllText(configPath);
                    _config = JsonSerializer.Deserialize<PromotionCostImportConfig>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else
                {
                    _logger.LogWarning("Promotion cost import configuration file not found at: {ConfigPath}", configPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading promotion cost import configuration");
            }
        }

        private List<string> GetRequiredColumns()
        {
            if (_config?.ImportColumns?.columns != null)
            {
                return _config.ImportColumns.columns.Where(c => c.required).Select(c => c.name).ToList();
            }
            return new List<string> { "ProductCode", "PromotionCost", "StartDate", "EndDate" };
        }

        private List<string> GetValidColumns()
        {
            if (_config?.ImportColumns?.columns != null)
            {
                return _config.ImportColumns.columns.Select(c => c.name).ToList();
            }
            
            return new List<string>
            {
                "ProductCode", "ProductName", "PromotionCost", "StartDate", "EndDate", 
                "SupplierName", "Notes", "IsActive", "CreatedDate"
            };
        }

        public async Task<PromotionCostImportResult> ImportPromotionCostsAsync(IFormFile file, bool validateData = true, int? userId = null)
        {
            var result = new PromotionCostImportResult
            {
                StartTime = DateTime.Now,
                Status = "Processing"
            };

            var cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokens[result.ImportId] = cancellationTokenSource;

            try
            {
                _logger.LogInformation("Starting promotion cost import. ImportId: {ImportId}, File: {FileName}", 
                    result.ImportId, file.FileName);

                // Initialize progress tracking
                var progress = new ImportProgress
                {
                    ImportId = result.ImportId,
                    Status = "Processing",
                    StartTime = result.StartTime,
                    CurrentOperation = "Reading Excel file..."
                };
                _importProgress[result.ImportId] = progress;

                // Step 1: Read Excel file
                var excelResult = await _excelReaderService.ReadFromExcelAsync<PromotionCostImport>(file, "PromotionCost");
                if (!excelResult.IsSuccess)
                {
                    result.GlobalErrors.AddRange(excelResult.Errors);
                    result.Status = "Failed";
                    progress.Status = "Failed";
                    return result;
                }

                result.TotalRecords = excelResult.TotalRows;
                progress.TotalRecords = result.TotalRecords;
                progress.CurrentOperation = "Validating data...";

                // Step 2: Validate data if requested
                PromotionCostBatchValidationResult<PromotionCostImport> validationResult = null;
                if (validateData)
                {
                    validationResult = await _validationService.ValidatePromotionCostsAsync(excelResult.Data);
                    result.GlobalErrors.AddRange(validationResult.GlobalErrors);
                    result.GlobalWarnings.AddRange(validationResult.GlobalWarnings);
                    result.Duplicates = validationResult.Duplicates;
                }
                else
                {
                    // If not validating, assume all are valid
                    validationResult = new PromotionCostBatchValidationResult<PromotionCostImport>
                    {
                        ValidRecords = excelResult.Data,
                        ValidRecordCount = excelResult.Data.Count,
                        TotalRecords = excelResult.Data.Count
                    };
                }

                progress.CurrentOperation = "Importing to database...";

                // Step 3: Import valid promotion costs to database
                if (validationResult.ValidRecords.Any())
                {
                    var importResults = await ImportValidPromotionCostsAsync(
                        validationResult.ValidRecords, 
                        userId, 
                        progress, 
                        cancellationTokenSource.Token);

                    result.SuccessfulImports = importResults.SuccessCount;
                    result.FailedImports = importResults.FailCount;
                    result.SuccessfulPromotionCosts = importResults.SuccessfulPromotionCosts;
                    result.FailedPromotionCosts.AddRange(importResults.FailedPromotionCosts);
                }

                // Add validation failures to failed promotion costs
                result.FailedPromotionCosts.AddRange(validationResult.InvalidRecords);
                result.FailedImports += validationResult.InvalidRecordCount;

                // Finalize result
                result.EndTime = DateTime.Now;
                result.IsSuccess = result.GlobalErrors.Count == 0 && result.SuccessfulImports > 0;
                result.Status = cancellationTokenSource.Token.IsCancellationRequested ? "Cancelled" : 
                               result.IsSuccess ? "Completed" : "Failed";

                progress.Status = result.Status;
                progress.ProcessedRecords = result.TotalRecords;
                progress.SuccessfulRecords = result.SuccessfulImports;
                progress.FailedRecords = result.FailedImports;

                _logger.LogInformation("Promotion cost import completed. ImportId: {ImportId}, Success: {Success}, Failed: {Failed}", 
                    result.ImportId, result.SuccessfulImports, result.FailedImports);

            }
            catch (OperationCanceledException)
            {
                result.Status = "Cancelled";
                result.EndTime = DateTime.Now;
                _logger.LogInformation("Promotion cost import cancelled. ImportId: {ImportId}", result.ImportId);
            }
            catch (Exception ex)
            {
                result.GlobalErrors.Add($"Import failed: {ex.Message}");
                result.Status = "Failed";
                result.EndTime = DateTime.Now;
                _logger.LogError(ex, "Error during promotion cost import. ImportId: {ImportId}", result.ImportId);
            }
            finally
            {
                _cancellationTokens.TryRemove(result.ImportId, out _);
            }

            return result;
        }

        public async Task<PromotionCostImportPreview> PreviewImportAsync(IFormFile file, bool validateData = true)
        {
            var preview = new PromotionCostImportPreview();

            try
            {
                _logger.LogInformation("Starting import preview for file: {FileName}", file.FileName);

                // Read Excel file
                var excelResult = await _excelReaderService.ReadFromExcelAsync<PromotionCostImport>(file, "PromotionCost");
                
                if (!excelResult.IsSuccess)
                {
                    preview.Errors.AddRange(excelResult.Errors);
                    return preview;
                }

                preview.TotalRecords = excelResult.TotalRows;
                preview.ColumnHeaders = excelResult.ColumnMapping.Keys.ToList();

                // Check for required columns
                var requiredColumns = GetRequiredColumns();
                preview.MissingRequiredColumns = requiredColumns
                    .Where(col => !preview.ColumnHeaders.Contains(col, StringComparer.OrdinalIgnoreCase))
                    .ToList();

                // Check for unrecognized columns
                var validColumns = GetValidColumns();
                
                preview.UnrecognizedColumns = preview.ColumnHeaders
                    .Where(col => !validColumns.Contains(col, StringComparer.OrdinalIgnoreCase))
                    .ToList();

                // Validate data if requested
                if (validateData && preview.MissingRequiredColumns.Count == 0)
                {
                    var validationResult = await _validationService.ValidatePromotionCostsAsync(excelResult.Data);
                    
                    preview.ValidRecords = validationResult.ValidRecordCount;
                    preview.InvalidRecords = validationResult.InvalidRecordCount;
                    preview.Duplicates = validationResult.Duplicates;
                    preview.Warnings.AddRange(validationResult.GlobalWarnings);
                    preview.Errors.AddRange(validationResult.GlobalErrors);

                    // Sample records for preview (first 5 of each)
                    preview.SampleValidRecords = validationResult.ValidRecords.Take(5).ToList();
                    preview.SampleInvalidRecords = validationResult.InvalidRecords.Take(5).ToList();
                }
                else
                {
                    preview.ValidRecords = excelResult.Data.Count;
                    preview.SampleValidRecords = excelResult.Data.Take(5).ToList();
                }

                // Determine if import can proceed
                preview.CanProceedWithImport = preview.MissingRequiredColumns.Count == 0 && 
                                             preview.Errors.Count == 0 && 
                                             preview.ValidRecords > 0;

                // Add warnings for unrecognized columns
                if (preview.UnrecognizedColumns.Any())
                {
                    preview.Warnings.Add($"Unrecognized columns will be ignored: {string.Join(", ", preview.UnrecognizedColumns)}");
                }

                _logger.LogInformation("Import preview completed. Valid: {Valid}, Invalid: {Invalid}", 
                    preview.ValidRecords, preview.InvalidRecords);

            }
            catch (Exception ex)
            {
                preview.Errors.Add($"Preview failed: {ex.Message}");
                _logger.LogError(ex, "Error during import preview");
            }

            return preview;
        }

        public async Task<ImportProgress> GetImportProgressAsync(string importId)
        {
            if (_importProgress.TryGetValue(importId, out var progress))
            {
                return progress;
            }

            return new ImportProgress
            {
                ImportId = importId,
                Status = "Not Found"
            };
        }

        public async Task<bool> CancelImportAsync(string importId)
        {
            if (_cancellationTokens.TryGetValue(importId, out var cancellationTokenSource))
            {
                cancellationTokenSource.Cancel();
                
                if (_importProgress.TryGetValue(importId, out var progress))
                {
                    progress.Status = "Cancelled";
                    progress.CanCancel = false;
                }

                _logger.LogInformation("Import cancellation requested. ImportId: {ImportId}", importId);
                return true;
            }

            return false;
        }

        private async Task<(int SuccessCount, int FailCount, List<PromotionCostImport> SuccessfulPromotionCosts, List<PromotionCostImport> FailedPromotionCosts)> 
            ImportValidPromotionCostsAsync(List<PromotionCostImport> promotionCosts, int? userId, ImportProgress progress, CancellationToken cancellationToken)
        {
            int successCount = 0;
            int failCount = 0;
            var successfulPromotionCosts = new List<PromotionCostImport>();
            var failedPromotionCosts = new List<PromotionCostImport>();

            for (int i = 0; i < promotionCosts.Count; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var promotionCostImport = promotionCosts[i];
                
                try
                {
                    var existingSuppliers = await _supplierRepository.GetAllAsync(filter: x =>( x.Name == promotionCostImport.SupplierName || x.Skucode == promotionCostImport.SupplierName));
                    var existingProduct = await _productRepository.GetAllAsync(filter: x => (x.Code == promotionCostImport.ProductName || x.ProductName == promotionCostImport.ProductName));
                    // Convert PromotionCostImport to PromotionCost entity
                    var promotionCost = new PromotionCost
                    {
                        ProductId = existingProduct.FirstOrDefault().Id,
                        SupplierId = existingSuppliers.FirstOrDefault().Id,
                        PromotionCost1 = promotionCostImport.PromotionCost ?? 0,
                        StartDate = promotionCostImport.StartDate ?? DateTime.Now,
                        EndDate = promotionCostImport.EndDate ?? DateTime.Now.AddDays(30),
                        SelloutStartDate = promotionCostImport.SelloutStartDate ?? DateTime.Now,
                        SelloutEndDate = promotionCostImport.SelloutEndDate ?? DateTime.Now.AddDays(30),
                        BonusDescription= promotionCostImport.BonusDescription,
                        SellOutDescription = promotionCostImport.SellOutDescription,
                        //Notes = promotionCostImport.Notes,
                        IsActive = promotionCostImport.IsActive,
                        CreatedDate = DateTime.Now,
                        CreatedBy = userId,
                        ModifiedDate = DateTime.Now,
                        Modifiedby = userId,
                        IsDelete = false
                    };

                    // Save to database
                    await _promotionCostRepository.InsertAsync(promotionCost);
                    
                    promotionCostImport.Id = promotionCost.PromotionCostId;
                    successfulPromotionCosts.Add(promotionCostImport);
                    successCount++;

                    _logger.LogDebug("Successfully imported promotion cost for product: {Product}", promotionCostImport.ProductName);
                }
                catch (Exception ex)
                {
                    promotionCostImport.ValidationErrors.Add($"Database error: {ex.Message}");
                    promotionCostImport.HasErrors = true;
                    failedPromotionCosts.Add(promotionCostImport);
                    failCount++;

                    _logger.LogError(ex, "Failed to import promotion cost for product: {Product}", promotionCostImport.ProductName);
                }

                // Update progress
                progress.ProcessedRecords = i + 1;
                progress.SuccessfulRecords = successCount;
                progress.FailedRecords = failCount;
                progress.CurrentOperation = $"Importing promotion cost {i + 1} of {promotionCosts.Count}...";

                // Estimate completion time
                if (i > 0)
                {
                    var elapsed = DateTime.Now - progress.StartTime;
                    var avgTimePerRecord = elapsed.TotalMilliseconds / (i + 1);
                    var remainingRecords = promotionCosts.Count - (i + 1);
                    progress.EstimatedCompletion = DateTime.Now.AddMilliseconds(avgTimePerRecord * remainingRecords);
                }
            }

            return (successCount, failCount, successfulPromotionCosts, failedPromotionCosts);
        }
    }

    // Configuration classes for JSON deserialization
    public class PromotionCostImportConfig
    {
        public PromotionCostImportColumns ImportColumns { get; set; }
    }

    public class PromotionCostImportColumns
    {
        public string title { get; set; }
        public string description { get; set; }
        public List<PromotionCostColumnDefinition> columns { get; set; }
        public PromotionCostImportSettings importSettings { get; set; }
        public PromotionCostValidationRules validationRules { get; set; }
    }

    public class PromotionCostColumnDefinition
    {
        public string name { get; set; }
        public string displayName { get; set; }
        public string type { get; set; }
        public bool required { get; set; }
        public int? maxLength { get; set; }
        public string description { get; set; }
        public object defaultValue { get; set; }
        public List<string> acceptedValues { get; set; }
        public decimal? min { get; set; }
        public decimal? max { get; set; }
        public int? precision { get; set; }
        public string format { get; set; }
    }

    public class PromotionCostImportSettings
    {
        public string maxFileSize { get; set; }
        public List<string> supportedFormats { get; set; }
        public int maxRows { get; set; }
        public bool validateData { get; set; }
        public bool skipEmptyRows { get; set; }
        public bool trimWhitespace { get; set; }
    }

    public class PromotionCostValidationRules
    {
        public PromotionCostDuplicateCheckRule duplicateCheck { get; set; }
        public PromotionCostValidationRule requiredFieldValidation { get; set; }
        public PromotionCostValidationRule dataTypeValidation { get; set; }
        public PromotionCostValidationRule lengthValidation { get; set; }
    }

    public class PromotionCostDuplicateCheckRule
    {
        public bool enabled { get; set; }
        public List<string> checkFields { get; set; }
        public string action { get; set; }
    }

    public class PromotionCostValidationRule
    {
        public bool enabled { get; set; }
        public string action { get; set; }
    }
}
