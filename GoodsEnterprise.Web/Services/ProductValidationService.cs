using GoodsEnterprise.DataAccess.Interface;
using GoodsEnterprise.Model.Models;
using GoodsEnterprise.Web.Pages;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static GoodsEnterprise.Web.Services.IProductValidationService;

namespace GoodsEnterprise.Web.Services
{
    /// <summary>
    /// Service for validating product import data
    /// </summary>
    public class ProductValidationService : IProductValidationService
    {
        private readonly IGeneralRepository<Product> _productRepository;
        private readonly IGeneralRepository<Brand> _brandRepository;
        private readonly IGeneralRepository<Category> _categoryRepository;
        private readonly IGeneralRepository<SubCategory> _subCategoryRepository;
        private readonly IGeneralRepository<Supplier> _supplierRepository;
        private readonly ILogger<ProductValidationService> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private dynamic _configuration;

        public ProductValidationService(
            IGeneralRepository<Product> productRepository,
            IGeneralRepository<Brand> brandRepository,
            IGeneralRepository<Category> categoryRepository,
            IGeneralRepository<SubCategory> subCategoryRepository,
            IGeneralRepository<Supplier> supplierRepository,
            ILogger<ProductValidationService> logger,
            IWebHostEnvironment webHostEnvironment)
        {
            _productRepository = productRepository;
            _brandRepository = brandRepository;
            _categoryRepository = categoryRepository;
            _subCategoryRepository = subCategoryRepository;
            _supplierRepository = supplierRepository;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            LoadConfiguration();
        }
        private void LoadConfiguration()
        {
            try
            {
                var configPath = Path.Combine(_webHostEnvironment.WebRootPath, "config", "product-import-columns.json");
                if (File.Exists(configPath))
                {
                    var configJson = File.ReadAllText(configPath);
                    _configuration = JsonConvert.DeserializeObject(configJson);
                    _logger.LogInformation("Product import configuration loaded successfully");
                }
                else
                {
                    _logger.LogWarning("Product import configuration file not found at: {ConfigPath}", configPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Product import configuration");
            }
        }
        public async Task<BatchValidationResult<ProductImport>> ValidateProductsAsync1(List<ProductImport> products)
        {
            var result = new BatchValidationResult<ProductImport>
            {
                TotalRecords = products.Count
            };

            try
            {
                //string[] duplicates = products.AsEnumerable()
                //   .Select(dr => Convert.ToString(dr["OuterEAN"]))
                //   .GroupBy(x => x)
                //   .Where(g => g.Count() > 1)
                //   .Select(g => g.Key)
                //   .ToArray();

                //if (duplicates != null && duplicates.Count() > 0)
                //{
                //    string messages = string.Empty;
                //    foreach (string msg in duplicates)
                //    {
                //        messages += msg + "<br />";
                //    }
                //    ViewData["ValidationMsg"] = $"Below Outer EAN has duplicate values, please correct it and retry<br /><div style='max-height: 200px; overflow-y: auto; border: 1px solid #ccc; padding: 10px; background-color: #f9f9f9;'>{messages}</div>";
                //    return Page();
                //}

                // Get existing data for validation
                var productCodes = products.Select(p => p.Code).Where(c => !string.IsNullOrEmpty(c)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                var productOuterEANs = products.Select(p => p.OuterEan).Where(c => !string.IsNullOrEmpty(c)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                var brandNames = products.Select(p => p.BrandName).Where(n => !string.IsNullOrEmpty(n)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                var categoryNames = products.Select(p => p.CategoryName).Where(n => !string.IsNullOrEmpty(n)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                var subCategoryNames = products.Select(p => p.SubCategoryName).Where(n => !string.IsNullOrEmpty(n)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                var supplierNames = products.Select(p => p.SupplierName).Where(n => !string.IsNullOrEmpty(n)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                var existingProducts = await _productRepository.GetAllAsync(filter: p => productCodes.Contains(p.Code));
                var existingProductOuterEANs = await _productRepository.GetAllAsync(filter: p => productOuterEANs.Contains(p.OuterEan));
                var existingBrands = await _brandRepository.GetAllAsync(filter: b => brandNames.Contains(b.Name));
                var existingCategories = await _categoryRepository.GetAllAsync(filter: c => categoryNames.Contains(c.Name));
                var existingSubCategories = await _subCategoryRepository.GetAllAsync(filter: sc => subCategoryNames.Contains(sc.Name));
                var existingSuppliers = await _supplierRepository.GetAllAsync(filter: s => supplierNames.Contains(s.Name));

                var existingProductOuterEANNames = existingProductOuterEANs.Select(po => po.OuterEan?.ToLower()).Where(n => !string.IsNullOrEmpty(n)).ToHashSet();
                var existingBrandNames = existingBrands.Select(b => b.Name?.ToLower()).Where(n => !string.IsNullOrEmpty(n)).ToHashSet();
                var existingCategoryNames = existingCategories.Select(c => c.Name?.ToLower()).Where(n => !string.IsNullOrEmpty(n)).ToHashSet();
                var existingSubCategoryNames = existingSubCategories.Select(sc => sc.Name?.ToLower()).Where(n => !string.IsNullOrEmpty(n)).ToHashSet();
                var existingsupplierNames = existingSuppliers.Select(s => s.Name?.ToLower()).Where(n => !string.IsNullOrEmpty(n)).ToHashSet();

                // Track duplicates within the batch
                var batchProductCodes = new Dictionary<string, List<int>>();
                var batchBarcodes = new Dictionary<string, List<int>>();

                for (int i = 0; i < products.Count; i++)
                {
                    var product = products[i];
                    product.RowNumber = i + 1;

                    var validationResult = await ValidateProductAsync(product);

                    // Additional validation for related entities
                    if (!string.IsNullOrEmpty(product.BrandName))
                    {
                        if (!existingBrandNames.Contains(product.BrandName.ToLower()))
                        {
                            validationResult.Warnings.Add($"Brand '{product.BrandName}' not found in system");
                        }
                    }

                    if (!string.IsNullOrEmpty(product.CategoryName))
                    {
                        if (!existingCategoryNames.Contains(product.CategoryName.ToLower()))
                        {
                            validationResult.Warnings.Add($"Category '{product.CategoryName}' not found in system");
                        }
                    }

                    // Check for duplicates within batch
                    if (!string.IsNullOrEmpty(product.Code))
                    {
                        var productCodeLower = product.Code.ToLower();
                        if (!batchProductCodes.ContainsKey(productCodeLower))
                        {
                            batchProductCodes[productCodeLower] = new List<int>();
                        }
                        batchProductCodes[productCodeLower].Add(i + 1);
                    }

                    if (!string.IsNullOrEmpty(product.OuterEan))
                    {
                        var barcodeLower = product.OuterEan.ToLower();
                        if (!batchBarcodes.ContainsKey(barcodeLower))
                        {
                            batchBarcodes[barcodeLower] = new List<int>();
                        }
                        batchBarcodes[barcodeLower].Add(i + 1);
                    }

                    // Check for duplicates in existing data
                    //var existingDuplicate = existingProducts.FirstOrDefault(p => 
                    //    p.Code?.ToLower() == product.Code?.ToLower());

                    //if (existingDuplicate != null)
                    //{
                    //    result.Duplicates.Add(new DuplicateInfo
                    //    {
                    //        RowNumber = product.RowNumber,
                    //        Field = "Code",
                    //        Value = product.Code,
                    //        DuplicateType = "Database",
                    //        ExistingId = existingDuplicate.Id
                    //    });
                    //}

                    // Check for duplicates in existing data
                    var existingDuplicate = existingProductOuterEANs.FirstOrDefault(p =>
                        p.OuterEan?.ToLower() == product.OuterEan?.ToLower());

                    if (existingDuplicate != null)
                    {
                        result.Duplicates.Add(new DuplicateInfo
                        {
                            RowNumber = product.RowNumber,
                            Field = "OuterEan",
                            Value = product.OuterEan,
                            DuplicateType = "Database",
                            ExistingId = existingDuplicate.Id
                        });
                    }

                    // Add validation errors and warnings to the import object
                    product.ValidationErrors.AddRange(validationResult.Errors);
                    product.ValidationWarnings.AddRange(validationResult.Warnings);
                    product.HasErrors = validationResult.Errors.Any();
                    //product.HasWarnings = validationResult.Warnings.Any();

                    if (validationResult.IsValid && !product.HasErrors)
                    {
                        result.ValidRecords.Add(product);
                        result.ValidRecordCount++;
                    }
                    else
                    {
                        result.InvalidRecords.Add(product);
                        result.InvalidRecordCount++;
                    }
                }

                // Check for batch duplicates
                foreach (var kvp in batchProductCodes.Where(x => x.Value.Count > 1))
                {
                    foreach (var rowNumber in kvp.Value)
                    {
                        result.Duplicates.Add(new DuplicateInfo
                        {
                            RowNumber = rowNumber,
                            Field = "Code",
                            Value = kvp.Key,
                            DuplicateType = "Batch",
                            ConflictingRows = kvp.Value.Where(r => r != rowNumber).ToList()
                        });
                    }
                }

                foreach (var kvp in batchBarcodes.Where(x => x.Value.Count > 1))
                {
                    foreach (var rowNumber in kvp.Value)
                    {
                        result.Duplicates.Add(new DuplicateInfo
                        {
                            RowNumber = rowNumber,
                            Field = "Barcode",
                            Value = kvp.Key,
                            DuplicateType = "Batch",
                            ConflictingRows = kvp.Value.Where(r => r != rowNumber).ToList()
                        });
                    }
                }

                _logger.LogInformation("Product validation completed. Valid: {Valid}, Invalid: {Invalid}, Duplicates: {Duplicates}",
                    result.ValidRecordCount, result.InvalidRecordCount, result.Duplicates.Count);

            }
            catch (Exception ex)
            {
                result.GlobalErrors.Add($"Validation error: {ex.Message}");
                _logger.LogError(ex, "Error during product validation");
            }

            return result;
        }

        //public async Task<ValidationResult> ValidateProductAsync(ProductImport product)
        //{
        //    var result = new ValidationResult { IsValid = true };

        //    try
        //    {
        //        // Required field validation
        //        if (string.IsNullOrWhiteSpace(product.Code))
        //        {
        //            result.Errors.Add("Product Code is required");
        //            result.IsValid = false;
        //        }

        //        if (string.IsNullOrWhiteSpace(product.ProductName))
        //        {
        //            result.Errors.Add("Product Name is required");
        //            result.IsValid = false;
        //        }

        //        if (string.IsNullOrWhiteSpace(product.OuterEan))
        //        {
        //            result.Errors.Add("OuterEan is required");
        //            result.IsValid = false;
        //        }

        //        if (string.IsNullOrWhiteSpace(product.ExpiryDate))
        //        {
        //            result.Errors.Add("ExpiryDate is required");
        //            result.IsValid = false;
        //        }

        //        // String length validation
        //        if (!string.IsNullOrEmpty(product.Code) && product.Code.Length > 50)
        //        {
        //            result.Errors.Add("Product Code cannot exceed 50 characters");
        //            result.IsValid = false;
        //        }

        //        if (!string.IsNullOrEmpty(product.ProductName) && product.ProductName.Length > 200)
        //        {
        //            result.Errors.Add("Product Name cannot exceed 200 characters");
        //            result.IsValid = false;
        //        }

        //        if (!string.IsNullOrEmpty(product.OuterEan) && product.OuterEan.Length > 200)
        //        {
        //            result.Errors.Add("Outer EAN cannot exceed 25 characters");
        //            result.IsValid = false;
        //        }

        //        if (!string.IsNullOrEmpty(product.BrandName) && product.BrandName.Length > 100)
        //        {
        //            result.Errors.Add("Brand Name cannot exceed 100 characters");
        //            result.IsValid = false;
        //        }

        //        if (!string.IsNullOrEmpty(product.CategoryName) && product.CategoryName.Length > 100)
        //        {
        //            result.Errors.Add("Category Name cannot exceed 100 characters");
        //            result.IsValid = false;
        //        }

        //        if (!string.IsNullOrEmpty(product.ProductDescription) && product.ProductDescription.Length > 1000)
        //        {
        //            result.Errors.Add("Description cannot exceed 1000 characters");
        //            result.IsValid = false;
        //        }

        //        // Numeric validation
        //        if (product.Upc.HasValue && product.Upc < 0)
        //        {
        //            result.Errors.Add("Unit Price cannot be negative");
        //            result.IsValid = false;
        //        }

        //        if (product.CasePrice.HasValue && product.CasePrice < 0)
        //        {
        //            result.Errors.Add("Cost Price cannot be negative");
        //            result.IsValid = false;
        //        }

        //        //if (product.Weight.HasValue && product.Weight < 0)
        //        //{
        //        //    result.Errors.Add("Weight cannot be negative");
        //        //    result.IsValid = false;
        //        //}

        //        //if (product.StockQuantity.HasValue && product.StockQuantity < 0)
        //        //{
        //        //    result.Errors.Add("Stock Quantity cannot be negative");
        //        //    result.IsValid = false;
        //        //}

        //        //if (product.MinStockLevel.HasValue && product.MinStockLevel < 0)
        //        //{
        //        //    result.Errors.Add("Min Stock Level cannot be negative");
        //        //    result.IsValid = false;
        //        //}

        //        //if (product.MaxStockLevel.HasValue && product.MaxStockLevel < 0)
        //        //{
        //        //    result.Errors.Add("Max Stock Level cannot be negative");
        //        //    result.IsValid = false;
        //        //}

        //        //// Business logic validation
        //        //if (product.MinStockLevel.HasValue && product.MaxStockLevel.HasValue && 
        //        //    product.MinStockLevel > product.MaxStockLevel)
        //        //{
        //        //    result.Errors.Add("Min Stock Level cannot be greater than Max Stock Level");
        //        //    result.IsValid = false;
        //        //}

        //        //if (product.UnitPrice.HasValue && product.CostPrice.HasValue && 
        //        //    product.CostPrice > product.UnitPrice)
        //        //{
        //        //    result.Warnings.Add("Cost Price is higher than Unit Price - check profit margin");
        //        //}

        //        //// Format validation
        //        //if (!string.IsNullOrEmpty(product.Barcode))
        //        //{
        //        //    // Basic barcode format validation (digits only, common lengths)
        //        //    if (!Regex.IsMatch(product.Barcode, @"^\d{8,14}$"))
        //        //    {
        //        //        result.Warnings.Add("Barcode format may be invalid - should be 8-14 digits");
        //        //    }
        //        //}

        //        //if (!string.IsNullOrEmpty(product.ImageUrl))
        //        //{
        //        //    // Basic URL format validation
        //        //    if (!Uri.TryCreate(product.ImageUrl, UriKind.Absolute, out _))
        //        //    {
        //        //        result.Warnings.Add("Image URL format may be invalid");
        //        //    }
        //        //}

        //        //// Range validation warnings
        //        //if (product.UnitPrice.HasValue && product.UnitPrice > 999999.99m)
        //        //{
        //        //    result.Warnings.Add("Unit Price is very high - please verify");
        //        //}

        //        //if (product.Weight.HasValue && product.Weight > 1000)
        //        //{
        //        //    result.Warnings.Add("Weight is very high - please verify units");
        //        //}

        //    }
        //    catch (Exception ex)
        //    {
        //        result.Errors.Add($"Validation error: {ex.Message}");
        //        result.IsValid = false;
        //        _logger.LogError(ex, "Error validating product: {Code}", product.Code);
        //    }

        //    return result;
        //}


        public async Task<ProductBatchValidationResult<ProductImport>> ValidateProductsAsync(List<ProductImport> products)
        {
            var result = new ProductBatchValidationResult<ProductImport>
            {
                TotalRecords = products.Count
            };

            try
            {
                // Track batch duplicates
                var batchDuplicates = new Dictionary<string, List<int>>();

                for (int i = 0; i < products.Count; i++)
                {
                    var product = products[i];
                    product.RowNumber = i + 1;

                    // Validate using JSON configuration or fallback
                    var validationResult = _configuration != null
                        ? await ValidateUsingJsonConfig(product)
                        : await ValidateFallbackRules(product);

                    // Business rules validation
                    //await ValidateBusinessRules(product, validationResult);

                    // Database duplicate check
                    await CheckDatabaseDuplicates(product, result);

                    // Track batch duplicates
                    TrackBatchDuplicates(product, batchDuplicates, product.RowNumber);

                    //Check duplicates in Batch
                    //ProcessBatchDuplicates(product, batchDuplicates, result);

                    // Add validation results to import object
                    product.ValidationErrors.AddRange(validationResult.Errors);
                    product.ValidationWarnings.AddRange(validationResult.Warnings);
                    product.HasErrors = validationResult.Errors.Any() == false ? product.HasErrors: validationResult.Errors.Any();

                    if (validationResult.IsValid && !product.HasErrors)
                    {
                        result.ValidRecords.Add(product);
                        result.ValidRecordCount++;
                    }
                    else
                    {
                        result.InvalidRecords.Add(product);
                        result.InvalidRecordCount++;
                    }
                }

                // Process batch duplicates
                ProcessBatchDuplicates(batchDuplicates, result);

                // Re-evaluate product lists after marking batch duplicates as errors
                ReEvaluateProductLists(result);

                _logger.LogInformation("Product validation completed. Valid: {Valid}, Invalid: {Invalid}, Duplicates: {Duplicates}",
                    result.ValidRecordCount, result.InvalidRecordCount, result.Duplicates.Count);

            }
            catch (Exception ex)
            {
                result.GlobalErrors.Add($"Validation error: {ex.Message}");
                _logger.LogError(ex, "Error during promotion cost validation");
            }

            return result;
        }

        private void TrackBatchDuplicates(ProductImport product, Dictionary<string, List<int>> batchDuplicates, int rowNumber)
        {
            var productIdentifier = product.OuterEan;

            if (string.IsNullOrEmpty(productIdentifier) || !product.ExpiryDate.HasValue || string.IsNullOrEmpty(product.Code))
                return;

            var key = $"{product.Code}({productIdentifier.ToLower()})";
            if (!batchDuplicates.ContainsKey(key))
            {
                batchDuplicates[key] = new List<int>();
            }
            batchDuplicates[key].Add(rowNumber);
        }
        private void ProcessBatchDuplicates(Dictionary<string, List<int>> batchDuplicates, ProductBatchValidationResult<ProductImport> result)
        {
            foreach (var kvp in batchDuplicates.Where(x => x.Value.Count > 1))
            {
                foreach (var rowNumber in kvp.Value)
                {
                    result.Duplicates.Add(new DuplicateInfo
                    {
                        RowNumber = rowNumber,
                        Field = "Product + OuterEAN",
                        Value = kvp.Key,
                        DuplicateType = "Batch",
                        ConflictingRows = kvp.Value.Where(r => r != rowNumber).ToList()
                    });

                    // Mark this product as having validation errors to exclude from ValidRecords
                    var product = result.TotalRecords > 0 ? FindProductByRowNumber(result, rowNumber) : null;
                    if (product != null)
                    {
                        product.ValidationErrors.Add($"Duplicate product found in batch: '{kvp.Key}' appears {kvp.Value.Count} times (rows: {string.Join(", ", kvp.Value)})");
                        product.HasErrors = true;
                    }
                }
            }
        }

        private void ReEvaluateProductLists(ProductBatchValidationResult<ProductImport> result)
        {
            // Move products that were marked as having errors (due to batch duplicates) from ValidRecords to InvalidRecords
            var productsToMove = result.ValidRecords.Where(p => p.HasErrors).ToList();

            foreach (var product in productsToMove)
            {
                result.ValidRecords.Remove(product);
                result.ValidRecordCount--;

                // Only add to InvalidRecords if not already there
                if (!result.InvalidRecords.Contains(product))
                {
                    result.InvalidRecords.Add(product);
                    result.InvalidRecordCount++;
                }
            }
        }

        private ProductImport FindProductByRowNumber(ProductBatchValidationResult<ProductImport> result, int rowNumber)
        {
            // Search in ValidRecords first, then InvalidRecords
            var product = result.ValidRecords.FirstOrDefault(p => p.RowNumber == rowNumber);
            if (product == null)
            {
                product = result.InvalidRecords.FirstOrDefault(p => p.RowNumber == rowNumber);
            }
            return product;
        }

        private async Task CheckDatabaseDuplicates(ProductImport product, ProductBatchValidationResult<ProductImport> result)
        {
            var productIdentifier = product.OuterEan;

            if (string.IsNullOrEmpty(productIdentifier) || !product.ExpiryDate.HasValue || string.IsNullOrEmpty(product.Code))
                return;

            var existingProducts = await _productRepository.GetAllAsync(filter: pc =>
                pc.OuterEan == productIdentifier && pc.Code == product.Code);

            if (existingProducts.Any())
            {
                // Add to duplicates list for reporting
                result.Duplicates.Add(new DuplicateInfo
                {
                    RowNumber = product.RowNumber,
                    Field = "Product + OuterEAN",
                    Value = $"{product.Code} ({productIdentifier})",
                    DuplicateType = "Database",
                    ExistingId = existingProducts.FirstOrDefault()?.Id
                });

                // Mark as having errors to exclude from ValidRecords
                product.ValidationErrors.Add($"Product with Code '{product.Code}' and OuterEAN '{productIdentifier}' already exists in database");
                product.HasErrors = true;
            }
        }

        private async Task<ProductValidationResult> ValidateFallbackRules(ProductImport product)
        {
            var result = new ProductValidationResult { IsValid = true };

                // Required field validation
                if (string.IsNullOrWhiteSpace(product.Code))
                {
                    result.Errors.Add("Product Code is required");
                    result.IsValid = false;
                }

                if (string.IsNullOrWhiteSpace(product.ProductName))
                {
                    result.Errors.Add("Product Name is required");
                    result.IsValid = false;
                }

                if (string.IsNullOrWhiteSpace(product.OuterEan))
                {
                    result.Errors.Add("OuterEan is required");
                    result.IsValid = false;
                }
                if (!product.ExpiryDate.HasValue)                
                {
                    result.Errors.Add("ExpiryDate is required");
                    result.IsValid = false;
                }

                // String length validation
                if (!string.IsNullOrEmpty(product.Code) && product.Code.Length > 50)
                {
                    result.Errors.Add("Product Code cannot exceed 50 characters");
                    result.IsValid = false;
                }

                if (!string.IsNullOrEmpty(product.ProductName) && product.ProductName.Length > 200)
                {
                    result.Errors.Add("Product Name cannot exceed 200 characters");
                    result.IsValid = false;
                }

                if (!string.IsNullOrEmpty(product.OuterEan) && product.OuterEan.Length > 200)
                {
                    result.Errors.Add("Outer EAN cannot exceed 25 characters");
                    result.IsValid = false;
                }

                if (!string.IsNullOrEmpty(product.BrandName) && product.BrandName.Length > 100)
                {
                    result.Errors.Add("Brand Name cannot exceed 100 characters");
                    result.IsValid = false;
                }

                if (!string.IsNullOrEmpty(product.CategoryName) && product.CategoryName.Length > 100)
                {
                    result.Errors.Add("Category Name cannot exceed 100 characters");
                    result.IsValid = false;
                }

                if (!string.IsNullOrEmpty(product.ProductDescription) && product.ProductDescription.Length > 1000)
                {
                    result.Errors.Add("Description cannot exceed 1000 characters");
                    result.IsValid = false;
                }

                // Numeric validation
                if (product.Upc.HasValue && product.Upc < 0)
                {
                    result.Errors.Add("Unit Price cannot be negative");
                    result.IsValid = false;
                }

                if (product.CasePrice.HasValue && product.CasePrice < 0)
                {
                    result.Errors.Add("Cost Price cannot be negative");
                    result.IsValid = false;
                }

                //if (product.Weight.HasValue && product.Weight < 0)
                //{
                //    result.Errors.Add("Weight cannot be negative");
                //    result.IsValid = false;
                //}

                //if (product.StockQuantity.HasValue && product.StockQuantity < 0)
                //{
                //    result.Errors.Add("Stock Quantity cannot be negative");
                //    result.IsValid = false;
                //}

                //if (product.MinStockLevel.HasValue && product.MinStockLevel < 0)
                //{
                //    result.Errors.Add("Min Stock Level cannot be negative");
                //    result.IsValid = false;
                //}

                //if (product.MaxStockLevel.HasValue && product.MaxStockLevel < 0)
                //{
                //    result.Errors.Add("Max Stock Level cannot be negative");
                //    result.IsValid = false;
                //}

                //// Business logic validation
                //if (product.MinStockLevel.HasValue && product.MaxStockLevel.HasValue && 
                //    product.MinStockLevel > product.MaxStockLevel)
                //{
                //    result.Errors.Add("Min Stock Level cannot be greater than Max Stock Level");
                //    result.IsValid = false;
                //}

                //if (product.UnitPrice.HasValue && product.CostPrice.HasValue && 
                //    product.CostPrice > product.UnitPrice)
                //{
                //    result.Warnings.Add("Cost Price is higher than Unit Price - check profit margin");
                //}

                //// Format validation
                //if (!string.IsNullOrEmpty(product.Barcode))
                //{
                //    // Basic barcode format validation (digits only, common lengths)
                //    if (!Regex.IsMatch(product.Barcode, @"^\d{8,14}$"))
                //    {
                //        result.Warnings.Add("Barcode format may be invalid - should be 8-14 digits");
                //    }
                //}

                //if (!string.IsNullOrEmpty(product.ImageUrl))
                //{
                //    // Basic URL format validation
                //    if (!Uri.TryCreate(product.ImageUrl, UriKind.Absolute, out _))
                //    {
                //        result.Warnings.Add("Image URL format may be invalid");
                //    }
                //}

                //// Range validation warnings
                //if (product.UnitPrice.HasValue && product.UnitPrice > 999999.99m)
                //{
                //    result.Warnings.Add("Unit Price is very high - please verify");
                //}

                //if (product.Weight.HasValue && product.Weight > 1000)
                //{
                //    result.Warnings.Add("Weight is very high - please verify units");
                //}
                return result;
        }

        private async Task<ProductValidationResult> ValidateUsingJsonConfig(ProductImport product)
        {
            var result = new ProductValidationResult { IsValid = true };

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
                            var value = GetPropertyValue(product, columnName);
                            if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
                            {
                                var error = $"{columnDisplayName} is required";
                                result.Errors.Add(error);
                                AddFieldError(result, columnName, error);
                                result.IsValid = false;
                            }
                        }

                        // Field-specific validation
                        ValidateFieldRules(product, column, result);
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

        private void ValidateFieldRules(ProductImport product, dynamic column, ProductValidationResult result)
        {
            var columnName = column.name != null ? column.name.ToString() : null;
            var columnDisplayName = column.displayName != null ? column.displayName.ToString() : columnName;

            if (string.IsNullOrWhiteSpace(columnName))
            {
                _logger.LogWarning("Skipping validation for column with missing name in config.");
                return;
            }

            var value = GetPropertyValue(product, columnName);
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
        private object GetPropertyValue(ProductImport product, string propertyName)
        {
            try
            {
                var property = typeof(ProductImport).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property != null)
                {
                    return property.GetValue(product);
                }

                // Log warning for missing property
                _logger.LogWarning("Property '{PropertyName}' not found in ProductImport model", propertyName);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting property value for '{PropertyName}'", propertyName);
                return null;
            }
        }

        private void AddFieldError(ProductValidationResult result, string fieldName, string error)
        {
            if (result.FieldErrors == null)
                result.FieldErrors = new Dictionary<string, List<string>>();

            if (!result.FieldErrors.ContainsKey(fieldName))
                result.FieldErrors[fieldName] = new List<string>();

            result.FieldErrors[fieldName].Add(error);
        }

        // Legacy method for backward compatibility
        public async Task<ProductValidationResult> ValidateProductAsync(ProductImport product)
        {
            var result = _configuration != null
                ? await ValidateUsingJsonConfig(product)
                : await ValidateFallbackRules(product);

            //await ValidateBusinessRules(product, result);

            return new ProductValidationResult
            {
                IsValid = result.IsValid,
                Errors = result.Errors,
                Warnings = result.Warnings
            };
        }

    }
}
