using GoodsEnterprise.Model.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoodsEnterprise.Web.Services
{
    /// <summary>
    /// Interface for supplier data validation
    /// </summary>
    public interface ISupplierValidationService
    {
        /// <summary>
        /// Validate a single supplier import record
        /// </summary>
        /// <param name="supplier">Supplier to validate</param>
        /// <returns>Validation result</returns>
        Task<ValidationResult> ValidateSupplierAsync(SupplierImport supplier);

        /// <summary>
        /// Validate a batch of supplier import records
        /// </summary>
        /// <param name="suppliers">List of suppliers to validate</param>
        /// <returns>Batch validation result</returns>
        Task<BatchValidationResult> ValidateSuppliersAsync(List<SupplierImport> suppliers);

        /// <summary>
        /// Check for duplicate suppliers in the database
        /// </summary>
        /// <param name="suppliers">Suppliers to check</param>
        /// <returns>List of suppliers with duplicate information</returns>
        Task<List<DuplicateInfo>> CheckForDuplicatesAsync(List<SupplierImport> suppliers);

        /// <summary>
        /// Validate business rules for supplier data
        /// </summary>
        /// <param name="supplier">Supplier to validate</param>
        /// <returns>Business rule validation result</returns>
        Task<BusinessRuleValidationResult> ValidateBusinessRulesAsync(SupplierImport supplier);
    }

    /// <summary>
    /// Validation result for a single supplier
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public Dictionary<string, List<string>> FieldErrors { get; set; } = new Dictionary<string, List<string>>();
    }

    /// <summary>
    /// Batch validation result
    /// </summary>
    public class BatchValidationResult
    {
        public int TotalRecords { get; set; }
        public int ValidRecords { get; set; }
        public int InvalidRecords { get; set; }
        public List<SupplierImport> ValidSuppliers { get; set; } = new List<SupplierImport>();
        public List<SupplierImport> InvalidSuppliers { get; set; } = new List<SupplierImport>();
        public List<string> GlobalErrors { get; set; } = new List<string>();
        public List<string> GlobalWarnings { get; set; } = new List<string>();
        public List<DuplicateInfo> Duplicates { get; set; } = new List<DuplicateInfo>();
    }

    /// <summary>
    /// Duplicate information
    /// </summary>
    public class DuplicateInfo
    {
        public int RowNumber { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }
        public string DuplicateType { get; set; } // "Database", "Batch", "Both"
        public List<int> ConflictingRows { get; set; } = new List<int>();
        public int? ExistingId { get; set; }
    }

    /// <summary>
    /// Business rule validation result
    /// </summary>
    public class BusinessRuleValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> RuleViolations { get; set; } = new List<string>();
        public List<string> Recommendations { get; set; } = new List<string>();
    }
}
