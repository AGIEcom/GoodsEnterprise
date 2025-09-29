using GoodsEnterprise.Model.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoodsEnterprise.Web.Services
{
    /// <summary>
    /// Interface for product import operations
    /// </summary>
    public interface IProductImportService
    {
        /// <summary>
        /// Import products from Excel file
        /// </summary>
        /// <param name="file">Excel file containing product data</param>
        /// <param name="validateData">Whether to perform data validation</param>
        /// <param name="userId">ID of user performing the import</param>
        /// <returns>Import result</returns>
        Task<ProductImportResult> ImportProductsAsync(IFormFile file, bool validateData = true, int? userId = null);

        /// <summary>
        /// Preview import data without saving to database
        /// </summary>
        /// <param name="file">Excel file to preview</param>
        /// <param name="validateData">Whether to perform validation</param>
        /// <returns>Preview result</returns>
        Task<ProductImportPreview> PreviewImportAsync(IFormFile file, bool validateData = true);

        /// <summary>
        /// Get import progress for a specific import session
        /// </summary>
        /// <param name="importId">Import session ID</param>
        /// <returns>Import progress information</returns>
        Task<ImportProgress> GetImportProgressAsync(string importId);

        /// <summary>
        /// Cancel an ongoing import operation
        /// </summary>
        /// <param name="importId">Import session ID</param>
        /// <returns>Cancellation result</returns>
        Task<bool> CancelImportAsync(string importId);
    }

    /// <summary>
    /// Result of product import operation
    /// </summary>
    public class ProductImportResult
    {
        public string ImportId { get; set; } = Guid.NewGuid().ToString();
        public bool IsSuccess { get; set; }
        public int TotalRecords { get; set; }
        public int SuccessfulImports { get; set; }
        public int FailedImports { get; set; }
        public int SkippedRecords { get; set; }
        public List<string> GlobalErrors { get; set; } = new List<string>();
        public List<string> GlobalWarnings { get; set; } = new List<string>();
        public List<ProductImport> SuccessfulProducts { get; set; } = new List<ProductImport>();
        public List<ProductImport> FailedProducts { get; set; } = new List<ProductImport>();
        public List<DuplicateInfo> Duplicates { get; set; } = new List<DuplicateInfo>();
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan? Duration => EndTime?.Subtract(StartTime);
        public string Status { get; set; } = "Completed";
    }

    /// <summary>
    /// Preview result for product import data
    /// </summary>
    public class ProductImportPreview
    {
        public int TotalRecords { get; set; }
        public int ValidRecords { get; set; }
        public int InvalidRecords { get; set; }
        public List<ProductImport> SampleValidRecords { get; set; } = new List<ProductImport>();
        public List<ProductImport> SampleInvalidRecords { get; set; } = new List<ProductImport>();
        public List<string> ColumnHeaders { get; set; } = new List<string>();
        public List<string> MissingRequiredColumns { get; set; } = new List<string>();
        public List<string> UnrecognizedColumns { get; set; } = new List<string>();
        public List<DuplicateInfo> Duplicates { get; set; } = new List<DuplicateInfo>();
        public bool CanProceedWithImport { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> Errors { get; set; } = new List<string>();
    }
}
