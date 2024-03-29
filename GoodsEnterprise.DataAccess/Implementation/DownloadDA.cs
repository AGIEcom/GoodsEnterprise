﻿using EFCore.BulkExtensions;
using GoodsEnterprise.DataAccess.Interface;
using GoodsEnterprise.Model.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoodsEnterprise.DataAccess.Implementation
{
    /// <summary>
    /// DownloadDA
    /// </summary>
    public class DownloadDA : IDownloadDA
    {
        private readonly GoodsEnterpriseContext _goodsEnterpriseContext;
        public DownloadDA(GoodsEnterpriseContext goodsEnterpriseContext)
        {
            _goodsEnterpriseContext = goodsEnterpriseContext;
        }

        /// <summary>
        /// BulkInsertBrandAsync
        /// </summary>
        /// <param name="brands"></param>
        /// <returns></returns>
        public async Task BulkInsertBrandAsync(List<Brand> brands)
        {
            var bulkConfig = new BulkConfig { SetOutputIdentity = true, BatchSize = 1000 };
            await _goodsEnterpriseContext.BulkInsertAsync(brands, bulkConfig);
        }

        /// <summary>
        /// BulkInsertCategoryAsync
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public async Task BulkInsertCategoryAsync(List<Category> categories)
        {
            var bulkConfig = new BulkConfig { SetOutputIdentity = true, BatchSize = 1000 };
            await _goodsEnterpriseContext.BulkInsertAsync(categories, bulkConfig);
        }

        /// <summary>
        /// BulkInsertSubCategoryAsync
        /// </summary>
        /// <param name="subCategories"></param>
        /// <returns></returns>
        public async Task BulkInsertSubCategoryAsync(List<SubCategory> subCategories)
        {
            var bulkConfig = new BulkConfig { SetOutputIdentity = true, BatchSize = 1000 };
            await _goodsEnterpriseContext.BulkInsertAsync(subCategories, bulkConfig);
        }

        /// <summary>
        /// BulkInsertProductAsync
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        public async Task BulkInsertProductAsync(List<Product> products)
        {
            var bulkConfig = new BulkConfig { SetOutputIdentity = true, BatchSize = 1000 };
            await _goodsEnterpriseContext.BulkInsertAsync(products, bulkConfig);
        }
    }
}
