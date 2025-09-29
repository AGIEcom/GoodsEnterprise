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
    /// Service for importing products from Excel files
    /// </summary>
    public class ProductImportService : IProductImportService
    {
        private readonly IExcelReaderService _excelReaderService;
        private readonly IProductValidationService _validationService;
        private readonly IGeneralRepository<Product> _productRepository;
        private readonly ILogger<ProductImportService> _logger;
        private readonly IWebHostEnvironment _environment;
        private ProductImportConfig _config;
        
        // In-memory storage for import progress (in production, use Redis or database)
        private static readonly ConcurrentDictionary<string, ImportProgress> _importProgress = new ConcurrentDictionary<string, ImportProgress>();
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _cancellationTokens = new ConcurrentDictionary<string, CancellationTokenSource>();

        public ProductImportService(
            IExcelReaderService excelReaderService,
            IProductValidationService validationService,
            IGeneralRepository<Product> productRepository,
            ILogger<ProductImportService> logger,
            IWebHostEnvironment environment)
        {
            _excelReaderService = excelReaderService;
            _validationService = validationService;
            _productRepository = productRepository;
            _logger = logger;
            _environment = environment;
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            try
            {
                var configPath = Path.Combine(_environment.WebRootPath, "config", "product-import-columns.json");
                if (File.Exists(configPath))
                {
                    var jsonContent = File.ReadAllText(configPath);
                    _config = JsonSerializer.Deserialize<ProductImportConfig>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else
                {
                    _logger.LogWarning("Product import configuration file not found at: {ConfigPath}", configPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading product import configuration");
            }
        }

        private List<string> GetRequiredColumns()
        {
            if (_config?.ImportColumns?.columns != null)
            {
                return _config.ImportColumns.columns.Where(c => c.required).Select(c => c.name).ToList();
            }
            return new List<string> { "Code", "ProductName" };
        }

        private List<string> GetValidColumns()
        {
            if (_config?.ImportColumns?.columns != null)
            {
                return _config.ImportColumns.columns.Select(c => c.name).ToList();
            }
            
            return new List<string>
            {
                "Code", "ProductName", "BrandName", "CategoryName", "SubCategoryName", "Description",
                "UnitPrice", "CostPrice", "Weight", "Dimensions", "Color", "Size", "Material",
                "Manufacturer", "CountryOfOrigin", "Barcode", "SKU", "StockQuantity", "MinStockLevel",
                "MaxStockLevel", "IsActive", "Tags", "ImageUrl"
            };
        }

        public async Task<ProductImportResult> ImportProductsAsync(IFormFile file, bool validateData = true, int? userId = null)
        {
            var result = new ProductImportResult
            {
                StartTime = DateTime.Now,
                Status = "Processing"
            };

            var cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokens[result.ImportId] = cancellationTokenSource;

            try
            {
                _logger.LogInformation("Starting product import. ImportId: {ImportId}, File: {FileName}", 
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
                var excelResult = await _excelReaderService.ReadFromExcelAsync<ProductImport>(file, "Product");
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
                BatchValidationResult<ProductImport> validationResult = null;
                if (validateData)
                {
                    validationResult = await _validationService.ValidateProductsAsync(excelResult.Data);
                    result.GlobalErrors.AddRange(validationResult.GlobalErrors);
                    result.GlobalWarnings.AddRange(validationResult.GlobalWarnings);
                    result.Duplicates = validationResult.Duplicates;
                }
                else
                {
                    // If not validating, assume all are valid
                    validationResult = new BatchValidationResult<ProductImport>
                    {
                        ValidRecords = excelResult.Data,
                        ValidRecordCount = excelResult.Data.Count,
                        TotalRecords = excelResult.Data.Count
                    };
                }

                progress.CurrentOperation = "Importing to database...";

                // Step 3: Import valid products to database
                if (validationResult.ValidRecords.Any())
                {
                    var importResults = await ImportValidProductsAsync(
                        validationResult.ValidRecords, 
                        userId, 
                        progress, 
                        cancellationTokenSource.Token);

                    result.SuccessfulImports = importResults.SuccessCount;
                    result.FailedImports = importResults.FailCount;
                    result.SuccessfulProducts = importResults.SuccessfulProducts;
                    result.FailedProducts.AddRange(importResults.FailedProducts);
                }

                // Add validation failures to failed products
                result.FailedProducts.AddRange(validationResult.InvalidRecords);
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

                _logger.LogInformation("Product import completed. ImportId: {ImportId}, Success: {Success}, Failed: {Failed}", 
                    result.ImportId, result.SuccessfulImports, result.FailedImports);

            }
            catch (OperationCanceledException)
            {
                result.Status = "Cancelled";
                result.EndTime = DateTime.Now;
                _logger.LogInformation("Product import cancelled. ImportId: {ImportId}", result.ImportId);
            }
            catch (Exception ex)
            {
                result.GlobalErrors.Add($"Import failed: {ex.Message}");
                result.Status = "Failed";
                result.EndTime = DateTime.Now;
                _logger.LogError(ex, "Error during product import. ImportId: {ImportId}", result.ImportId);
            }
            finally
            {
                _cancellationTokens.TryRemove(result.ImportId, out _);
            }

            return result;
        }

        public async Task<ProductImportPreview> PreviewImportAsync(IFormFile file, bool validateData = true)
        {
            var preview = new ProductImportPreview();

            try
            {
                _logger.LogInformation("Starting import preview for file: {FileName}", file.FileName);

                // Read Excel file
                var excelResult = await _excelReaderService.ReadFromExcelAsync<ProductImport>(file, "Product");
                
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
                    var validationResult = await _validationService.ValidateProductsAsync(excelResult.Data);
                    
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

        private async Task<(int SuccessCount, int FailCount, List<ProductImport> SuccessfulProducts, List<ProductImport> FailedProducts)> 
            ImportValidProductsAsync(List<ProductImport> products, int? userId, ImportProgress progress, CancellationToken cancellationToken)
        {
            int successCount = 0;
            int failCount = 0;
            var successfulProducts = new List<ProductImport>();
            var failedProducts = new List<ProductImport>();

            for (int i = 0; i < products.Count; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var productImport = products[i];
                
                try
                {
                    // Convert ProductImport to Product entity
                    var product = new Product
                    {
                        Code = productImport.Code,
                        ProductName = productImport.ProductName,
                        //BrandName = productImport.BrandName,
                        //CategoryName = productImport.CategoryName,
                        //SubCategoryName = productImport.SubCategoryName,
                        //Description = productImport.ProductDescription,
                        //UnitPrice = productImport.UnitPrice,
                        //CostPrice = productImport.CostPrice,
                        //Weight = productImport.Weight,
                        //Dimensions = productImport.Dimensions,
                        //Color = productImport.Color,
                        //Size = productImport.Size,
                        //Material = productImport.Material,
                        //Manufacturer = productImport.Manufacturer,
                        //CountryOfOrigin = productImport.CountryOfOrigin,
                        //Barcode = productImport.Barcode,
                        //SKU = productImport.SKU,
                        //StockQuantity = productImport.StockQuantity,
                        //MinStockLevel = productImport.MinStockLevel,
                        //MaxStockLevel = productImport.MaxStockLevel,
                        //IsActive = productImport.IsActive ?? true,
                        //Tags = productImport.Tags,
                        //ImageUrl = productImport.ImageUrl,
                        CreatedDate = DateTime.Now,
                        Createdby = userId,
                        ModifiedDate = DateTime.Now,
                        Modifiedby = userId,
                        IsDelete = false
                    };

                    // Save to database
                    await _productRepository.InsertAsync(product);
                    
                    productImport.Id = product.Id;
                    successfulProducts.Add(productImport);
                    successCount++;

                    _logger.LogDebug("Successfully imported product: {Code}", productImport.Code);
                }
                catch (Exception ex)
                {
                    productImport.ValidationErrors.Add($"Database error: {ex.Message}");
                    productImport.HasErrors = true;
                    failedProducts.Add(productImport);
                    failCount++;

                    _logger.LogError(ex, "Failed to import product: {Code}", productImport.Code);
                }

                // Update progress
                progress.ProcessedRecords = i + 1;
                progress.SuccessfulRecords = successCount;
                progress.FailedRecords = failCount;
                progress.CurrentOperation = $"Importing product {i + 1} of {products.Count}...";

                // Estimate completion time
                if (i > 0)
                {
                    var elapsed = DateTime.Now - progress.StartTime;
                    var avgTimePerRecord = elapsed.TotalMilliseconds / (i + 1);
                    var remainingRecords = products.Count - (i + 1);
                    progress.EstimatedCompletion = DateTime.Now.AddMilliseconds(avgTimePerRecord * remainingRecords);
                }
            }

            return (successCount, failCount, successfulProducts, failedProducts);
        }
    }

    // Configuration classes for JSON deserialization
    public class ProductImportConfig
    {
        public ProductImportColumns ImportColumns { get; set; }
    }

    public class ProductImportColumns
    {
        public string title { get; set; }
        public string description { get; set; }
        public List<ProductColumnDefinition> columns { get; set; }
        public ProductImportSettings importSettings { get; set; }
        public ProductValidationRules validationRules { get; set; }
    }

    public class ProductColumnDefinition
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

    public class ProductImportSettings
    {
        public string maxFileSize { get; set; }
        public List<string> supportedFormats { get; set; }
        public int maxRows { get; set; }
        public bool validateData { get; set; }
        public bool skipEmptyRows { get; set; }
        public bool trimWhitespace { get; set; }
    }

    public class ProductValidationRules
    {
        public ProductDuplicateCheckRule duplicateCheck { get; set; }
        public ProductValidationRule requiredFieldValidation { get; set; }
        public ProductValidationRule dataTypeValidation { get; set; }
        public ProductValidationRule lengthValidation { get; set; }
    }

    public class ProductDuplicateCheckRule
    {
        public bool enabled { get; set; }
        public List<string> checkFields { get; set; }
        public string action { get; set; }
    }

    public class ProductValidationRule
    {
        public bool enabled { get; set; }
        public string action { get; set; }
    }
}
