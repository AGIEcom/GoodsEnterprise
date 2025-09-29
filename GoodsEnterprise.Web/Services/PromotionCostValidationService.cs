using GoodsEnterprise.Model.Models;
using GoodsEnterprise.DataAccess.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GoodsEnterprise.Web.Services
{
    /// <summary>
    /// Service for validating promotion cost import data
    /// </summary>
    public class PromotionCostValidationService : IPromotionCostValidationService
    {
        private readonly IGeneralRepository<PromotionCost> _promotionCostRepository;
        private readonly IGeneralRepository<Product> _productRepository;
        private readonly IGeneralRepository<Supplier> _supplierRepository;
        private readonly ILogger<PromotionCostValidationService> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private dynamic _configuration;

        public PromotionCostValidationService(
            IGeneralRepository<PromotionCost> promotionCostRepository,
            IGeneralRepository<Product> productRepository,
            IGeneralRepository<Supplier> supplierRepository,
            ILogger<PromotionCostValidationService> logger,
            IWebHostEnvironment webHostEnvironment)
        {
            _promotionCostRepository = promotionCostRepository;
            _productRepository = productRepository;
            _supplierRepository = supplierRepository;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            try
            {
                var configPath = Path.Combine(_webHostEnvironment.WebRootPath, "config", "promotioncost-import-columns.json");
                if (File.Exists(configPath))
                {
                    var configJson = File.ReadAllText(configPath);
                    _configuration = JsonConvert.DeserializeObject(configJson);
                    _logger.LogInformation("Promotion cost import configuration loaded successfully");
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

        public async Task<PromotionCostBatchValidationResult<PromotionCostImport>> ValidatePromotionCostsAsync(List<PromotionCostImport> promotionCosts)
        {
            var result = new PromotionCostBatchValidationResult<PromotionCostImport>
            {
                TotalRecords = promotionCosts.Count
            };

            try
            {
                // Track batch duplicates
                var batchDuplicates = new Dictionary<string, List<int>>();

                for (int i = 0; i < promotionCosts.Count; i++)
                {
                    var promotionCost = promotionCosts[i];
                    promotionCost.RowNumber = i + 1;

                    // Validate using JSON configuration or fallback
                    var validationResult = _configuration != null 
                        ? await ValidateUsingJsonConfig(promotionCost)
                        : await ValidateFallbackRules(promotionCost);

                    // Business rules validation
                    await ValidateBusinessRules(promotionCost, validationResult);

                    // Database duplicate check
                    await CheckDatabaseDuplicates(promotionCost, result);

                    // Track batch duplicates
                    TrackBatchDuplicates(promotionCost, batchDuplicates, promotionCost.RowNumber);

                    // Add validation results to import object
                    promotionCost.ValidationErrors.AddRange(validationResult.Errors);
                    promotionCost.ValidationWarnings.AddRange(validationResult.Warnings);
                    promotionCost.HasErrors = validationResult.Errors.Any();

                    if (validationResult.IsValid && !promotionCost.HasErrors)
                    {
                        result.ValidRecords.Add(promotionCost);
                        result.ValidRecordCount++;
                    }
                    else
                    {
                        result.InvalidRecords.Add(promotionCost);
                        result.InvalidRecordCount++;
                    }
                }

                // Process batch duplicates
                ProcessBatchDuplicates(batchDuplicates, result);

                _logger.LogInformation("Promotion cost validation completed. Valid: {Valid}, Invalid: {Invalid}, Duplicates: {Duplicates}", 
                    result.ValidRecordCount, result.InvalidRecordCount, result.Duplicates.Count);

            }
            catch (Exception ex)
            {
                result.GlobalErrors.Add($"Validation error: {ex.Message}");
                _logger.LogError(ex, "Error during promotion cost validation");
            }

            return result;
        }



        private async Task<PromotionCostValidationResult> ValidateUsingJsonConfig(PromotionCostImport promotionCost)
        {
            var result = new PromotionCostValidationResult { IsValid = true };

            try
            {
                var columns = _configuration?.ImportColumns?.columns;
                if (columns != null)
                {
                    foreach (var column in columns)
                    {
                        var columnName = column.name != null ? column.name.ToString() : null;
                        var columnDisplayName = column.displayName != null ? column.displayName.ToString() : columnName;

                        // Required field validation
                        if (column.required == true)
                        {
                            var value = GetPropertyValue(promotionCost, columnName);
                            if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
                            {
                                var error = $"{columnDisplayName} is required";
                                result.Errors.Add(error);
                                AddFieldError(result, columnName, error);
                                result.IsValid = false;
                            }
                        }

                        // Field-specific validation
                        ValidateFieldRules(promotionCost, column, result);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Configuration validation error: {ex.Message}");
                result.IsValid = false;
                _logger.LogError(ex, "Error during JSON configuration validation for promotion cost");
            }

            return result;
        }

        private void ValidateFieldRules(PromotionCostImport promotionCost, dynamic column, PromotionCostValidationResult result)
        {
            var columnName = column.name != null ? column.name.ToString() : null;
            var columnDisplayName = column.displayName != null ? column.displayName.ToString() : columnName;

            if (string.IsNullOrWhiteSpace(columnName))
            {
                _logger.LogWarning("Skipping validation for column with missing name in config.");
                return;
            }

            var value = GetPropertyValue(promotionCost, columnName);
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
                        var error = $"{columnDisplayName} cannot exceed {maxLength} characters";
                        result.Errors.Add(error);
                        AddFieldError(result, columnName, error);
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
                        var error = $"{columnDisplayName} must be a valid decimal number";
                        result.Errors.Add(error);
                        AddFieldError(result, columnName, error);
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
                                    var error = $"{columnDisplayName} must be at least {minValue}";
                                    result.Errors.Add(error);
                                    AddFieldError(result, columnName, error);
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
                                    var error = $"{columnDisplayName} cannot exceed {maxValue}";
                                    result.Errors.Add(error);
                                    AddFieldError(result, columnName, error);
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
                        var error = $"{columnDisplayName} must be a valid date";
                        result.Errors.Add(error);
                        AddFieldError(result, columnName, error);
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
                        var error = $"{columnDisplayName} must be one of: {acceptedList}";
                        result.Errors.Add(error);
                        AddFieldError(result, columnName, error);
                    }
                    break;
            }
        }

        private async Task ValidateBusinessRules(PromotionCostImport promotionCost, PromotionCostValidationResult result)
        {
            // Product existence check - prioritize ProductName since it's now required
            var productIdentifier =   promotionCost.ProductName  ;

            if (!string.IsNullOrEmpty(productIdentifier))
            {
                var existingProducts = await _productRepository.GetAllAsync(filter: p => 
                    p.ProductName == productIdentifier || p.Code == productIdentifier);

                if (!existingProducts.Any())
                {
                    result.Errors.Add($"Product '{productIdentifier}' not found in system");
                }
            }

            // Supplier existence check
            if (!string.IsNullOrEmpty(promotionCost.SupplierName))
            {
                var existingSuppliers = await _supplierRepository.GetAllAsync(filter: s => 
                    s.Name == promotionCost.SupplierName || s.Skucode == promotionCost.SupplierName);

                if (!existingSuppliers.Any())
                {
                    result.Warnings.Add($"Supplier '{promotionCost.SupplierName}' not found in system");
                }
            }
            if (!string.IsNullOrEmpty(promotionCost.ProductName) && !string.IsNullOrEmpty(promotionCost.SupplierName))
            {
                var existingBaseCosts = await _promotionCostRepository.GetAllAsync(filter: p =>
                    (p.Product.ProductName == promotionCost.ProductName || p.Product.Code == promotionCost.ProductName) &&
                    (p.Supplier.Name == promotionCost.SupplierName || p.Supplier.Skucode == promotionCost.SupplierName) &&
                    p.PromotionCost1 == promotionCost.PromotionCost);

                if (existingBaseCosts.Any())
                {
                    result.Errors.Add($"Base cost record for Product '{promotionCost.ProductName}' with Supplier '{promotionCost.SupplierName}' already exists for the same date and cost");
                }

                // Check for overlapping date ranges with different costs
                if (promotionCost.StartDate.HasValue)
                {
                    var newStartDate = promotionCost.StartDate.Value;
                    var newEndDate = promotionCost.EndDate ?? DateTime.MaxValue; // If no end date, assume ongoing

                    var overlappingBaseCosts = await _promotionCostRepository.GetAllAsync(filter: p =>
                        (p.Product.ProductName == promotionCost.ProductName || p.Product.Code == promotionCost.ProductName) &&
                        (p.Supplier.Name == promotionCost.SupplierName || p.Supplier.Skucode == promotionCost.SupplierName) &&
                        p.PromotionCost1 != promotionCost.PromotionCost);

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
                            var newRange = $"{promotionCost.StartDate?.ToString("yyyy-MM-dd")} to {promotionCost.EndDate?.ToString("yyyy-MM-dd") ?? "Ongoing"}";

                            result.Errors.Add($"Date range conflict: Product '{promotionCost.ProductName}' with Supplier '{promotionCost.SupplierName}' already has base cost ${conflictingCost.PromotionCost1:F2} for period ({existingRange}), which overlaps with new period ({newRange})");
                        }
                    }
                }

                // Validate base cost range for this product-supplier combination
                if (promotionCost.PromotionCost.HasValue)
                {
                    var existingCosts = await _promotionCostRepository.GetAllAsync(filter: p =>
                        (p.Product.ProductName == promotionCost.ProductName || p.Product.Code == promotionCost.ProductName) &&
                        (p.Supplier.Name == promotionCost.SupplierName || p.Supplier.Skucode == promotionCost.SupplierName));

                    if (existingCosts.Any())
                    {
                        var avgCost = existingCosts.Average(c => c.PromotionCost1 ?? 0);
                        var maxCost = existingCosts.Max(c => c.PromotionCost1 ?? 0);
                        var minCost = existingCosts.Min(c => c.PromotionCost1 ?? 0);

                        // Check if new cost is significantly different from historical costs
                        if (promotionCost.PromotionCost > maxCost * 1.5m)
                        {
                            result.Errors.Add($"Base cost ${promotionCost.PromotionCost:F2} is significantly higher than historical maximum ${maxCost:F2} for this product-supplier combination");
                        }
                        else if (promotionCost.PromotionCost < minCost * 0.5m)
                        {
                            result.Errors.Add($"Base cost ${promotionCost.PromotionCost:F2} is significantly lower than historical minimum ${minCost:F2} for this product-supplier combination");
                        }
                    }
                }
            }

            // Date range validation
            if (promotionCost.StartDate.HasValue && promotionCost.EndDate.HasValue)
            {
                if (promotionCost.EndDate < promotionCost.StartDate)
                {
                    result.Errors.Add("End Date cannot be earlier than Start Date");
                }

                var duration = promotionCost.EndDate.Value - promotionCost.StartDate.Value;
                if (duration.TotalDays > 365)
                {
                    result.Warnings.Add("Promotion period is longer than 1 year");
                }
            }

            // Sellout date validation
            if (promotionCost.SelloutStartDate.HasValue && promotionCost.SelloutEndDate.HasValue)
            {
                if (promotionCost.SelloutEndDate < promotionCost.SelloutStartDate)
                {
                    result.Errors.Add("Sellout End Date cannot be earlier than Sellout Start Date");
                }
            }

            // Business warnings
            if (promotionCost.PromotionCost.HasValue && promotionCost.PromotionCost > 10000)
            {
                result.Warnings.Add("Promotion Cost is unusually high (over $10,000)");
            }

            if (promotionCost.StartDate.HasValue && promotionCost.StartDate < DateTime.Now.Date)
            {
                result.Warnings.Add("Start Date is in the past");
            }

            // Check for overlapping promotions
            if (!string.IsNullOrEmpty(productIdentifier) && promotionCost.StartDate.HasValue && promotionCost.EndDate.HasValue)
            {
                var overlappingPromotions = await _promotionCostRepository.GetAllAsync(filter: pc =>
                    (pc.Product.ProductName == productIdentifier || pc.Product.Code == productIdentifier) &&
                    pc.StartDate <= promotionCost.EndDate &&
                    pc.EndDate >= promotionCost.StartDate);

                if (overlappingPromotions.Any())
                {
                    result.Warnings.Add($"Overlapping promotion found for product '{productIdentifier}' in the specified date range");
                }
            }
        }

        private async Task<PromotionCostValidationResult> ValidateFallbackRules(PromotionCostImport promotionCost)
        {
            var result = new PromotionCostValidationResult { IsValid = true };

            // Basic fallback validation when JSON config is not available
            // Check for ProductName first (now required), then ProductCode as fallback
            if (string.IsNullOrWhiteSpace(promotionCost.ProductName) )
            {
                result.Errors.Add("Product Name or Product Code is required");
                result.IsValid = false;
            }

            if (!promotionCost.PromotionCost.HasValue || promotionCost.PromotionCost <= 0)
            {
                result.Errors.Add("Promotion Cost is required and must be greater than 0");
                result.IsValid = false;
            }

            if (!promotionCost.StartDate.HasValue)
            {
                result.Errors.Add("Start Date is required");
                result.IsValid = false;
            }

            if (!promotionCost.EndDate.HasValue)
            {
                result.Errors.Add("End Date is required");
                result.IsValid = false;
            }

            if (!string.IsNullOrEmpty(promotionCost.ProductName) && promotionCost.ProductName.Length > 200)
            {
                result.Errors.Add("Product Name cannot exceed 200 characters");
                result.IsValid = false;
            }

            //if (!string.IsNullOrEmpty(promotionCost.ProductCode) && promotionCost.ProductCode.Length > 50)
            //{
            //    result.Errors.Add("Product Code cannot exceed 50 characters");
            //    result.IsValid = false;
            //}

            if (!string.IsNullOrEmpty(promotionCost.Remark) && promotionCost.Remark.Length > 500)
            {
                result.Errors.Add("Remark cannot exceed 500 characters");
                result.IsValid = false;
            }

            return result;
        }

        private async Task CheckDatabaseDuplicates(PromotionCostImport promotionCost, PromotionCostBatchValidationResult<PromotionCostImport> result)
        {
            var productIdentifier =   promotionCost.ProductName;

            if (string.IsNullOrEmpty(productIdentifier) || !promotionCost.StartDate.HasValue || !promotionCost.EndDate.HasValue)
                return;

            var existingPromotionCosts = await _promotionCostRepository.GetAllAsync(filter: pc => 
                (pc.Product.ProductName == productIdentifier || pc.Product.Code == productIdentifier) && 
                pc.StartDate == promotionCost.StartDate &&
                pc.EndDate == promotionCost.EndDate);

            if (existingPromotionCosts.Any())
            {
                result.Duplicates.Add(new DuplicateInfo
                {
                    RowNumber = promotionCost.RowNumber,
                    Field = "Product + DateRange",
                    Value = $"{productIdentifier} ({promotionCost.StartDate:yyyy-MM-dd} to {promotionCost.EndDate:yyyy-MM-dd})",
                    DuplicateType = "Database",
                    ExistingId = existingPromotionCosts.FirstOrDefault()?.PromotionCostId
                });
            }
        }

        private void TrackBatchDuplicates(PromotionCostImport promotionCost, Dictionary<string, List<int>> batchDuplicates, int rowNumber)
        {
            var productIdentifier = promotionCost.ProductName;

            if (string.IsNullOrEmpty(productIdentifier) || !promotionCost.StartDate.HasValue || !promotionCost.EndDate.HasValue)
                return;

            var key = $"{productIdentifier.ToLower()}_{promotionCost.StartDate:yyyy-MM-dd}_{promotionCost.EndDate:yyyy-MM-dd}";
            if (!batchDuplicates.ContainsKey(key))
            {
                batchDuplicates[key] = new List<int>();
            }
            batchDuplicates[key].Add(rowNumber);
        }

        private void ProcessBatchDuplicates(Dictionary<string, List<int>> batchDuplicates, PromotionCostBatchValidationResult<PromotionCostImport> result)
        {
            foreach (var kvp in batchDuplicates.Where(x => x.Value.Count > 1))
            {
                foreach (var rowNumber in kvp.Value)
                {
                    result.Duplicates.Add(new DuplicateInfo
                    {
                        RowNumber = rowNumber,
                        Field = "Product + DateRange",
                        Value = kvp.Key,
                        DuplicateType = "Batch",
                        ConflictingRows = kvp.Value.Where(r => r != rowNumber).ToList()
                    });
                }
            }
        }

        private object GetPropertyValue(PromotionCostImport promotionCost, string propertyName)
        {
            try
            {
                var property = typeof(PromotionCostImport).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property != null)
                {
                    return property.GetValue(promotionCost);
                }
                
                // Log warning for missing property
                _logger.LogWarning("Property '{PropertyName}' not found in PromotionCostImport model", propertyName);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting property value for '{PropertyName}'", propertyName);
                return null;
            }
        }

        private void AddFieldError(PromotionCostValidationResult result, string fieldName, string error)
        {
            if (result.FieldErrors == null)
                result.FieldErrors = new Dictionary<string, List<string>>();

            if (!result.FieldErrors.ContainsKey(fieldName))
                result.FieldErrors[fieldName] = new List<string>();

            result.FieldErrors[fieldName].Add(error);
        }

        // Legacy method for backward compatibility
        public async Task<PromotionCostValidationResult> ValidatePromotionCostAsync(PromotionCostImport promotionCost)
        {
            var result = _configuration != null 
                ? await ValidateUsingJsonConfig(promotionCost)
                : await ValidateFallbackRules(promotionCost);

            await ValidateBusinessRules(promotionCost, result);

            return new PromotionCostValidationResult
            {
                IsValid = result.IsValid,
                Errors = result.Errors,
                Warnings = result.Warnings
            };
        }
    }

    
}
