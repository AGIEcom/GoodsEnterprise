using GoodsEnterprise.Model.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoodsEnterprise.Web.Services
{
    /// <summary>
    /// Interface for base cost validation operations
    /// </summary>
    public interface IBaseCostValidationService
    {
        /// <summary>
        /// Validate a batch of base costs
        /// </summary>
        /// <param name="baseCosts">List of base costs to validate</param>
        /// <returns>Validation result</returns>
        Task<BatchValidationResult<BaseCostImport>> ValidateBaseCostsAsync(List<BaseCostImport> baseCosts);

        /// <summary>
        /// Validate a single base cost
        /// </summary>
        /// <param name="baseCost">Base cost to validate</param>
        /// <returns>Validation result</returns>
        Task<BaseCostValidationResult> ValidateBaseCostAsync(BaseCostImport baseCost);
    }

    /// <summary>
    /// Generic batch validation result
    /// </summary>
    /// <typeparam name="T">Type of import model</typeparam>
    public class BatchValidationResult<T>
    {
        public int TotalRecords { get; set; }
        public int ValidRecordCount { get; set; }
        public int InvalidRecordCount { get; set; }
        public List<T> ValidRecords { get; set; } = new List<T>();
        public List<T> InvalidRecords { get; set; } = new List<T>();
        public List<string> GlobalErrors { get; set; } = new List<string>();
        public List<string> GlobalWarnings { get; set; } = new List<string>();
        public List<DuplicateInfo> Duplicates { get; set; } = new List<DuplicateInfo>();
    }

    /// <summary>
    /// Single validation result
    /// </summary>
    public class BaseCostValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public Dictionary<string, List<string>> FieldErrors { get; set; } = new Dictionary<string, List<string>>();
    }
}
