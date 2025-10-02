using GoodsEnterprise.Model.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoodsEnterprise.Web.Services
{
    /// <summary>
    /// Interface for product validation operations
    /// </summary>
    public interface IProductValidationService
    {
        /// <summary>
        /// Validate a batch of products
        /// </summary>
        /// <param name="products">List of products to validate</param>
        /// <returns>Validation result</returns>
        Task<ProductBatchValidationResult<ProductImport>> ValidateProductsAsync(List<ProductImport> products);

        /// <summary>
        /// Validate a single product
        /// </summary>
        /// <param name="product">Product to validate</param>
        /// <returns>Validation result</returns>
        Task<ProductValidationResult> ValidateProductAsync(ProductImport product);

        /// <summary>
        /// Generic batch validation result
        /// </summary>
        /// <typeparam name="T">Type of import model</typeparam>
        public class ProductBatchValidationResult<T>
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
        public class ProductValidationResult
        {
            public bool IsValid { get; set; }
            public List<string> Errors { get; set; } = new List<string>();
            public List<string> Warnings { get; set; } = new List<string>();
            public Dictionary<string, List<string>> FieldErrors { get; set; } = new Dictionary<string, List<string>>();
        }

    }
}
