using GoodsEnterprise.Model.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoodsEnterprise.Web.Services
{
    /// <summary>
    /// Interface for promotion cost validation operations
    /// </summary>
    public interface IPromotionCostValidationService
    {
        /// <summary>
        /// Validate a batch of promotion costs
        /// </summary>
        /// <param name="promotionCosts">List of promotion costs to validate</param>
        /// <returns>Validation result</returns>
        Task<PromotionCostBatchValidationResult<PromotionCostImport>> ValidatePromotionCostsAsync(List<PromotionCostImport> promotionCosts);

        /// <summary>
        /// Validate a single promotion cost
        /// </summary>
        /// <param name="promotionCost">Promotion cost to validate</param>
        /// <returns>Validation result</returns>
        Task<PromotionCostValidationResult> ValidatePromotionCostAsync(PromotionCostImport promotionCost);
    }

    /// <summary>
    /// Generic batch validation result
    /// </summary>
    /// <typeparam name="T">Type of import model</typeparam>
    public class PromotionCostBatchValidationResult<T>
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
    public class PromotionCostValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public Dictionary<string, List<string>> FieldErrors { get; set; } = new Dictionary<string, List<string>>();
    }
}
