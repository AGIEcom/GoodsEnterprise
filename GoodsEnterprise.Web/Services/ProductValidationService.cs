using GoodsEnterprise.Model.Models;
using GoodsEnterprise.DataAccess.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

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
        private readonly ILogger<ProductValidationService> _logger;

        public ProductValidationService(
            IGeneralRepository<Product> productRepository,
            IGeneralRepository<Brand> brandRepository,
            IGeneralRepository<Category> categoryRepository,
            ILogger<ProductValidationService> logger)
        {
            _productRepository = productRepository;
            _brandRepository = brandRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<BatchValidationResult<ProductImport>> ValidateProductsAsync(List<ProductImport> products)
        {
            var result = new BatchValidationResult<ProductImport>
            {
                TotalRecords = products.Count
            };

            try
            {
                // Get existing data for validation
                var productCodes = products.Select(p => p.Code).Where(c => !string.IsNullOrEmpty(c)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                var brandNames = products.Select(p => p.BrandName).Where(n => !string.IsNullOrEmpty(n)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                var categoryNames = products.Select(p => p.CategoryName).Where(n => !string.IsNullOrEmpty(n)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                var existingProducts = await _productRepository.GetAllAsync(filter: p => productCodes.Contains(p.Code));
                var existingBrands = await _brandRepository.GetAllAsync(filter: b => brandNames.Contains(b.Name));
                var existingCategories = await _categoryRepository.GetAllAsync(filter: c => categoryNames.Contains(c.Name));

                var existingBrandNames = existingBrands.Select(b => b.Name?.ToLower()).Where(n => !string.IsNullOrEmpty(n)).ToHashSet();
                var existingCategoryNames = existingCategories.Select(c => c.Name?.ToLower()).Where(n => !string.IsNullOrEmpty(n)).ToHashSet();

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

                    //if (!string.IsNullOrEmpty(product.Barcode))
                    //{
                    //    var barcodeLower = product.Barcode.ToLower();
                    //    if (!batchBarcodes.ContainsKey(barcodeLower))
                    //    {
                    //        batchBarcodes[barcodeLower] = new List<int>();
                    //    }
                    //    batchBarcodes[barcodeLower].Add(i + 1);
                    //}

                    // Check for duplicates in existing data
                    var existingDuplicate = existingProducts.FirstOrDefault(p => 
                        p.Code?.ToLower() == product.Code?.ToLower());

                    if (existingDuplicate != null)
                    {
                        result.Duplicates.Add(new DuplicateInfo
                        {
                            RowNumber = product.RowNumber,
                            Field = "Code",
                            Value = product.Code,
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

        public async Task<ValidationResult> ValidateProductAsync(ProductImport product)
        {
            var result = new ValidationResult { IsValid = true };

            try
            {
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

            }
            catch (Exception ex)
            {
                result.Errors.Add($"Validation error: {ex.Message}");
                result.IsValid = false;
                _logger.LogError(ex, "Error validating product: {Code}", product.Code);
            }

            return result;
        }
    }
}
