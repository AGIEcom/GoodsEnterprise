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

namespace GoodsEnterprise.Web.Services
{
    /// <summary>
    /// Service for reading Excel files and converting to supplier import objects
    /// </summary>
    public class ExcelReaderService : IExcelReaderService
    {
        private readonly ILogger<ExcelReaderService> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly Dictionary<string, string> _columnMapping;

        public ExcelReaderService(ILogger<ExcelReaderService> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
            _columnMapping = LoadColumnMapping();

            // Register ExcelDataReader encoding provider
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public async Task<ExcelReadResult<SupplierImport>> ReadSuppliersFromExcelAsync(IFormFile file)
        {
            var result = new ExcelReadResult<SupplierImport>();

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
                result.ColumnMapping = GetColumnMappingFromDataTable(dataTable);

                if (result.ColumnMapping.Count() == 0)
                {
                    result.Errors.Add("No valid columns found. Please ensure the first row contains column headers.");
                    return result;
                }

                // Validate required columns 
                var requiredColumns = GetRequiredColumnsFromConfig();
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
                        var supplier = ReadSupplierFromDataRow(dataTable.Rows[i], result.ColumnMapping);
                        supplier.RowNumber = i + 1;

                        result.Data.Add(supplier);

                        if (supplier.HasErrors)
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

        public async Task<Dictionary<string, int>> GetColumnMappingAsync(IFormFile file)
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

                return GetColumnMappingFromDataTable(dataSet.Tables[0]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting column mapping from file: {FileName}", file.FileName);
                return new Dictionary<string, int>();
            }
        }

        private Dictionary<string, int> GetColumnMappingFromDataTable(DataTable dataTable)
        {
            var mapping = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            if (dataTable?.Columns == null) return mapping;

            for (int col = 0; col < dataTable.Columns.Count; col++)
            {
                var headerValue = dataTable.Columns[col].ColumnName?.Trim();
                if (!string.IsNullOrEmpty(headerValue))
                {
                    // Direct mapping first
                    if (_columnMapping.ContainsKey(headerValue))
                    {
                        mapping[_columnMapping[headerValue]] = col;
                    }
                    // Try exact match
                    else if (IsValidColumnName(headerValue))
                    {
                        mapping[headerValue] = col;
                    }
                }
            }

            return mapping;
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

                // Validate required fields
                if (string.IsNullOrEmpty(supplier.SupplierName))
                    errors.Add("Supplier Name is required");

                if (string.IsNullOrEmpty(supplier.SKUCode))
                    errors.Add("SKU Code is required");

                if (string.IsNullOrEmpty(supplier.Email))
                    errors.Add("Email is required");
                else if (!IsValidEmail(supplier.Email))
                    errors.Add("Invalid email format");

                // Validate field lengths
                if (!string.IsNullOrEmpty(supplier.SupplierName) && supplier.SupplierName.Length > 100)
                    errors.Add("Supplier Name exceeds 100 characters");

                if (!string.IsNullOrEmpty(supplier.SKUCode) && supplier.SKUCode.Length > 50)
                    errors.Add("SKU Code exceeds 50 characters");

                // Add more validations as needed...

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

        private bool IsValidColumnName(string columnName)
        {
            var validColumns = GetValidColumnsFromConfig();
            return validColumns.Contains(columnName, StringComparer.OrdinalIgnoreCase);
        }

        private List<string> GetValidColumnsFromConfig()
        {
            try
            {
                var configPath = Path.Combine(_environment.WebRootPath, "config", "supplier-import-columns.json");
                if (File.Exists(configPath))
                {
                    var jsonContent = File.ReadAllText(configPath);
                    var config = JsonSerializer.Deserialize<SupplierImportConfig>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    var allColumns = new List<string>();
                    allColumns.AddRange(config?.SupplierImportColumns?.RequiredColumns?.Select(c => c.Name) ?? new List<string>());
                    allColumns.AddRange(config?.SupplierImportColumns?.OptionalColumns?.Select(c => c.Name) ?? new List<string>());
                    return allColumns;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading column configuration, using default columns");
            }

            // Fallback to hardcoded columns if config loading fails
            return new List<string>
            {
                "SupplierName", "SKUCode", "Email", "FirstName", "LastName", "Phone",
                "Address1", "Address2", "Description", "IsActive", "IsPreferred",
                "LeadTimeDays", "MoqCase", "LastCost", "Incoterm", "ValidFrom", "ValidTo"
            };
        }

        private List<string> GetRequiredColumnsFromConfig()
        {
            try
            {
                var configPath = Path.Combine(_environment.WebRootPath, "config", "supplier-import-columns.json");
                if (File.Exists(configPath))
                {
                    var jsonContent = File.ReadAllText(configPath);
                    var config = JsonSerializer.Deserialize<SupplierImportConfig>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return config?.SupplierImportColumns?.RequiredColumns?.Select(c => c.Name).ToList()
                   ?? new List<string> { "SupplierName", "SKUCode", "Email" };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading column configuration, using default columns");
            }

            // Fallback to hardcoded columns if config loading fails
            return new List<string>
            {
                "SupplierName", "SKUCode", "Email"
            };
        }

        private Dictionary<string, string> LoadColumnMapping()
        {
            // Load column mapping from JSON config if needed
            // For now, return empty dictionary as we're using direct column name matching
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
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
    }
}
