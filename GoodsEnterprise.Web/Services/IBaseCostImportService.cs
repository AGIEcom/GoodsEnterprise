using GoodsEnterprise.Model.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoodsEnterprise.Web.Services
{
    /// <summary>
    /// Interface for base cost import operations
    /// </summary>
    public interface IBaseCostImportService
    {
        /// <summary>
        /// Import base costs from Excel file
        /// </summary>
        /// <param name="file">Excel file containing base cost data</param>
        /// <param name="validateData">Whether to perform data validation</param>
        /// <param name="userId">ID of user performing the import</param>
        /// <returns>Import result</returns>
        Task<BaseCostImportResult> ImportBaseCostsAsync(IFormFile file, bool validateData = true, int? userId = null);

        /// <summary>
        /// Preview import data without saving to database
        /// </summary>
        /// <param name="file">Excel file to preview</param>
        /// <param name="validateData">Whether to perform validation</param>
        /// <returns>Preview result</returns>
        Task<BaseCostImportPreview> PreviewImportAsync(IFormFile file, bool validateData = true);

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
    /// Result of base cost import operation
    /// </summary>
    public class BaseCostImportResult
    {
        public string ImportId { get; set; } = Guid.NewGuid().ToString();
        public bool IsSuccess { get; set; }
        public int TotalRecords { get; set; }
        public int SuccessfulImports { get; set; }
        public int FailedImports { get; set; }
        public int SkippedRecords { get; set; }
        public List<string> GlobalErrors { get; set; } = new List<string>();
        public List<string> GlobalWarnings { get; set; } = new List<string>();
        public List<BaseCostImport> SuccessfulBaseCosts { get; set; } = new List<BaseCostImport>();
        public List<BaseCostImport> FailedBaseCosts { get; set; } = new List<BaseCostImport>();
        public List<DuplicateInfo> Duplicates { get; set; } = new List<DuplicateInfo>();
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan? Duration => EndTime?.Subtract(StartTime);
        public string Status { get; set; } = "Completed";
    }

    /// <summary>
    /// Preview result for base cost import data
    /// </summary>
    public class BaseCostImportPreview
    {
        public int TotalRecords { get; set; }
        public int ValidRecords { get; set; }
        public int InvalidRecords { get; set; }
        public List<BaseCostImport> SampleValidRecords { get; set; } = new List<BaseCostImport>();
        public List<BaseCostImport> SampleInvalidRecords { get; set; } = new List<BaseCostImport>();
        public List<string> ColumnHeaders { get; set; } = new List<string>();
        public List<string> MissingRequiredColumns { get; set; } = new List<string>();
        public List<string> UnrecognizedColumns { get; set; } = new List<string>();
        public List<DuplicateInfo> Duplicates { get; set; } = new List<DuplicateInfo>();
        public bool CanProceedWithImport { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> Errors { get; set; } = new List<string>();
    }
}
