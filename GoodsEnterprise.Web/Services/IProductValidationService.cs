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
        Task<BatchValidationResult<ProductImport>> ValidateProductsAsync(List<ProductImport> products);

        /// <summary>
        /// Validate a single product
        /// </summary>
        /// <param name="product">Product to validate</param>
        /// <returns>Validation result</returns>
        Task<ValidationResult> ValidateProductAsync(ProductImport product);
    }
}
