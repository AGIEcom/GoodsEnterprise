using GoodsEnterprise.Model.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoodsEnterprise.Web.Services
{
    /// <summary>
    /// Interface for supplier import operations
    /// </summary>
    public interface ISupplierImportService
    {
        /// <summary>
        /// Import suppliers from Excel file
        /// </summary>
        /// <param name="file">Excel file containing supplier data</param>
        /// <param name="validateData">Whether to perform data validation</param>
        /// <param name="userId">ID of user performing the import</param>
        /// <returns>Import result</returns>
        Task<SupplierImportResult> ImportSuppliersAsync(IFormFile file, bool validateData = true, int? userId = null);

        /// <summary>
        /// Preview import data without saving to database
        /// </summary>
        /// <param name="file">Excel file to preview</param>
        /// <param name="validateData">Whether to perform validation</param>
        /// <returns>Preview result</returns>
        Task<SupplierImportPreview> PreviewImportAsync(IFormFile file, bool validateData = true);

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
    /// Result of supplier import operation
    /// </summary>
    public class SupplierImportResult
    {
        public string ImportId { get; set; } = Guid.NewGuid().ToString();
        public bool IsSuccess { get; set; }
        public int TotalRecords { get; set; }
        public int SuccessfulImports { get; set; }
        public int FailedImports { get; set; }
        public int SkippedRecords { get; set; }
        public List<string> GlobalErrors { get; set; } = new List<string>();
        public List<string> GlobalWarnings { get; set; } = new List<string>();
        public List<SupplierImport> SuccessfulSuppliers { get; set; } = new List<SupplierImport>();
        public List<SupplierImport> FailedSuppliers { get; set; } = new List<SupplierImport>();
        public List<DuplicateInfo> Duplicates { get; set; } = new List<DuplicateInfo>();
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan? Duration => EndTime?.Subtract(StartTime);
        public string Status { get; set; } = "Completed";
    }

    /// <summary>
    /// Preview result for import data
    /// </summary>
    public class SupplierImportPreview
    {
        public int TotalRecords { get; set; }
        public int ValidRecords { get; set; }
        public int InvalidRecords { get; set; }
        public List<SupplierImport> SampleValidRecords { get; set; } = new List<SupplierImport>();
        public List<SupplierImport> SampleInvalidRecords { get; set; } = new List<SupplierImport>();
        public List<string> ColumnHeaders { get; set; } = new List<string>();
        public List<string> MissingRequiredColumns { get; set; } = new List<string>();
        public List<string> UnrecognizedColumns { get; set; } = new List<string>();
        public List<DuplicateInfo> Duplicates { get; set; } = new List<DuplicateInfo>();
        public bool CanProceedWithImport { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> Errors { get; set; } = new List<string>();
    }

    /// <summary>
    /// Import progress information
    /// </summary>
    public class ImportProgress
    {
        public string ImportId { get; set; }
        public string Status { get; set; } // "Processing", "Completed", "Failed", "Cancelled"
        public int TotalRecords { get; set; }
        public int ProcessedRecords { get; set; }
        public int SuccessfulRecords { get; set; }
        public int FailedRecords { get; set; }
        public double ProgressPercentage => TotalRecords > 0 ? (double)ProcessedRecords / TotalRecords * 100 : 0;
        public DateTime StartTime { get; set; }
        public DateTime? EstimatedCompletion { get; set; }
        public string CurrentOperation { get; set; }
        public List<string> RecentErrors { get; set; } = new List<string>();
        public bool CanCancel { get; set; } = true;
    }
}
