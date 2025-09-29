using GoodsEnterprise.Model.Models;
using GoodsEnterprise.DataAccess.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Globalization;
using System.Reflection;

namespace GoodsEnterprise.Web.Services
{
    /// <summary>
    /// Service for validating base cost import data
    /// </summary>
    public class BaseCostValidationService : IBaseCostValidationService
    {
        private readonly IGeneralRepository<BaseCost> _baseCostRepository;
        private readonly IGeneralRepository<Product> _productRepository;
        private readonly ILogger<BaseCostValidationService> _logger;
        private readonly IWebHostEnvironment _environment;
        private BaseCostImportConfig _config;

        public BaseCostValidationService(
            IGeneralRepository<BaseCost> baseCostRepository,
            IGeneralRepository<Product> productRepository,
            ILogger<BaseCostValidationService> logger,
            IWebHostEnvironment environment)
        {
            _baseCostRepository = baseCostRepository;
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

        public async Task<BatchValidationResult<BaseCostImport>> ValidateBaseCostsAsync(List<BaseCostImport> baseCosts)
        {
            var result = new BatchValidationResult<BaseCostImport>
            {
                TotalRecords = baseCosts.Count
            };

            try
            {
                // Track duplicates within the batch
                var batchDuplicates = new Dictionary<string, List<int>>();

                for (int i = 0; i < baseCosts.Count; i++)
                {
                    var baseCost = baseCosts[i];
                    baseCost.RowNumber = i + 1;

                    // JSON-based validation
                    var validationResult = await ValidateBaseCostAsync(baseCost);

                    // Business rules validation
                    await ValidateBusinessRules(baseCost, validationResult);

                    // Database duplicate check
                    await CheckDatabaseDuplicates(baseCost, result);

                    // Batch duplicate tracking
                    TrackBatchDuplicates(baseCost, batchDuplicates, i + 1);

                    // Add validation results to import object
                    baseCost.ValidationErrors.AddRange(validationResult.Errors);
                    baseCost.ValidationWarnings.AddRange(validationResult.Warnings);
                    baseCost.HasErrors = validationResult.Errors.Any();

                    // Categorize record
                    if (validationResult.IsValid && !baseCost.HasErrors)
                    {
                        result.ValidRecords.Add(baseCost);
                        result.ValidRecordCount++;
                    }
                    else
                    {
                        result.InvalidRecords.Add(baseCost);
                        result.InvalidRecordCount++;
                    }
                }

                // Process batch duplicates
                ProcessBatchDuplicates(batchDuplicates, result);

                _logger.LogInformation("Base cost validation completed. Valid: {Valid}, Invalid: {Invalid}, Duplicates: {Duplicates}",
                    result.ValidRecordCount, result.InvalidRecordCount, result.Duplicates.Count);
            }
            catch (Exception ex)
            {
                result.GlobalErrors.Add($"Validation error: {ex.Message}");
                _logger.LogError(ex, "Error during base cost validation");
            }

            return result;
        }

 
        public async Task<BaseCostValidationResult> ValidateBaseCostAsync(BaseCostImport baseCost)
        {
            var result = new BaseCostValidationResult();

            // JSON-based validation instead of hardcoded rules
            await ValidateUsingJsonConfig(baseCost, result);
            
            // Additional business logic validation
            await ValidateBusinessRules(baseCost, result);

            result.IsValid = result.Errors.Count == 0;
            return result;
        }

        private async Task ValidateUsingJsonConfig(BaseCostImport baseCost, BaseCostValidationResult result)
        {
            if (_config?.ImportColumns?.columns == null)
            {
                // Fallback to basic validation if config not loaded
                await ValidateFallbackRules(baseCost, result);
                return;
            }

            // Validate all columns from JSON config
            foreach (var column in _config.ImportColumns.columns)
            {
                var value = GetPropertyValue(baseCost, column.name);
                
                // Required field validation
                if (column.required && (value == null || string.IsNullOrWhiteSpace(value.ToString())))
                {
                    var error = $"{column.displayName ?? column.name} is required";
                    result.Errors.Add(error);
                    AddFieldError(result, column.name, error);
                }
                else if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                {
                    // Validate field-specific rules for non-empty fields
                    ValidateFieldRules(baseCost, column, result);
                }
            }
        }

        private void ValidateFieldRules(BaseCostImport baseCost, dynamic column, BaseCostValidationResult result)
        {
            var value = GetPropertyValue(baseCost, column.name);
            if (value == null) return;

            var stringValue = value.ToString();

            // String length validation
            if (column.maxLength != null)
            {
                try
                {
                    var maxLength = Convert.ToInt32(column.maxLength);
                    if (stringValue.Length > maxLength)
                    {
                        var error = $"{column.displayName ?? column.name} cannot exceed {maxLength} characters";
                        result.Errors.Add(error);
                        AddFieldError(result, column.name, error);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error parsing maxLength value for string field: {MaxLength}");
                }
            }

            // Type-specific validation
            switch (column.type?.ToString().ToLower())
            {
                case "decimal":
                    if (!decimal.TryParse(stringValue, out decimal decimalValue))
                    {
                        var error = $"{column.displayName ?? column.name} must be a valid decimal number";
                        result.Errors.Add(error);
                        AddFieldError(result, column.name, error);
                    }
                    else
                    {
                        // Handle minimum value validation
                        if (column.min != null)
                        {
                            try
                            {
                                var minValue = Convert.ToDecimal(column.min);
                                if (decimalValue < minValue)
                                {
                                    var error = $"{column.displayName ?? column.name} must be at least {minValue}";
                                    result.Errors.Add(error);
                                    AddFieldError(result, column.name, error);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error parsing min value for decimal field: {MinValue}"); 
                            }
                        }

                        // Handle maximum value validation
                        if (column.max != null)
                        {
                            try
                            {
                                var maxValue = Convert.ToDecimal(column.max);
                                if (decimalValue > maxValue)
                                {
                                    var error = $"{column.displayName ?? column.name} cannot exceed {maxValue}";
                                    result.Errors.Add(error);
                                    AddFieldError(result, column.name, error);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Error parsing max value for decimal field: {MaxValue}");
                            }
                        }
                    }
                    break;

                case "date":
                    if (!DateTime.TryParse(stringValue, out DateTime dateValue))
                    {
                        var error = $"{column.displayName ?? column.name} must be a valid date";
                        result.Errors.Add(error);
                        AddFieldError(result, column.name, error);
                    }
                    break;

                case "boolean":
                    // Alternative approach using HashSet for better performance and cleaner code
                    var defaultAcceptedValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase) 
                    { 
                        "true", "false", "1", "0", "yes", "no", "active", "inactive" 
                    };
                    
                    var validValues = defaultAcceptedValues;
                    
                    if (column.acceptedValues != null)
                    {
                        try
                        {
                            var customValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                            
                            // Handle different JSON formats more elegantly
                            if (column.acceptedValues is System.Collections.IEnumerable enumerable)
                            {
                                foreach (var item in enumerable)
                                {
                                    var valueStr = item?.ToString()?.Trim();
                                    if (!string.IsNullOrEmpty(valueStr))
                                    {
                                        customValues.Add(valueStr);
                                    }
                                }
                                
                                if (customValues.Count > 0)
                                {
                                    validValues = customValues;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Error parsing acceptedValues for boolean field, using defaults");
                        }
                    }

                    // Case-insensitive validation using HashSet.Contains
                    if (!validValues.Contains(stringValue))
                    {
                        var acceptedList = string.Join(", ", validValues.OrderBy(v => v));
                        var error = $"{column.displayName ?? column.name} must be one of: {acceptedList}";
                        result.Errors.Add(error);
                        AddFieldError(result, column.name, error);
                    }
                    break;
            }
        }

        private async Task ValidateBusinessRules(BaseCostImport baseCost, BaseCostValidationResult result)
        {
            // Product existence check
            if (!string.IsNullOrEmpty(baseCost.ProductName))
            {
                var existingProducts = await _productRepository.GetAllAsync(filter: p => 
                    p.ProductName == baseCost.ProductName || p.Code == baseCost.ProductName);

                if (!existingProducts.Any())
                {
                    result.Errors.Add($"Product '{baseCost.ProductName}' not found in system");
                }
            }
            if (!string.IsNullOrEmpty(baseCost.ProductName) && !string.IsNullOrEmpty(baseCost.SupplierName))
            {
                var existingBaseCosts = await _baseCostRepository.GetAllAsync(filter: p =>
                    (p.Product.ProductName == baseCost.ProductName || p.Product.Code == baseCost.ProductName) && 
                    (p.Supplier.Name == baseCost.SupplierName || p.Supplier.Skucode == baseCost.SupplierName) &&                    
                    p.BaseCost1 == baseCost.BaseCost);

                if (existingBaseCosts.Any())
                {
                    result.Errors.Add($"Base cost record for Product '{baseCost.ProductName}' with Supplier '{baseCost.SupplierName}' already exists for the same date and cost");
                }

                // Check for overlapping date ranges with different costs
                if (baseCost.StartDate.HasValue)
                {
                    var newStartDate = baseCost.StartDate.Value;
                    var newEndDate = baseCost.EndDate ?? DateTime.MaxValue; // If no end date, assume ongoing

                    var overlappingBaseCosts = await _baseCostRepository.GetAllAsync(filter: p =>
                        (p.Product.ProductName == baseCost.ProductName || p.Product.Code == baseCost.ProductName) && 
                        (p.Supplier.Name == baseCost.SupplierName || p.Supplier.Skucode == baseCost.SupplierName) &&
                        p.BaseCost1 != baseCost.BaseCost);

                    // Filter in memory for complex date range overlap logic
                    var conflictingCosts = overlappingBaseCosts.Where(existing => 
                    {
                        var existingStartDate = existing.StartDate ?? DateTime.MinValue;
                        var existingEndDate = existing.EndDate ?? DateTime.MaxValue;

                        // Check if date ranges overlap:
                        // 1. New range starts before existing ends AND new range ends after existing starts
                        // 2. OR new range is completely within existing range
                        // 3. OR existing range is completely within new range
                        // 4. OR ranges have exact same dates
                        return (newStartDate <= existingEndDate && newEndDate >= existingStartDate);
                    }).ToList();

                    if (conflictingCosts.Any())
                    {
                        foreach (var conflictingCost in conflictingCosts)
                        {
                            var existingRange = $"{conflictingCost.StartDate?.ToString("yyyy-MM-dd") ?? "No Start"} to {conflictingCost.EndDate?.ToString("yyyy-MM-dd") ?? "Ongoing"}";
                            var newRange = $"{baseCost.StartDate?.ToString("yyyy-MM-dd")} to {baseCost.EndDate?.ToString("yyyy-MM-dd") ?? "Ongoing"}";
                            
                            result.Errors.Add($"Date range conflict: Product '{baseCost.ProductName}' with Supplier '{baseCost.SupplierName}' already has base cost ${conflictingCost.BaseCost1:F2} for period ({existingRange}), which overlaps with new period ({newRange})");
                        }
                    }
                }

                // Validate base cost range for this product-supplier combination
                if (baseCost.BaseCost.HasValue)
                {
                    var existingCosts = await _baseCostRepository.GetAllAsync(filter: p =>
                        (p.Product.ProductName == baseCost.ProductName || p.Product.Code == baseCost.ProductName) && 
                        (p.Supplier.Name == baseCost.SupplierName || p.Supplier.Skucode == baseCost.SupplierName));

                    if (existingCosts.Any())
                    {
                        var avgCost = existingCosts.Average(c => c.BaseCost1 ?? 0);
                        var maxCost = existingCosts.Max(c => c.BaseCost1 ?? 0);
                        var minCost = existingCosts.Min(c => c.BaseCost1 ?? 0);

                        // Check if new cost is significantly different from historical costs
                        if (baseCost.BaseCost > maxCost * 1.5m)
                        {
                            result.Errors.Add($"Base cost ${baseCost.BaseCost:F2} is significantly higher than historical maximum ${maxCost:F2} for this product-supplier combination");
                        }
                        else if (baseCost.BaseCost < minCost * 0.5m)
                        {
                            result.Errors.Add($"Base cost ${baseCost.BaseCost:F2} is significantly lower than historical minimum ${minCost:F2} for this product-supplier combination");
                        }
                    }
                }
            }

            // Date range validation
            if (baseCost.StartDate.HasValue && baseCost.EndDate.HasValue)
            {
                if (baseCost.EndDate < baseCost.StartDate)
                {
                    result.Errors.Add("End Date cannot be earlier than Start Date");
                }
            }

            // Business warnings
            if (baseCost.BaseCost.HasValue && baseCost.BaseCost > 10000)
            {
                result.Warnings.Add("Base Cost is unusually high (over $10,000)");
            }

            if (baseCost.StartDate.HasValue && baseCost.StartDate < DateTime.Now.AddYears(-1))
            {
                result.Warnings.Add("Start Date is more than 1 year in the past");
            }
        }

        private async Task ValidateFallbackRules(BaseCostImport baseCost, BaseCostValidationResult result)
        {
            // Basic fallback validation when JSON config is not available
            if (baseCost.BaseCost <= 0)
            {
                result.Errors.Add("Base Cost is required and must be greater than 0");
            }

            if (!baseCost.StartDate.HasValue)
            {
                result.Errors.Add("Start Date is required");
            }

            if (!string.IsNullOrEmpty(baseCost.ProductName) && baseCost.ProductName.Length > 200)
            {
                result.Errors.Add("Product Name cannot exceed 200 characters");
            }
        }

        private async Task CheckDatabaseDuplicates(BaseCostImport baseCost, BatchValidationResult<BaseCostImport> result)
        {
            if (string.IsNullOrEmpty(baseCost.ProductName) || !baseCost.StartDate.HasValue)
                return;

            var existingBaseCosts = await _baseCostRepository.GetAllAsync(filter: bc => 
                (bc.Product.ProductName == baseCost.ProductName || bc.Product.Code == baseCost.ProductName) && 
                bc.StartDate == baseCost.StartDate);

            if (existingBaseCosts.Any())
            {
                result.Duplicates.Add(new DuplicateInfo
                {
                    RowNumber = baseCost.RowNumber,
                    Field = "ProductName + StartDate",
                    Value = $"{baseCost.ProductName} ({baseCost.StartDate:yyyy-MM-dd})",
                    DuplicateType = "Database",
                    ExistingId = existingBaseCosts.FirstOrDefault()?.ProductId
                });
            }
        }

        private void TrackBatchDuplicates(BaseCostImport baseCost, Dictionary<string, List<int>> batchDuplicates, int rowNumber)
        {
            if (string.IsNullOrEmpty(baseCost.ProductName) || !baseCost.StartDate.HasValue)
                return;

            var key = $"{baseCost.ProductName.ToLower()}_{baseCost.StartDate:yyyy-MM-dd}";
            if (!batchDuplicates.ContainsKey(key))
            {
                batchDuplicates[key] = new List<int>();
            }
            batchDuplicates[key].Add(rowNumber);
        }

        private void ProcessBatchDuplicates(Dictionary<string, List<int>> batchDuplicates, BatchValidationResult<BaseCostImport> result)
        {
            foreach (var kvp in batchDuplicates.Where(x => x.Value.Count > 1))
            {
                foreach (var rowNumber in kvp.Value)
                {
                    result.Duplicates.Add(new DuplicateInfo
                    {
                        RowNumber = rowNumber,
                        Field = "ProductName + StartDate",
                        Value = kvp.Key,
                        DuplicateType = "Batch",
                        ConflictingRows = kvp.Value.Where(r => r != rowNumber).ToList()
                    });
                }
            }
        }

        private object GetPropertyValue(BaseCostImport baseCost, string propertyName)
        {
            var property = typeof(BaseCostImport).GetProperty(propertyName);
            return property?.GetValue(baseCost);
        }

        private void AddFieldError(BaseCostValidationResult result, string fieldName, string error)
        {
            if (result.FieldErrors == null)
                result.FieldErrors = new Dictionary<string, List<string>>();

            if (!result.FieldErrors.ContainsKey(fieldName))
                result.FieldErrors[fieldName] = new List<string>();

            result.FieldErrors[fieldName].Add(error);
        }

        
    }
}
