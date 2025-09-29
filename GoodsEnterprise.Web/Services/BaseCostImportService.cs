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
    /// Service for importing base costs from Excel files
    /// </summary>
    public class BaseCostImportService : IBaseCostImportService
    {
        private readonly IExcelReaderService _excelReaderService;
        private readonly IBaseCostValidationService _validationService;
        private readonly IGeneralRepository<BaseCost> _baseCostRepository;
        private readonly ILogger<BaseCostImportService> _logger;
        private readonly IWebHostEnvironment _environment;
        private BaseCostImportConfig _config;
        private readonly IGeneralRepository<Supplier> _supplierRepository;
        private readonly IGeneralRepository<Product> _productRepository;


        // In-memory storage for import progress (in production, use Redis or database)
        private static readonly ConcurrentDictionary<string, ImportProgress> _importProgress = new ConcurrentDictionary<string, ImportProgress>();
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _cancellationTokens = new ConcurrentDictionary<string, CancellationTokenSource>();

        public BaseCostImportService(
            IExcelReaderService excelReaderService,
            IBaseCostValidationService validationService,
            IGeneralRepository<BaseCost> baseCostRepository,
            ILogger<BaseCostImportService> logger,
            IWebHostEnvironment environment,
             IGeneralRepository<Supplier> supplierRepository,
              IGeneralRepository<Product> productRepository)
        {
            _excelReaderService = excelReaderService;
            _validationService = validationService;
            _baseCostRepository = baseCostRepository;
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
                var configPath = Path.Combine(_environment.WebRootPath, "config", "basecost-import-columns.json");
                if (File.Exists(configPath))
                {
                    var jsonContent = File.ReadAllText(configPath);
                    _config = JsonSerializer.Deserialize<BaseCostImportConfig>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else
                {
                    _logger.LogWarning("Base cost import configuration file not found at: {ConfigPath}", configPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading base cost import configuration");
            }
        }

        private List<string> GetRequiredColumns()
        {
            if (_config?.ImportColumns?.columns != null)
            {
                return _config.ImportColumns.columns.Where(c => c.required).Select(c => c.name).ToList();
            }
            return new List<string> { "ProductName", "BaseCost", "StartDate" };
        }

        private List<string> GetValidColumns()
        {
            if (_config?.ImportColumns?.columns != null)
            {
                return _config.ImportColumns.columns.Select(c => c.name).ToList();
            }
            
            return new List<string>
            {
                "ProductName", "BaseCost", "StartDate", "EndDate", "SupplierName", 
                "Notes", "IsActive", "CreatedDate"
            };
        }

        public async Task<BaseCostImportResult> ImportBaseCostsAsync(IFormFile file, bool validateData = true, int? userId = null)
        {
            var result = new BaseCostImportResult
            {
                StartTime = DateTime.Now,
                Status = "Processing"
            };

            var cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokens[result.ImportId] = cancellationTokenSource;

            try
            {
                _logger.LogInformation("Starting base cost import. ImportId: {ImportId}, File: {FileName}", 
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
                var excelResult = await _excelReaderService.ReadFromExcelAsync<BaseCostImport>(file, "BaseCost");
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
                BatchValidationResult<BaseCostImport> validationResult = null;
                if (validateData)
                {
                    validationResult = await _validationService.ValidateBaseCostsAsync(excelResult.Data);
                    result.GlobalErrors.AddRange(validationResult.GlobalErrors);
                    result.GlobalWarnings.AddRange(validationResult.GlobalWarnings);
                    result.Duplicates = validationResult.Duplicates;
                }
                else
                {
                    // If not validating, assume all are valid
                    validationResult = new BatchValidationResult<BaseCostImport>
                    {
                        ValidRecords = excelResult.Data,
                        ValidRecordCount = excelResult.Data.Count,
                        TotalRecords = excelResult.Data.Count
                    };
                }

                progress.CurrentOperation = "Importing to database...";

                // Step 3: Import valid base costs to database
                if (validationResult.ValidRecords.Any())
                {
                    var importResults = await ImportValidBaseCostsAsync(
                        validationResult.ValidRecords, 
                        userId, 
                        progress, 
                        cancellationTokenSource.Token);

                    result.SuccessfulImports = importResults.SuccessCount;
                    result.FailedImports = importResults.FailCount;
                    result.SuccessfulBaseCosts = importResults.SuccessfulBaseCosts;
                    result.FailedBaseCosts.AddRange(importResults.FailedBaseCosts);
                }

                // Add validation failures to failed base costs
                result.FailedBaseCosts.AddRange(validationResult.InvalidRecords);
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

                _logger.LogInformation("Base cost import completed. ImportId: {ImportId}, Success: {Success}, Failed: {Failed}", 
                    result.ImportId, result.SuccessfulImports, result.FailedImports);

            }
            catch (OperationCanceledException)
            {
                result.Status = "Cancelled";
                result.EndTime = DateTime.Now;
                _logger.LogInformation("Base cost import cancelled. ImportId: {ImportId}", result.ImportId);
            }
            catch (Exception ex)
            {
                result.GlobalErrors.Add($"Import failed: {ex.Message}");
                result.Status = "Failed";
                result.EndTime = DateTime.Now;
                _logger.LogError(ex, "Error during base cost import. ImportId: {ImportId}", result.ImportId);
            }
            finally
            {
                _cancellationTokens.TryRemove(result.ImportId, out _);
            }

            return result;
        }

        public async Task<BaseCostImportPreview> PreviewImportAsync(IFormFile file, bool validateData = true)
        {
            var preview = new BaseCostImportPreview();

            try
            {
                _logger.LogInformation("Starting import preview for file: {FileName}", file.FileName);

                // Read Excel file
                var excelResult = await _excelReaderService.ReadFromExcelAsync<BaseCostImport>(file, "BaseCost");
                
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
                    var validationResult = await _validationService.ValidateBaseCostsAsync(excelResult.Data);
                    
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

        private async Task<(int SuccessCount, int FailCount, List<BaseCostImport> SuccessfulBaseCosts, List<BaseCostImport> FailedBaseCosts)> 
            ImportValidBaseCostsAsync(List<BaseCostImport> baseCosts, int? userId, ImportProgress progress, CancellationToken cancellationToken)
        {
            int successCount = 0;
            int failCount = 0;
            var successfulBaseCosts = new List<BaseCostImport>();
            var failedBaseCosts = new List<BaseCostImport>();

            for (int i = 0; i < baseCosts.Count; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var baseCostImport = baseCosts[i];
                
                try
                {
                    var existingSuppliers = await _supplierRepository.GetAllAsync(filter: x => x.Name == baseCostImport.SupplierName);
                    var existingProduct = await _productRepository.GetAllAsync(filter: x => (x.Code == baseCostImport.ProductName || x.ProductName== baseCostImport.ProductName));

                    // Convert BaseCostImport to BaseCost entity
                    var baseCost = new BaseCost
                    {
                        ProductId = existingProduct.FirstOrDefault().Id,
                        SupplierId = existingSuppliers.FirstOrDefault().Id,
                        BaseCost1 = baseCostImport.BaseCost ?? 0,
                        StartDate = baseCostImport.StartDate ?? DateTime.Now,
                        EndDate = baseCostImport.EndDate, 
                        IsActive = baseCostImport.IsActive ,
                        CreatedDate = DateTime.Now,
                        CreatedBy = userId,
                        ModifiedDate = DateTime.Now,
                        Modifiedby = userId,
                        IsDelete = false
                    };

                    // Save to database
                    await _baseCostRepository.InsertAsync(baseCost);
                    
                    baseCostImport.Id = baseCost.BaseCostId;
                    successfulBaseCosts.Add(baseCostImport);
                    successCount++;

                    _logger.LogDebug("Successfully imported base cost for product: {ProductName}", baseCostImport.ProductName);
                }
                catch (Exception ex)
                {
                    baseCostImport.ValidationErrors.Add($"Database error: {ex.Message}");
                    baseCostImport.HasErrors = true;
                    failedBaseCosts.Add(baseCostImport);
                    failCount++;

                    _logger.LogError(ex, "Failed to import base cost for product: {ProductName}", baseCostImport.ProductName);
                }

                // Update progress
                progress.ProcessedRecords = i + 1;
                progress.SuccessfulRecords = successCount;
                progress.FailedRecords = failCount;
                progress.CurrentOperation = $"Importing base cost {i + 1} of {baseCosts.Count}...";

                // Estimate completion time
                if (i > 0)
                {
                    var elapsed = DateTime.Now - progress.StartTime;
                    var avgTimePerRecord = elapsed.TotalMilliseconds / (i + 1);
                    var remainingRecords = baseCosts.Count - (i + 1);
                    progress.EstimatedCompletion = DateTime.Now.AddMilliseconds(avgTimePerRecord * remainingRecords);
                }
            }

            return (successCount, failCount, successfulBaseCosts, failedBaseCosts);
        }
    }

    // Configuration classes for JSON deserialization
    public class BaseCostImportConfig
    {
        public BaseCostImportColumns ImportColumns { get; set; }
    }

    public class BaseCostImportColumns
    {
        public string title { get; set; }
        public string description { get; set; }
        public List<BaseCostColumnDefinition> columns { get; set; }
        public BaseCostImportSettings importSettings { get; set; }
        public BaseCostValidationRules validationRules { get; set; }
    }

    public class BaseCostColumnDefinition
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

    public class BaseCostImportSettings
    {
        public string maxFileSize { get; set; }
        public List<string> supportedFormats { get; set; }
        public int maxRows { get; set; }
        public bool validateData { get; set; }
        public bool skipEmptyRows { get; set; }
        public bool trimWhitespace { get; set; }
    }

    public class BaseCostValidationRules
    {
        public BaseCostDuplicateCheckRule duplicateCheck { get; set; }
        public BaseCostValidationRule requiredFieldValidation { get; set; }
        public BaseCostValidationRule dataTypeValidation { get; set; }
        public BaseCostValidationRule lengthValidation { get; set; }
    }

    public class BaseCostDuplicateCheckRule
    {
        public bool enabled { get; set; }
        public List<string> checkFields { get; set; }
        public string action { get; set; }
    }

    public class BaseCostValidationRule
    {
        public bool enabled { get; set; }
        public string action { get; set; }
    }
}
