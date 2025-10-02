using GoodsEnterprise.Model.Models;
using Microsoft.AspNetCore.Http;
using ExcelDataReader;
using System.Data;
using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Linq;
using System.Reflection;

namespace GoodsEnterprise.Web.Services
{
    /// <summary>
    /// Service for reading Excel files and converting to import objects (Supplier, Product, BaseCost, PromotionCost)
    /// </summary>
    public class ExcelReaderService : IExcelReaderService
    {
        private readonly ILogger<ExcelReaderService> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly Dictionary<string, Dictionary<string, string>> _columnMappings;

        public ExcelReaderService(ILogger<ExcelReaderService> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
            _columnMappings = LoadAllColumnMappings();

            // Register ExcelDataReader encoding provider
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public async Task<ExcelReadResult<T>> ReadFromExcelAsync<T>(IFormFile file, string importType) where T : class, IImportModel, new()
        {
            var result = new ExcelReadResult<T>();

            try
            {
                // Validate file first
                var fileValidation = await ValidateExcelFileAsync(file);
                if (!fileValidation.IsValid)
                {
                    result.Errors.AddRange(fileValidation.Errors);
                    return result;
                }

                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                stream.Position = 0;

                using var reader = ExcelReaderFactory.CreateReader(stream);
                var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });

                var dataTable = dataSet.Tables[0];
                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    result.Errors.Add("No data found in the Excel file.");
                    return result;
                }

                // Get column mapping
                result.ColumnMapping = GetColumnMappingFromDataTable(dataTable, importType);

                if (result.ColumnMapping.Count() == 0)
                {
                    result.Errors.Add("No valid columns found. Please ensure the first row contains column headers.");
                    return result;
                }

                // Validate required columns 
                var requiredColumns = GetRequiredColumnsFromConfig(importType);
                var missingColumns = requiredColumns.Where(col => !result.ColumnMapping.ContainsKey(col)).ToList();

                if (missingColumns.Any())
                {
                    result.Errors.Add($"Missing required columns: {string.Join(", ", missingColumns)}");
                    return result;
                }

                // Read data rows
                result.TotalRows = dataTable.Rows.Count;

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    try
                    {
                        var importObject = ReadFromDataRow<T>(dataTable.Rows[i], result.ColumnMapping, importType);
                        importObject.RowNumber = i + 1;

                        result.Data.Add(importObject);

                        if (importObject.HasErrors)
                        {
                            result.ErrorRows++;
                        }
                        else
                        {
                            result.ValidRows++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error reading row {Row}", i + 1);
                        result.Errors.Add($"Error reading row {i + 1}: {ex.Message}");
                        result.ErrorRows++;
                    }
                }

                _logger.LogInformation("Excel read completed. Total: {Total}, Valid: {Valid}, Errors: {Errors}",
                    result.TotalRows, result.ValidRows, result.ErrorRows);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading Excel file: {FileName}", file.FileName);
                result.Errors.Add($"Error reading Excel file: {ex.Message}");
            }

            return result;
        }

        public async Task<ExcelReadResult<SupplierImport>> ReadSuppliersFromExcelAsync(IFormFile file)
        {
            return await ReadFromExcelAsync<SupplierImport>(file, "Supplier");
        }

        public async Task<ExcelReadResult<ProductImport>> ReadProductsFromExcelAsync(IFormFile file)
        {
            return await ReadFromExcelAsync<ProductImport>(file, "Product");
        }

        public async Task<ExcelReadResult<BaseCostImport>> ReadBaseCostsFromExcelAsync(IFormFile file)
        {
            return await ReadFromExcelAsync<BaseCostImport>(file, "BaseCost");
        }

        public async Task<ExcelReadResult<PromotionCostImport>> ReadPromotionCostsFromExcelAsync(IFormFile file)
        {
            return await ReadFromExcelAsync<PromotionCostImport>(file, "PromotionCost");
        }

        public async Task<FileValidationResult> ValidateExcelFileAsync(IFormFile file)
        {
            var result = new FileValidationResult
            {
                FileName = file.FileName,
                FileSize = file.Length,
                FileExtension = Path.GetExtension(file.FileName).ToLowerInvariant()
            };

            // Check file size (50MB limit)
            const long maxFileSize = 50 * 1024 * 1024; // 50MB
            if (file.Length > maxFileSize)
            {
                result.Errors.Add($"File size ({FormatFileSize(file.Length)}) exceeds maximum allowed size (50MB).");
            }

            // Check file extension
            var allowedExtensions = new[] { ".xlsx", ".xls" };
            if (!allowedExtensions.Contains(result.FileExtension))
            {
                result.Errors.Add($"Invalid file type. Only Excel files (.xlsx, .xls) are allowed.");
            }

            // Check if file is empty
            if (file.Length == 0)
            {
                result.Errors.Add("File is empty.");
            }

            // Try to open the file to check if it's a valid Excel file
            if (result.Errors.Count() == 0)
            {
                try
                {
                    using var stream = new MemoryStream();
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    using var reader = ExcelReaderFactory.CreateReader(stream);
                    var dataSet = reader.AsDataSet();

                    if (dataSet.Tables.Count == 0)
                    {
                        result.Errors.Add("No worksheets found in the Excel file.");
                    }
                    else if (dataSet.Tables[0].Rows.Count < 1)
                    {
                        result.Warnings.Add("Excel file appears to have no data rows (only headers or empty).");
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Invalid Excel file format: {ex.Message}");
                }
            }

            result.IsValid = result.Errors.Count() == 0;
            return result;
        }

        public async Task<Dictionary<string, int>> GetColumnMappingAsync(IFormFile file, string importType = "Supplier")
        {
            try
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                stream.Position = 0;

                using var reader = ExcelReaderFactory.CreateReader(stream);
                var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });

                return GetColumnMappingFromDataTable(dataSet.Tables[0], importType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting column mapping from file: {FileName}", file.FileName);
                return new Dictionary<string, int>();
            }
        }

        private Dictionary<string, int> GetColumnMappingFromDataTable(DataTable dataTable, string importType = "Supplier")
        {
            var mapping = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            if (dataTable?.Columns == null) return mapping;

            var columnMapping = _columnMappings.ContainsKey(importType) ? _columnMappings[importType] : new Dictionary<string, string>();

            for (int col = 0; col < dataTable.Columns.Count; col++)
            {
                var headerValue = dataTable.Columns[col].ColumnName?.Trim();
                if (!string.IsNullOrEmpty(headerValue))
                {
                    // Direct mapping first
                    if (columnMapping.ContainsKey(headerValue))
                    {
                        mapping[columnMapping[headerValue]] = col;
                    }
                    // Try exact match
                    else if (IsValidColumnName(headerValue, importType))
                    {
                        mapping[headerValue] = col;
                    }
                }
            }

            return mapping;
        }

        private T ReadFromDataRow<T>(DataRow dataRow, Dictionary<string, int> columnMapping, string importType) where T : class, IImportModel, new()
        {
            return importType switch
            {
                "Supplier" => ReadSupplierFromDataRow(dataRow, columnMapping) as T,
                "Product" => ReadProductFromDataRow(dataRow, columnMapping) as T,
                "BaseCost" => ReadBaseCostFromDataRow(dataRow, columnMapping) as T,
                "PromotionCost" => ReadPromotionCostFromDataRow(dataRow, columnMapping) as T,
                _ => throw new ArgumentException($"Unsupported import type: {importType}")
            };
        }

        private SupplierImport ReadSupplierFromDataRow(DataRow dataRow, Dictionary<string, int> columnMapping)
        {
            var supplier = new SupplierImport();
            var errors = new List<string>();
            var warnings = new List<string>();

            try
            {
                // Required fields
                supplier.SupplierName = GetCellValueFromDataRow(dataRow, columnMapping, "SupplierName")?.Trim();
                supplier.SKUCode = GetCellValueFromDataRow(dataRow, columnMapping, "SKUCode")?.Trim();
                supplier.Email = GetCellValueFromDataRow(dataRow, columnMapping, "Email")?.Trim();

                // Optional fields
                supplier.FirstName = GetCellValueFromDataRow(dataRow, columnMapping, "FirstName")?.Trim();
                supplier.LastName = GetCellValueFromDataRow(dataRow, columnMapping, "LastName")?.Trim();
                supplier.Phone = GetCellValueFromDataRow(dataRow, columnMapping, "Phone")?.Trim();
                supplier.Address1 = GetCellValueFromDataRow(dataRow, columnMapping, "Address1")?.Trim();
                supplier.Address2 = GetCellValueFromDataRow(dataRow, columnMapping, "Address2")?.Trim();
                supplier.Description = GetCellValueFromDataRow(dataRow, columnMapping, "Description")?.Trim();

                // Boolean fields
                supplier.IsActive = ParseBoolean(GetCellValueFromDataRow(dataRow, columnMapping, "IsActive"), true);
                supplier.IsPreferred = ParseBoolean(GetCellValueFromDataRow(dataRow, columnMapping, "IsPreferred"), false);

                // Numeric fields
                if (int.TryParse(GetCellValueFromDataRow(dataRow, columnMapping, "LeadTimeDays"), out int leadTime))
                {
                    supplier.LeadTimeDays = leadTime;
                }

                if (decimal.TryParse(GetCellValueFromDataRow(dataRow, columnMapping, "LastCost"), out decimal lastCost))
                {
                    supplier.LastCost = lastCost;
                }

                supplier.MoqCase = GetCellValueFromDataRow(dataRow, columnMapping, "MoqCase")?.Trim();
                supplier.Incoterm = GetCellValueFromDataRow(dataRow, columnMapping, "Incoterm")?.Trim();

                // Date fields
                supplier.ValidFrom = ParseDate(GetCellValueFromDataRow(dataRow, columnMapping, "ValidFrom"));
                supplier.ValidTo = ParseDate(GetCellValueFromDataRow(dataRow, columnMapping, "ValidTo"));

                // Validate using JSON configuration
                var configErrors = ValidateImportModelFromConfig(supplier, "Supplier");
                errors.AddRange(configErrors);

            }
            catch (Exception ex)
            {
                errors.Add($"Error parsing row data: {ex.Message}");
            }

            supplier.ValidationErrors = errors;
            supplier.ValidationWarnings = warnings;
            supplier.HasErrors = errors.Count > 0;

            return supplier;
        }

        private ProductImport ReadProductFromDataRow(DataRow dataRow, Dictionary<string, int> columnMapping)
        {
            var product = new ProductImport();
            var errors = new List<string>();
            var warnings = new List<string>();

            try
            {
                // Required fields
                product.Code = GetCellValueFromDataRow(dataRow, columnMapping, "Code")?.Trim();
                product.ProductName = GetCellValueFromDataRow(dataRow, columnMapping, "ProductName")?.Trim();

                // Optional fields
                product.ProductDescription = GetCellValueFromDataRow(dataRow, columnMapping, "ProductDescription")?.Trim();
                product.BrandName = GetCellValueFromDataRow(dataRow, columnMapping, "BrandName")?.Trim();
                product.CategoryName = GetCellValueFromDataRow(dataRow, columnMapping, "CategoryName")?.Trim();
                product.SubCategoryName = GetCellValueFromDataRow(dataRow, columnMapping, "SubCategoryName")?.Trim();
                product.InnerEan = GetCellValueFromDataRow(dataRow, columnMapping, "InnerEan")?.Trim();
                product.OuterEan = GetCellValueFromDataRow(dataRow, columnMapping, "OuterEan")?.Trim();
                product.UnitSize = GetCellValueFromDataRow(dataRow, columnMapping, "UnitSize")?.Trim();
                product.SupplierName = GetCellValueFromDataRow(dataRow, columnMapping, "SupplierName")?.Trim();
                product.TaxslabName = GetCellValueFromDataRow(dataRow, columnMapping, "TaxslabName")?.Trim();

                // Numeric fields
                if (int.TryParse(GetCellValueFromDataRow(dataRow, columnMapping, "Upc"), out int upc))
                    product.Upc = upc;
                if (int.TryParse(GetCellValueFromDataRow(dataRow, columnMapping, "LayerQuantity"), out int layerQty))
                    product.LayerQuantity = layerQty;
                if (int.TryParse(GetCellValueFromDataRow(dataRow, columnMapping, "PalletQuantity"), out int palletQty))
                    product.PalletQuantity = palletQty;
                if (int.TryParse(GetCellValueFromDataRow(dataRow, columnMapping, "ShelfLifeInWeeks"), out int shelfLife))
                    product.ShelfLifeInWeeks = shelfLife;

                // Decimal fields
                if (decimal.TryParse(GetCellValueFromDataRow(dataRow, columnMapping, "CasePrice"), out decimal casePrice))
                    product.CasePrice = casePrice;
                if (decimal.TryParse(GetCellValueFromDataRow(dataRow, columnMapping, "PackHeight"), out decimal packHeight))
                    product.PackHeight = packHeight;
                if (decimal.TryParse(GetCellValueFromDataRow(dataRow, columnMapping, "PackDepth"), out decimal packDepth))
                    product.PackDepth = packDepth;
                if (decimal.TryParse(GetCellValueFromDataRow(dataRow, columnMapping, "PackWidth"), out decimal packWidth))
                    product.PackWidth = packWidth;

                // Boolean fields
                product.IsActive = ParseBoolean(GetCellValueFromDataRow(dataRow, columnMapping, "IsActive"), true);
                product.isTaxable = ParseBoolean(GetCellValueFromDataRow(dataRow, columnMapping, "isTaxable"), false);

                // Date fields
                product.ExpiryDate = ParseDate(GetCellValueFromDataRow(dataRow, columnMapping, "ExpiryDate"));

                // Validate required fields
                if (string.IsNullOrEmpty(product.Code))
                    errors.Add("Product Code is required");
                if (string.IsNullOrEmpty(product.ProductName))
                    errors.Add("Product Name is required");
            }
            catch (Exception ex)
            {
                errors.Add($"Error parsing row data: {ex.Message}");
            }

            product.ValidationErrors = errors;
            product.ValidationWarnings = warnings;
            product.HasErrors = errors.Count > 0;

            return product;
        }

        private BaseCostImport ReadBaseCostFromDataRow(DataRow dataRow, Dictionary<string, int> columnMapping)
        {
            var baseCost = new BaseCostImport();
            var errors = new List<string>();
            var warnings = new List<string>();

            try
            {
                // Required fields
               // baseCost.ProductCode = GetCellValueFromDataRow(dataRow, columnMapping, "ProductCode")?.Trim();
                baseCost.ProductName = GetCellValueFromDataRow(dataRow, columnMapping, "ProductName")?.Trim();
                baseCost.SupplierName = GetCellValueFromDataRow(dataRow, columnMapping, "SupplierName")?.Trim();

                // Optional fields
                baseCost.Remark = GetCellValueFromDataRow(dataRow, columnMapping, "Remark")?.Trim();

                // Decimal fields
                if (decimal.TryParse(GetCellValueFromDataRow(dataRow, columnMapping, "BaseCost"), out decimal cost))
                    baseCost.BaseCost = cost;

                // Boolean fields
                baseCost.IsActive = ParseBoolean(GetCellValueFromDataRow(dataRow, columnMapping, "IsActive"), true);

                // Date fields
                baseCost.StartDate = ParseDate(GetCellValueFromDataRow(dataRow, columnMapping, "StartDate"));
                baseCost.EndDate = ParseDate(GetCellValueFromDataRow(dataRow, columnMapping, "EndDate"));

                // Validate required fields
                if (string.IsNullOrEmpty(baseCost.ProductName))
                    errors.Add("Either Product Code or Product Name is required");
                if (!baseCost.BaseCost.HasValue || baseCost.BaseCost <= 0)
                    errors.Add("Base Cost is required and must be greater than 0");
                if (!baseCost.StartDate.HasValue)
                    errors.Add("Start Date is required");
            }
            catch (Exception ex)
            {
                errors.Add($"Error parsing row data: {ex.Message}");
            }

            baseCost.ValidationErrors = errors;
            baseCost.ValidationWarnings = warnings;
            baseCost.HasErrors = errors.Count > 0;

            return baseCost;
        }

        private PromotionCostImport ReadPromotionCostFromDataRow(DataRow dataRow, Dictionary<string, int> columnMapping)
        {
            var promotionCost = new PromotionCostImport();
            var errors = new List<string>();
            var warnings = new List<string>();

            try
            {
                // Required fields 
                promotionCost.ProductName = GetCellValueFromDataRow(dataRow, columnMapping, "ProductName")?.Trim();
                promotionCost.SupplierName = GetCellValueFromDataRow(dataRow, columnMapping, "SupplierName")?.Trim();

                // Optional fields
                promotionCost.Remark = GetCellValueFromDataRow(dataRow, columnMapping, "Remark")?.Trim();

                // Decimal fields
                if (decimal.TryParse(GetCellValueFromDataRow(dataRow, columnMapping, "PromotionCost"), out decimal cost))
                    promotionCost.PromotionCost = cost;

                // Boolean fields
                promotionCost.IsActive = ParseBoolean(GetCellValueFromDataRow(dataRow, columnMapping, "IsActive"), true);

                // Date fields
                promotionCost.StartDate = ParseDate(GetCellValueFromDataRow(dataRow, columnMapping, "StartDate"));
                promotionCost.EndDate = ParseDate(GetCellValueFromDataRow(dataRow, columnMapping, "EndDate"));

                // Validate required fields
                if (string.IsNullOrEmpty(promotionCost.ProductName))
                    errors.Add("Either Product Code or Product Name is required");
                if (!promotionCost.PromotionCost.HasValue || promotionCost.PromotionCost <= 0)
                    errors.Add("Promotion Cost is required and must be greater than 0");
                if (!promotionCost.StartDate.HasValue)
                    errors.Add("Start Date is required");
                if (!promotionCost.EndDate.HasValue)
                    errors.Add("End Date is required");
            }
            catch (Exception ex)
            {
                errors.Add($"Error parsing row data: {ex.Message}");
            }

            promotionCost.ValidationErrors = errors;
            promotionCost.ValidationWarnings = warnings;
            promotionCost.HasErrors = errors.Count > 0;

            return promotionCost;
        }

        private string GetCellValueFromDataRow(DataRow dataRow, Dictionary<string, int> columnMapping, string columnName)
        {
            if (columnMapping.TryGetValue(columnName, out int colIndex) && colIndex < dataRow.Table.Columns.Count)
            {
                return dataRow[colIndex]?.ToString();
            }
            return null;
        }

        private bool ParseBoolean(string value, bool defaultValue = false)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;

            value = value.Trim().ToLowerInvariant();
            return value == "true" || value == "1" || value == "yes" || value == "active";
        }

        private DateTime? ParseDate(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;

            if (DateTime.TryParse(value, out DateTime date))
                return date;

            return null;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidColumnName(string columnName, string importType = "Supplier")
        {
            var validColumns = GetValidColumnsFromConfig(importType);
            return validColumns.Contains(columnName, StringComparer.OrdinalIgnoreCase);
        }

        private List<string> GetValidColumnsFromConfig(string importType = "Supplier")
        {
            try
            {
                var configFileName = $"{importType.ToLower()}-import-columns.json";
                var configPath = Path.Combine(_environment.WebRootPath, "config", configFileName);
                if (File.Exists(configPath))
                {
                    var jsonContent = File.ReadAllText(configPath);
                    var config = JsonSerializer.Deserialize<ImportConfig>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    // Handle new unified format
                    if (config?.ImportColumns?.Columns != null)
                    {
                        return config.ImportColumns.Columns.Select(c => c.Name).ToList();
                    }
                    // Legacy format support
                    else
                    {
                        var allColumns = new List<string>();
                        allColumns.AddRange(config?.ImportColumns?.RequiredColumns?.Select(c => c.Name) ?? new List<string>());
                        allColumns.AddRange(config?.ImportColumns?.OptionalColumns?.Select(c => c.Name) ?? new List<string>());
                        return allColumns;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading column configuration for {ImportType}, using default columns", importType);
            }

            // Fallback to hardcoded columns based on import type
            return GetDefaultColumnsForImportType(importType);
        }


      

        private List<string> GetRequiredColumnsFromConfig(string importType = "Supplier")
        {
            try
            {
                var configFileName = $"{importType.ToLower()}-import-columns.json";
                var configPath = Path.Combine(_environment.WebRootPath, "config", configFileName);
                if (File.Exists(configPath))
                {
                    var jsonContent = File.ReadAllText(configPath);
                    var config = JsonSerializer.Deserialize<ImportConfig>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    // Handle new unified format
                    if (config?.ImportColumns?.Columns != null)
                    {
                        return config.ImportColumns.Columns.Where(c => c.Required).Select(c => c.Name).ToList();
                    }
                    // Legacy format support
                    else if (config?.ImportColumns?.RequiredColumns != null)
                    {
                        return config.ImportColumns.RequiredColumns.Select(c => c.Name).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading column configuration for {ImportType}, using default columns", importType);
            }

            // Fallback to hardcoded columns based on import type
            return GetDefaultRequiredColumnsForImportType(importType);
        }

        private Dictionary<string, Dictionary<string, string>> LoadAllColumnMappings()
        {
            var mappings = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

            // Initialize empty mappings for each import type
            mappings["Supplier"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            mappings["Product"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            mappings["BaseCost"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            mappings["PromotionCost"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Load mappings from config files if they exist
            foreach (var importType in mappings.Keys.ToList())
            {
                try
                {
                    var configFileName = $"{importType.ToLower()}-import-columns.json";
                    var configPath = Path.Combine(_environment.WebRootPath, "config", configFileName);
                    if (File.Exists(configPath))
                    {
                        var jsonContent = File.ReadAllText(configPath);
                        var config = JsonSerializer.Deserialize<ImportConfig>(jsonContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        // Load column mappings if available in config
                        if (config?.ColumnMappings != null)
                        {
                            mappings[importType] = config.ColumnMappings;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading column mapping for {ImportType}", importType);
                }
            }

            return mappings;
        }

        private List<string> GetDefaultColumnsForImportType(string importType)
        {
            return importType.ToUpper() switch
            {
                "SUPPLIER" => new List<string>
                {
                    "SupplierName", "SKUCode", "Email", "FirstName", "LastName", "Phone",
                    "Address1", "Address2", "Description", "IsActive", "IsPreferred",
                    "LeadTimeDays", "MoqCase", "LastCost", "Incoterm", "ValidFrom", "ValidTo"
                },
                "PRODUCT" => new List<string>
                {
                    "Code", "ProductName", "ProductDescription", "BrandName", "CategoryName", "SubCategoryName",
                    "InnerEan", "OuterEan", "UnitSize", "Upc", "LayerQuantity", "PalletQuantity", "CasePrice",
                    "ShelfLifeInWeeks", "PackHeight", "PackDepth", "PackWidth", "NetCaseWeightKg", "GrossCaseWeightKg",
                    "IsActive", "isTaxable", "SupplierName", "ExpiryDate", "TaxslabName"
                },
                "BASECOST" => new List<string>
                {
                    "ProductCode", "ProductName", "BaseCost", "StartDate", "EndDate", "Remark", "SupplierName", "IsActive"
                },
                "PROMOTIONCOST" => new List<string>
                {
                    "ProductCode", "ProductName", "PromotionCost", "StartDate", "EndDate", "Remark", "SupplierName", "IsActive"
                },
                _ => new List<string>()
            };
        }

        private List<string> GetDefaultRequiredColumnsForImportType(string importType)
        {
            return importType.ToUpper() switch
            {
                "SUPPLIER" => new List<string> { "SupplierName", "SKUCode", "Email" },
                "PRODUCT" => new List<string> { "Code", "ProductName" },
                "BASECOST" => new List<string> { "ProductCode", "BaseCost", "StartDate" },
                "PROMOTIONCOST" => new List<string> { "ProductCode", "PromotionCost", "StartDate", "EndDate" },
                _ => new List<string>()
            };
        }

        // Configuration classes for JSON deserialization (Generic for all import types)
        private class ImportConfig
        {
            public ImportColumns ImportColumns { get; set; }
            public Dictionary<string, string> ColumnMappings { get; set; }
        }

        private class ImportColumns
        {
            // New unified format
            public List<ColumnDefinition> Columns { get; set; }
            
            // Legacy format support
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
            public double? Min { get; set; }
            public double? Max { get; set; }
            public int? Precision { get; set; }
            public List<string> AcceptedValues { get; set; }
            public object DefaultValue { get; set; }
            public string DataType { get; set; } // Legacy support
            public string ValidationRules { get; set; } // Legacy support
        }

        // Legacy configuration classes for backward compatibility
        private class SupplierImportConfig
        {
            public SupplierImportColumns SupplierImportColumns { get; set; }
        }

        private class SupplierImportColumns
        {
            public List<ColumnDefinition> RequiredColumns { get; set; }
            public List<ColumnDefinition> OptionalColumns { get; set; }
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        private ImportConfig GetImportConfig(string importType)
        {
            try
            {
                var configFileName = $"{importType.ToLower()}-import-columns.json";
                var configPath = Path.Combine(_environment.WebRootPath, "config", configFileName);
                if (!File.Exists(configPath))
                {
                    return null;
                }

                var jsonContent = File.ReadAllText(configPath);
                var config = JsonSerializer.Deserialize<ImportConfig>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return config;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading import configuration for {ImportType}", importType);
                return null;
            }
        }

        private List<string> ValidateImportModelFromConfig(object model, string importType)
        {
            var errors = new List<string>();

            try
            {
                var config = GetImportConfig(importType);
                if (config?.ImportColumns == null) return errors;

                var modelType = model.GetType();
                var allColumns = new List<ColumnDefinition>();

                // Handle new unified format
                if (config.ImportColumns.Columns != null)
                {
                    allColumns.AddRange(config.ImportColumns.Columns);
                }
                // Legacy format support
                else
                {
                    // Combine required and optional columns
                    if (config.ImportColumns.RequiredColumns != null)
                        allColumns.AddRange(config.ImportColumns.RequiredColumns);
                    if (config.ImportColumns.OptionalColumns != null)
                        allColumns.AddRange(config.ImportColumns.OptionalColumns);
                }

                foreach (var column in allColumns)
                {
                    var columnName = column?.Name;
                    if (string.IsNullOrEmpty(columnName)) continue;

                    var property = modelType.GetProperty(columnName);
                    if (property == null) continue;

                    var value = property.GetValue(model);
                    var stringValue = value?.ToString();
                    var displayName = column.DisplayName ?? columnName;
                    var isRequired = column.Required;
                    var columnType = column.Type ?? column.DataType; // Support both new and legacy field names
                    var maxLength = column.MaxLength;

                    // Required field validation
                    if (isRequired && string.IsNullOrEmpty(stringValue))
                    {
                        errors.Add($"{displayName} is required");
                        continue;
                    }

                    // Skip further validation if field is empty and not required
                    if (string.IsNullOrEmpty(stringValue)) continue;

                    // Length validation
                    if (maxLength.HasValue && stringValue.Length > maxLength.Value)
                    {
                        errors.Add($"{displayName} exceeds {maxLength.Value} characters");
                    }

                    // Type-specific validation
                    switch (columnType?.ToLower())
                    {
                        case "email":
                            if (!IsValidEmail(stringValue))
                                errors.Add($"Invalid {displayName} format");
                            break;
                        case "integer":
                            if (!int.TryParse(stringValue, out _))
                                errors.Add($"{displayName} must be a valid integer");
                            else
                            {
                                var intValue = int.Parse(stringValue);
                                if (column.Min.HasValue && intValue < column.Min.Value)
                                    errors.Add($"{displayName} must be at least {column.Min.Value}");
                                if (column.Max.HasValue && intValue > column.Max.Value)
                                    errors.Add($"{displayName} must not exceed {column.Max.Value}");
                            }
                            break;
                        case "decimal":
                            if (!decimal.TryParse(stringValue, out _))
                                errors.Add($"{displayName} must be a valid decimal number");
                            else
                            {
                                var decimalValue = decimal.Parse(stringValue);
                                if (column.Min.HasValue && decimalValue < (decimal)column.Min.Value)
                                    errors.Add($"{displayName} must be at least {column.Min.Value}");
                            }
                            break;
                        case "date":
                            if (!DateTime.TryParse(stringValue, out _))
                                errors.Add($"{displayName} must be a valid date");
                            break;
                        case "boolean":
                            if (column.AcceptedValues != null && column.AcceptedValues.Any())
                            {
                                var acceptedStrings = column.AcceptedValues.Select(v => v.ToLower()).ToList();
                                if (!acceptedStrings.Contains(stringValue.ToLower()))
                                {
                                    errors.Add($"{displayName} must be one of: {string.Join(", ", column.AcceptedValues)}");
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Validation configuration error: {ex.Message}");
            }

            return errors;
        }
    }
}
