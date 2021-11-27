using GoodsEnterprise.Model.Models;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GoodsEnterprise.DataAccess.Interface
{
    public interface IDownloadDA
    {
        Task BulkInsertBrandAsync(List<Brand> brands);
        Task BulkInsertCategoryAsync(List<Category> categories);
        Task BulkInsertSubCategoryAsync(List<SubCategory> subCategories);
        Task BulkInsertProductAsync(List<Product> products);
    }
}
