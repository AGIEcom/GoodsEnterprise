using GoodsEnterprise.Model.Models;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GoodsEnterprise.DataAccess.Interface
{
    public interface IBrandDA
    {
        Task<Brand> GetBrandAsync(int BrandId);

        Task<List<Brand>> GetBrandListAsync();

        Task<Brand> GetBrandNameAsync(string BrandName);
        Task<List<Brand>> GetBrandListAsync(Pagination pagination);

        Task SaveAsync(Brand objBrand);

        Task UpdateAsync(Brand objBrand);
    }
}
