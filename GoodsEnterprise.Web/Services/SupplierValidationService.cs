using GoodsEnterprise.Model.Models;
using GoodsEnterprise.DataAccess.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace GoodsEnterprise.Web.Services
{
    /// <summary>
    /// Service for validating supplier import data
    /// </summary>
    public class SupplierValidationService : ISupplierValidationService
    {
        private readonly IGeneralRepository<Supplier> _supplierRepository;
        private readonly ILogger<SupplierValidationService> _logger;
        private readonly IWebHostEnvironment _environment;
        private SupplierImportConfig _config;

        public SupplierValidationService(
            IGeneralRepository<Supplier> supplierRepository,
            ILogger<SupplierValidationService> logger,
            IWebHostEnvironment environment)
        {
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

        public async Task<ValidationResult> ValidateSupplierAsync(SupplierImport supplier)
        {
            var result = new ValidationResult();

            // JSON-based validation instead of data annotations
            await ValidateUsingJsonConfig(supplier, result);
            
            // Additional business logic validation
            await ValidateBusinessRules(supplier, result);

            // Custom business validation
            await ValidateCustomRulesAsync(supplier, result);

            result.IsValid = result.Errors.Count == 0;
            return result;
        }

        private async Task ValidateUsingJsonConfig(SupplierImport supplier, ValidationResult result)
        {
            if (_config?.SupplierImportColumns == null)
            {
                // Fallback to basic validation if config not loaded
                await ValidateFallbackRules(supplier, result);
                return;
            }

            // Validate required fields from JSON config
            var requiredColumns = _config.SupplierImportColumns.RequiredColumns ?? new List<ColumnDefinition>();
            foreach (var column in requiredColumns)
            {
                var value = GetPropertyValue(supplier, column.Name);
                if (string.IsNullOrWhiteSpace(value?.ToString()))
                {
                    var error = $"{column.DisplayName ?? column.Name} is required";
                    result.Errors.Add(error);
                    AddFieldError(result, column.Name, error);
                }
                else
                {
                    // Validate field-specific rules
                    ValidateFieldRules(supplier, column, result);
                }
            }

            // Validate optional fields from JSON config
            var optionalColumns = _config.SupplierImportColumns.OptionalColumns ?? new List<ColumnDefinition>();
            foreach (var column in optionalColumns)
            {
                var value = GetPropertyValue(supplier, column.Name);
                if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                {
                    // Validate field-specific rules for non-empty optional fields
                    ValidateFieldRules(supplier, column, result);
                }
            }
        }

        private void ValidateFieldRules(SupplierImport supplier, ColumnDefinition column, ValidationResult result)
        {
            var value = GetPropertyValue(supplier, column.Name);
            if (value == null) return;

            var stringValue = value.ToString();

            // String length validation
            if (column.MaxLength.HasValue && stringValue.Length > column.MaxLength.Value)
            {
                var error = $"{column.DisplayName ?? column.Name} cannot exceed {column.MaxLength} characters";
                result.Errors.Add(error);
                AddFieldError(result, column.Name, error);
            }

            // Type-specific validation
            switch (column.Type?.ToLower())
            {
                case "email":
                    if (!IsValidEmail(stringValue))
                    {
                        var error = $"{column.DisplayName ?? column.Name} must be a valid email address";
                        result.Errors.Add(error);
                        AddFieldError(result, column.Name, error);
                    }
                    break;

                case "integer":
                    if (!int.TryParse(stringValue, out int intValue))
                    {
                        var error = $"{column.DisplayName ?? column.Name} must be a valid integer";
                        result.Errors.Add(error);
                        AddFieldError(result, column.Name, error);
                    }
                    else
                    {
                        // Range validation for integers
                        if (column.Min.HasValue && intValue < column.Min.Value)
                        {
                            var error = $"{column.DisplayName ?? column.Name} must be at least {column.Min}";
                            result.Errors.Add(error);
                            AddFieldError(result, column.Name, error);
                        }
                        if (column.Max.HasValue && intValue > column.Max.Value)
                        {
                            var error = $"{column.DisplayName ?? column.Name} must not exceed {column.Max}";
                            result.Errors.Add(error);
                            AddFieldError(result, column.Name, error);
                        }
                    }
                    break;

                case "decimal":
                    if (!decimal.TryParse(stringValue, out decimal decimalValue))
                    {
                        var error = $"{column.DisplayName ?? column.Name} must be a valid decimal number";
                        result.Errors.Add(error);
                        AddFieldError(result, column.Name, error);
                    }
                    else if (column.Min.HasValue && decimalValue < column.Min.Value)
                    {
                        var error = $"{column.DisplayName ?? column.Name} must be at least {column.Min}";
                        result.Errors.Add(error);
                        AddFieldError(result, column.Name, error);
                    }
                    break;

                case "boolean":
                    if (column.AcceptedValues?.Any() == true)
                    {
                        if (!column.AcceptedValues.Contains(stringValue, StringComparer.OrdinalIgnoreCase))
                        {
                            var error = $"{column.DisplayName ?? column.Name} must be one of: {string.Join(", ", column.AcceptedValues)}";
                            result.Errors.Add(error);
                            AddFieldError(result, column.Name, error);
                        }
                    }
                    break;
            }

            // Accepted values validation
            if (column.AcceptedValues?.Any() == true && column.Type?.ToLower() != "boolean")
            {
                if (!column.AcceptedValues.Contains(stringValue, StringComparer.OrdinalIgnoreCase))
                {
                    var error = $"{column.DisplayName ?? column.Name} must be one of: {string.Join(", ", column.AcceptedValues)}";
                    result.Errors.Add(error);
                    AddFieldError(result, column.Name, error);
                }
            }
        }

        private object GetPropertyValue(SupplierImport supplier, string propertyName)
        {
            var property = typeof(SupplierImport).GetProperty(propertyName);
            return property?.GetValue(supplier);
        }

        private void AddFieldError(ValidationResult result, string fieldName, string error)
        {
            if (!result.FieldErrors.ContainsKey(fieldName))
                result.FieldErrors[fieldName] = new List<string>();
            
            result.FieldErrors[fieldName].Add(error);
        }

        private async Task ValidateFallbackRules(SupplierImport supplier, ValidationResult result)
        {
            // Fallback validation when JSON config is not available
            if (string.IsNullOrWhiteSpace(supplier.SupplierName))
            {
                result.Errors.Add("Supplier Name is required");
                AddFieldError(result, nameof(supplier.SupplierName), "Supplier Name is required");
            }

            if (string.IsNullOrWhiteSpace(supplier.SKUCode))
            {
                result.Errors.Add("SKU Code is required");
                AddFieldError(result, nameof(supplier.SKUCode), "SKU Code is required");
            }

            if (string.IsNullOrWhiteSpace(supplier.Email))
            {
                result.Errors.Add("Email is required");
                AddFieldError(result, nameof(supplier.Email), "Email is required");
            }
            else if (!IsValidEmail(supplier.Email))
            {
                result.Errors.Add("Email must be a valid email address");
                AddFieldError(result, nameof(supplier.Email), "Email must be a valid email address");
            }
        }

        private async Task ValidateBusinessRules(SupplierImport supplier, ValidationResult result)
        {
            // Additional business logic validation can be added here
            // For example: duplicate checking, business-specific rules, etc.
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Use built-in email validation
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public async Task<BatchValidationResult> ValidateSuppliersAsync(List<SupplierImport> suppliers)
        {
            var result = new BatchValidationResult
            {
                TotalRecords = suppliers.Count
            };

            try
            {
                // Validate each supplier individually
                foreach (var supplier in suppliers)
                {
                    var validation = await ValidateSupplierAsync(supplier);
                    
                    if (validation.IsValid)
                    {
                        result.ValidSuppliers.Add(supplier);
                        result.ValidRecords++;
                    }
                    else
                    {
                        supplier.ValidationErrors.AddRange(validation.Errors);
                        supplier.ValidationWarnings.AddRange(validation.Warnings);
                        supplier.HasErrors = true;
                        
                        result.InvalidSuppliers.Add(supplier);
                        result.InvalidRecords++;
                    }
                }

                // Check for duplicates
                result.Duplicates = await CheckForDuplicatesAsync(suppliers);
                
                // Mark suppliers with duplicates as invalid
                foreach (var duplicate in result.Duplicates)
                {
                    var supplier = suppliers.FirstOrDefault(s => s.RowNumber == duplicate.RowNumber);
                    if (supplier != null && !supplier.HasErrors)
                    {
                        supplier.ValidationErrors.Add($"Duplicate {duplicate.Field}: {duplicate.Value} ({duplicate.DuplicateType})");
                        supplier.HasErrors = true;
                        
                        // Move from valid to invalid if needed
                        if (result.ValidSuppliers.Contains(supplier))
                        {
                            result.ValidSuppliers.Remove(supplier);
                            result.InvalidSuppliers.Add(supplier);
                            result.ValidRecords--;
                            result.InvalidRecords++;
                        }
                    }
                }

                // Global validations
                if (suppliers.Count == 0)
                {
                    result.GlobalErrors.Add("No supplier data found to import.");
                }
                else if (result.ValidRecords == 0)
                {
                    result.GlobalWarnings.Add("No valid suppliers found. Please check your data and try again.");
                }

                _logger.LogInformation("Batch validation completed. Total: {Total}, Valid: {Valid}, Invalid: {Invalid}, Duplicates: {Duplicates}",
                    result.TotalRecords, result.ValidRecords, result.InvalidRecords, result.Duplicates.Count);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during batch validation");
                result.GlobalErrors.Add($"Validation error: {ex.Message}");
            }

            return result;
        }

        public async Task<List<DuplicateInfo>> CheckForDuplicatesAsync(List<SupplierImport> suppliers)
        {
            var duplicates = new List<DuplicateInfo>();

            try
            {
                // Check for duplicates within the batch
                var batchDuplicates = CheckBatchDuplicates(suppliers);
                duplicates.AddRange(batchDuplicates);

                // Check for duplicates against existing database records
                var dbDuplicates = await CheckDatabaseDuplicatesAsync(suppliers);
                duplicates.AddRange(dbDuplicates);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking for duplicates");
            }

            return duplicates;
        }

        public async Task<BusinessRuleValidationResult> ValidateBusinessRulesAsync(SupplierImport supplier)
        {
            var result = new BusinessRuleValidationResult { IsValid = true };

            try
            {
                // Rule 1: Lead time validation
                if (supplier.LeadTimeDays.HasValue && supplier.LeadTimeDays.Value > 180)
                {
                    result.Recommendations.Add("Lead time exceeds 180 days. Consider reviewing with supplier.");
                }

                // Rule 2: Cost validation
                if (supplier.LastCost.HasValue && supplier.LastCost.Value <= 0)
                {
                    result.RuleViolations.Add("Last cost must be greater than zero.");
                    result.IsValid = false;
                }

                // Rule 3: Date range validation
                if (supplier.ValidFrom.HasValue && supplier.ValidTo.HasValue)
                {
                    if (supplier.ValidFrom.Value >= supplier.ValidTo.Value)
                    {
                        result.RuleViolations.Add("Valid From date must be earlier than Valid To date.");
                        result.IsValid = false;
                    }

                    if (supplier.ValidTo.Value < DateTime.Now.Date)
                    {
                        result.Recommendations.Add("Valid To date is in the past. Supplier agreement may be expired.");
                    }
                }

                // Rule 4: Incoterm validation
                if (!string.IsNullOrEmpty(supplier.Incoterm))
                {
                    var validIncoterms = new[] { "EXW", "FCA", "CPT", "CIP", "DAT", "DAP", "DDP", "FAS", "FOB", "CFR", "CIF" };
                    if (!validIncoterms.Contains(supplier.Incoterm.ToUpperInvariant()))
                    {
                        result.RuleViolations.Add($"Invalid Incoterm: {supplier.Incoterm}. Valid values: {string.Join(", ", validIncoterms)}");
                        result.IsValid = false;
                    }
                }

                // Rule 5: Phone number format validation
                if (!string.IsNullOrEmpty(supplier.Phone))
                {
                    var phoneRegex = new Regex(@"^[\+]?[1-9][\d]{0,15}$");
                    if (!phoneRegex.IsMatch(supplier.Phone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "")))
                    {
                        result.Recommendations.Add("Phone number format may be invalid. Please verify.");
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating business rules for supplier");
                result.RuleViolations.Add($"Business rule validation error: {ex.Message}");
                result.IsValid = false;
            }

            return result;
        }

        private async Task ValidateCustomRulesAsync(SupplierImport supplier, ValidationResult result)
        {
            // Email format validation (additional to data annotations)
            if (!string.IsNullOrEmpty(supplier.Email))
            {
                var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
                if (!emailRegex.IsMatch(supplier.Email))
                {
                    result.Errors.Add("Invalid email format.");
                }
            }

            // SKU Code format validation
            if (!string.IsNullOrEmpty(supplier.SKUCode))
            {
                if (supplier.SKUCode.Length < 3)
                {
                    result.Errors.Add("SKU Code must be at least 3 characters long.");
                }
                
                if (supplier.SKUCode.Contains(" "))
                {
                    result.Warnings.Add("SKU Code contains spaces. Consider using underscores or hyphens instead.");
                }
            }

            // Business rule validation
            var businessRules = await ValidateBusinessRulesAsync(supplier);
            result.Errors.AddRange(businessRules.RuleViolations);
            result.Warnings.AddRange(businessRules.Recommendations);
        }

        private List<DuplicateInfo> CheckBatchDuplicates(List<SupplierImport> suppliers)
        {
            var duplicates = new List<DuplicateInfo>();

            // Check SKU Code duplicates within batch
            var skuGroups = suppliers
                .Where(s => !string.IsNullOrEmpty(s.SKUCode))
                .GroupBy(s => s.SKUCode.ToUpperInvariant())
                .Where(g => g.Count() > 1);

            foreach (var group in skuGroups)
            {
                var rows = group.Select(s => s.RowNumber).ToList();
                foreach (var supplier in group)
                {
                    duplicates.Add(new DuplicateInfo
                    {
                        RowNumber = supplier.RowNumber,
                        Field = "SKUCode",
                        Value = supplier.SKUCode,
                        DuplicateType = "Batch",
                        ConflictingRows = rows.Where(r => r != supplier.RowNumber).ToList()
                    });
                }
            }

            // Check Email duplicates within batch
            var emailGroups = suppliers
                .Where(s => !string.IsNullOrEmpty(s.Email))
                .GroupBy(s => s.Email.ToLowerInvariant())
                .Where(g => g.Count() > 1);

            foreach (var group in emailGroups)
            {
                var rows = group.Select(s => s.RowNumber).ToList();
                foreach (var supplier in group)
                {
                    duplicates.Add(new DuplicateInfo
                    {
                        RowNumber = supplier.RowNumber,
                        Field = "Email",
                        Value = supplier.Email,
                        DuplicateType = "Batch",
                        ConflictingRows = rows.Where(r => r != supplier.RowNumber).ToList()
                    });
                }
            }

            return duplicates;
        }

        private async Task<List<DuplicateInfo>> CheckDatabaseDuplicatesAsync(List<SupplierImport> suppliers)
        {
            var duplicates = new List<DuplicateInfo>();

            try
            {
                // Get all existing suppliers for comparison
                var existingSuppliers = await _supplierRepository.GetAllAsync();

                foreach (var supplier in suppliers)
                {
                    // Check SKU Code duplicates
                    if (!string.IsNullOrEmpty(supplier.SKUCode))
                    {
                        var existing = existingSuppliers.FirstOrDefault(s => 
                            s.Skucode?.Equals(supplier.SKUCode, StringComparison.OrdinalIgnoreCase) == true);
                        
                        if (existing != null)
                        {
                            duplicates.Add(new DuplicateInfo
                            {
                                RowNumber = supplier.RowNumber,
                                Field = "SKUCode",
                                Value = supplier.SKUCode,
                                DuplicateType = "Database",
                                ExistingId = existing.Id
                            });
                        }
                    }

                    // Check Email duplicates
                    if (!string.IsNullOrEmpty(supplier.Email))
                    {
                        var existing = existingSuppliers.FirstOrDefault(s => 
                            s.Email?.Equals(supplier.Email, StringComparison.OrdinalIgnoreCase) == true);
                        
                        if (existing != null)
                        {
                            duplicates.Add(new DuplicateInfo
                            {
                                RowNumber = supplier.RowNumber,
                                Field = "Email",
                                Value = supplier.Email,
                                DuplicateType = "Database",
                                ExistingId = existing.Id
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking database duplicates");
            }

            return duplicates;
        }

        // Configuration classes for JSON deserialization (shared with SupplierImportService)
        private class SupplierImportConfig
        {
            public SupplierImportColumns SupplierImportColumns { get; set; }
        }

        private class SupplierImportColumns
        {
            public List<ColumnDefinition> RequiredColumns { get; set; }
            public List<ColumnDefinition> OptionalColumns { get; set; }
        }

        private class ColumnDefinition
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
    }
}
