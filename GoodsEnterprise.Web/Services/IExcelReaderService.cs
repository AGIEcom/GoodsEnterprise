using GoodsEnterprise.Model.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoodsEnterprise.Web.Services
{
    /// <summary>
    /// Interface for Excel reading operations - Generic support for all import types
    /// </summary>
    public interface IExcelReaderService
    {
        /// <summary>
        /// Read data from Excel file for any import type
        /// </summary>
        /// <typeparam name="T">Import model type that implements IImportModel</typeparam>
        /// <param name="file">Excel file to read</param>
        /// <param name="importType">Type of import (Supplier, Product, BaseCost, PromotionCost)</param>
        /// <returns>List of import objects with validation metadata</returns>
        Task<ExcelReadResult<T>> ReadFromExcelAsync<T>(IFormFile file, string importType) where T : class, IImportModel, new();

        /// <summary>
        /// Read supplier data from Excel file (backward compatibility)
        /// </summary>
        /// <param name="file">Excel file to read</param>
        /// <returns>List of supplier import objects with validation metadata</returns>
        Task<ExcelReadResult<SupplierImport>> ReadSuppliersFromExcelAsync(IFormFile file);

        /// <summary>
        /// Read product data from Excel file
        /// </summary>
        /// <param name="file">Excel file to read</param>
        /// <returns>List of product import objects with validation metadata</returns>
        Task<ExcelReadResult<ProductImport>> ReadProductsFromExcelAsync(IFormFile file);

        /// <summary>
        /// Read base cost data from Excel file
        /// </summary>
        /// <param name="file">Excel file to read</param>
        /// <returns>List of base cost import objects with validation metadata</returns>
        Task<ExcelReadResult<BaseCostImport>> ReadBaseCostsFromExcelAsync(IFormFile file);

        /// <summary>
        /// Read promotion cost data from Excel file
        /// </summary>
        /// <param name="file">Excel file to read</param>
        /// <returns>List of promotion cost import objects with validation metadata</returns>
        Task<ExcelReadResult<PromotionCostImport>> ReadPromotionCostsFromExcelAsync(IFormFile file);

        /// <summary>
        /// Validate Excel file format and structure
        /// </summary>
        /// <param name="file">Excel file to validate</param>
        /// <returns>Validation result</returns>
        Task<FileValidationResult> ValidateExcelFileAsync(IFormFile file);

        /// <summary>
        /// Get column mapping from Excel headers
        /// </summary>
        /// <param name="file">Excel file</param>
        /// <param name="importType">Type of import for column validation</param>
        /// <returns>Column mapping dictionary</returns>
        Task<Dictionary<string, int>> GetColumnMappingAsync(IFormFile file, string importType = "Supplier");
    }

    /// <summary>
    /// Result of Excel reading operation
    /// </summary>
    /// <typeparam name="T">Type of data being read</typeparam>
    public class ExcelReadResult<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public int TotalRows { get; set; }
        public int ValidRows { get; set; }
        public int ErrorRows { get; set; }
        public bool IsSuccess => Errors.Count == 0;
        public Dictionary<string, int> ColumnMapping { get; set; } = new Dictionary<string, int>();
    }

    /// <summary>
    /// File validation result
    /// </summary>
    public class FileValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public long FileSize { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
    }
}
