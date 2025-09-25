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
    /// Service for importing suppliers from Excel files
    /// </summary>
    public class SupplierImportService : ISupplierImportService
    {
        private readonly IExcelReaderService _excelReaderService;
        private readonly ISupplierValidationService _validationService;
        private readonly IGeneralRepository<Supplier> _supplierRepository;
        private readonly ILogger<SupplierImportService> _logger;
        private readonly IWebHostEnvironment _environment;
        private SupplierImportConfig _config;
        
        // In-memory storage for import progress (in production, use Redis or database)
        private static readonly ConcurrentDictionary<string, ImportProgress> _importProgress = new ConcurrentDictionary<string, ImportProgress>();
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _cancellationTokens = new ConcurrentDictionary<string, CancellationTokenSource> ();

        public SupplierImportService(
            IExcelReaderService excelReaderService,
            ISupplierValidationService validationService,
            IGeneralRepository<Supplier> supplierRepository,
            ILogger<SupplierImportService> logger,
            IWebHostEnvironment environment)
        {
            _excelReaderService = excelReaderService;
            _validationService = validationService;
            _supplierRepository = supplierRepository;
            _logger = logger;
            _environment = environment;
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            try
            {
                var configPath = Path.Combine(_environment.WebRootPath, "config", "supplier-import-columns.json");
                if (File.Exists(configPath))
                {
                    var jsonContent = File.ReadAllText(configPath);
                    _config = JsonSerializer.Deserialize<SupplierImportConfig>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else
                {
                    _logger.LogWarning("Supplier import configuration file not found at: {ConfigPath}", configPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading supplier import configuration");
            }
        }

        private List<string> GetRequiredColumns()
        {
            return _config?.ImportColumns?.RequiredColumns?.Select(c => c.Name).ToList() 
                   ?? new List<string> { "SupplierName", "SKUCode", "Email" };
        }

        private List<string> GetValidColumns()
        {
            if (_config?.ImportColumns == null)
            {
                return new List<string>
                {
                    "SupplierName", "SKUCode", "Email", "FirstName", "LastName", "Phone",
                    "Address1", "Address2", "Description", "IsActive", "IsPreferred",
                    "LeadTimeDays", "MoqCase", "LastCost", "Incoterm", "ValidFrom", "ValidTo"
                };
            }

            var allColumns = new List<string>();
            allColumns.AddRange(_config.ImportColumns.RequiredColumns?.Select(c => c.Name) ?? new List<string>());
            allColumns.AddRange(_config.ImportColumns.OptionalColumns?.Select(c => c.Name) ?? new List<string>());
            return allColumns;
        }

        public async Task<SupplierImportResult> ImportSuppliersAsync(IFormFile file, bool validateData = true, int? userId = null)
        {
            var result = new SupplierImportResult
            {
                StartTime = DateTime.Now,
                Status = "Processing"
            };

            var cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokens[result.ImportId] = cancellationTokenSource;

            try
            {
                _logger.LogInformation("Starting supplier import. ImportId: {ImportId}, File: {FileName}", 
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
                var excelResult = await _excelReaderService.ReadSuppliersFromExcelAsync(file);
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
                BatchValidationResult validationResult = null;
                if (validateData)
                {
                    validationResult = await _validationService.ValidateSuppliersAsync(excelResult.Data);
                    result.GlobalErrors.AddRange(validationResult.GlobalErrors);
                    result.GlobalWarnings.AddRange(validationResult.GlobalWarnings);
                    result.Duplicates = validationResult.Duplicates;
                }
                else
                {
                    // If not validating, assume all are valid
                    validationResult = new BatchValidationResult
                    {
                        ValidSuppliers = excelResult.Data,
                        ValidRecords = excelResult.Data.Count,
                        TotalRecords = excelResult.Data.Count
                    };
                }

                progress.CurrentOperation = "Importing to database...";

                // Step 3: Import valid suppliers to database
                if (validationResult.ValidSuppliers.Any())
                {
                    var importResults = await ImportValidSuppliersAsync(
                        validationResult.ValidSuppliers, 
                        userId, 
                        progress, 
                        cancellationTokenSource.Token);

                    result.SuccessfulImports = importResults.SuccessCount;
                    result.FailedImports = importResults.FailCount;
                    result.SuccessfulSuppliers = importResults.SuccessfulSuppliers;
                    result.FailedSuppliers.AddRange(importResults.FailedSuppliers);
                }

                // Add validation failures to failed suppliers
                result.FailedSuppliers.AddRange(validationResult.InvalidSuppliers);
                result.FailedImports += validationResult.InvalidRecords;

                // Finalize result
                result.EndTime = DateTime.Now;
                result.IsSuccess = result.GlobalErrors.Count == 0 && result.SuccessfulImports > 0;
                result.Status = cancellationTokenSource.Token.IsCancellationRequested ? "Cancelled" : 
                               result.IsSuccess ? "Completed" : "Failed";

                progress.Status = result.Status;
                progress.ProcessedRecords = result.TotalRecords;
                progress.SuccessfulRecords = result.SuccessfulImports;
                progress.FailedRecords = result.FailedImports;

                _logger.LogInformation("Supplier import completed. ImportId: {ImportId}, Success: {Success}, Failed: {Failed}", 
                    result.ImportId, result.SuccessfulImports, result.FailedImports);

            }
            catch (OperationCanceledException)
            {
                result.Status = "Cancelled";
                result.EndTime = DateTime.Now;
                _logger.LogInformation("Supplier import cancelled. ImportId: {ImportId}", result.ImportId);
            }
            catch (Exception ex)
            {
                result.GlobalErrors.Add($"Import failed: {ex.Message}");
                result.Status = "Failed";
                result.EndTime = DateTime.Now;
                _logger.LogError(ex, "Error during supplier import. ImportId: {ImportId}", result.ImportId);
            }
            finally
            {
                _cancellationTokens.TryRemove(result.ImportId, out _);
            }

            return result;
        }

        public async Task<SupplierImportPreview> PreviewImportAsync(IFormFile file, bool validateData = true)
        {
            var preview = new SupplierImportPreview();

            try
            {
                _logger.LogInformation("Starting import preview for file: {FileName}", file.FileName);

                // Read Excel file
                var excelResult = await _excelReaderService.ReadSuppliersFromExcelAsync(file);
                
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
                    var validationResult = await _validationService.ValidateSuppliersAsync(excelResult.Data);
                    
                    preview.ValidRecords = validationResult.ValidRecords;
                    preview.InvalidRecords = validationResult.InvalidRecords;
                    preview.Duplicates = validationResult.Duplicates;
                    preview.Warnings.AddRange(validationResult.GlobalWarnings);
                    preview.Errors.AddRange(validationResult.GlobalErrors);

                    // Sample records for preview (first 5 of each)
                    preview.SampleValidRecords = validationResult.ValidSuppliers.Take(5).ToList();
                    preview.SampleInvalidRecords = validationResult.InvalidSuppliers.Take(5).ToList();
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

        private async Task<(int SuccessCount, int FailCount, List<SupplierImport> SuccessfulSuppliers, List<SupplierImport> FailedSuppliers)> 
            ImportValidSuppliersAsync(List<SupplierImport> suppliers, int? userId, ImportProgress progress, CancellationToken cancellationToken)
        {
            int successCount = 0;
            int failCount = 0;
            var successfulSuppliers = new List<SupplierImport>();
            var failedSuppliers = new List<SupplierImport>();

            for (int i = 0; i < suppliers.Count; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var supplierImport = suppliers[i];
                
                try
                {
                    // Convert SupplierImport to Supplier entity
                    var supplier = new Supplier
                    {
                        Name = supplierImport.SupplierName,
                        Skucode = supplierImport.SKUCode,
                        Email = supplierImport.Email,
                        FirstName = supplierImport.FirstName,
                        LastName = supplierImport.LastName,
                        Phone = supplierImport.Phone,
                        Address1 = supplierImport.Address1,
                        Address2 = supplierImport.Address2,
                        Description = supplierImport.Description,
                        IsActive = supplierImport.IsActive,
                        IsPreferred = supplierImport.IsPreferred,
                        LeadTimeDays = supplierImport.LeadTimeDays,
                        MoqCase = supplierImport.MoqCase,
                        LastCost = supplierImport.LastCost,
                        Incoterm = supplierImport.Incoterm,
                        ValidFrom = supplierImport.ValidFrom,
                        ValidTo = supplierImport.ValidTo,
                        CreatedDate = DateTime.Now,
                        Createdby = userId,
                        ModifiedDate = DateTime.Now,
                        Modifiedby = userId,
                        IsDelete = false
                    };

                    // Save to database
                    await _supplierRepository.InsertAsync(supplier);
                    
                    supplierImport.Id = supplier.Id;
                    successfulSuppliers.Add(supplierImport);
                    successCount++;

                    _logger.LogDebug("Successfully imported supplier: {SKUCode}", supplierImport.SKUCode);
                }
                catch (Exception ex)
                {
                    supplierImport.ValidationErrors.Add($"Database error: {ex.Message}");
                    supplierImport.HasErrors = true;
                    failedSuppliers.Add(supplierImport);
                    failCount++;

                    _logger.LogError(ex, "Failed to import supplier: {SKUCode}", supplierImport.SKUCode);
                }

                // Update progress
                progress.ProcessedRecords = i + 1;
                progress.SuccessfulRecords = successCount;
                progress.FailedRecords = failCount;
                progress.CurrentOperation = $"Importing supplier {i + 1} of {suppliers.Count}...";

                // Estimate completion time
                if (i > 0)
                {
                    var elapsed = DateTime.Now - progress.StartTime;
                    var avgTimePerRecord = elapsed.TotalMilliseconds / (i + 1);
                    var remainingRecords = suppliers.Count - (i + 1);
                    progress.EstimatedCompletion = DateTime.Now.AddMilliseconds(avgTimePerRecord * remainingRecords);
                }
            }

            return (successCount, failCount, successfulSuppliers, failedSuppliers);
        }
    }

    // Configuration classes for JSON deserialization
    public class SupplierImportConfig
    {
        public SupplierImportColumns ImportColumns { get; set; }
    }

    public class SupplierImportColumns
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<ColumnDefinition> RequiredColumns { get; set; }
        public List<ColumnDefinition> OptionalColumns { get; set; }
        public ImportSettings ImportSettings { get; set; }
        public ValidationRules ValidationRules { get; set; }
    }

    public class ColumnDefinition
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Type { get; set; }
        public bool Required { get; set; }
        public int? MaxLength { get; set; }
        public string Description { get; set; }
        public object DefaultValue { get; set; }
        public List<string> AcceptedValues { get; set; }
        public int? Min { get; set; }
        public int? Max { get; set; }
        public int? Precision { get; set; }
        public string Format { get; set; }
    }

    public class ImportSettings
    {
        public string MaxFileSize { get; set; }
        public List<string> SupportedFormats { get; set; }
        public int MaxRows { get; set; }
        public bool ValidateData { get; set; }
        public bool SkipEmptyRows { get; set; }
        public bool TrimWhitespace { get; set; }
    }

    public class ValidationRules
    {
        public DuplicateCheckRule DuplicateCheck { get; set; }
        public ValidationRule RequiredFieldValidation { get; set; }
        public ValidationRule DataTypeValidation { get; set; }
        public ValidationRule LengthValidation { get; set; }
    }

    public class DuplicateCheckRule
    {
        public bool Enabled { get; set; }
        public List<string> CheckFields { get; set; }
        public string Action { get; set; }
    }

    public class ValidationRule
    {
        public bool Enabled { get; set; }
        public string Action { get; set; }
    }
}
