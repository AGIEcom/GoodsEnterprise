using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GoodsEnterprise.Model;
using GoodsEnterprise.Model.Models;
using GoodsEnterprise.DataAccess.Interface;
using System.Linq;

using System.Data;

namespace GoodsEnterprise.DataAccess.Implementation
{

    public class BrandDA:IBrandDA
    {
        private readonly GoodsEnterpriseDbContext _goodsEnterpriseDbContext;
        public BrandDA(GoodsEnterpriseDbContext goodsEnterpriseDbContext)
        {
            _goodsEnterpriseDbContext = goodsEnterpriseDbContext;
        }


        public async Task<Brand> GetBrandAsync(int brandId)
        {
            return await _goodsEnterpriseDbContext.Brands.AsNoTracking().FirstOrDefaultAsync(x => x.Id == brandId && x.IsDelete != true);
        }

        public async Task<List<Brand>> GetBrandListAsync()
        {
            return await _goodsEnterpriseDbContext.Brands.Where(x => x.IsDelete == null || x.IsDelete == false).OrderByDescending(_ => _.Id).ToListAsync();
        }

        public async Task<List<Brand>> GetBrandListAsync(Pagination pagination)
        {
            try
            {
                Dictionary<string, SqlParameter> sqlParameters = new Dictionary<string, SqlParameter>();
                sqlParameters.Add("@PageNumber", new SqlParameter("@PageNumber", pagination.PageNumber == 0 ? 1 : pagination.PageNumber));
                sqlParameters.Add("@PageSize", new SqlParameter("@PageSize", pagination.PageSize == 0 ? 5 : pagination.PageSize));               
                sqlParameters.Add("@OrderByDescending", new SqlParameter("@OrderByDescending", 1));
                sqlParameters.Add("@OrderBy", new SqlParameter("@OrderBy", "DATE"));
                sqlParameters.Add("@SearchByBrandName", new SqlParameter("@SearchByBrandName", pagination.CurrentFilter == null ? string.Empty : pagination.CurrentFilter));
                SqlParameter totalRecordsParam = new SqlParameter
                {
                    ParameterName = "@TotalRecords",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Output,
                };
                sqlParameters.Add("@TotalRecords OUTPUT", totalRecordsParam);
                var result = await _goodsEnterpriseDbContext.Brands.FromSqlRaw($"[dbo].[USP_GetBrands] {string.Join(",", sqlParameters?.Keys)}", sqlParameters?.Values.ToArray()).ToListAsync();
                pagination.TotalRecords = Convert.ToInt32(totalRecordsParam.Value);
                return result;
            }
            catch (Exception ex)
            {
                return null;
                    }
        }

        public async Task<Brand> GetBrandNameAsync(string brandName)
        { 
            return await _goodsEnterpriseDbContext.Brands.AsNoTracking().FirstOrDefaultAsync(x => x.Name == brandName && x.IsDelete != true);
        }

        public async Task SaveAsync(Brand brand)
        {
            brand.CreatedDate = DateTime.Now;
            _goodsEnterpriseDbContext.Brands.Add(brand);
            await _goodsEnterpriseDbContext.SaveChangesAsync();          
        }

        public async Task UpdateAsync(Brand brand)
        {
            brand.ModifiedDate = DateTime.Now;
            _goodsEnterpriseDbContext.Attach(brand).State = EntityState.Modified;
            await _goodsEnterpriseDbContext.SaveChangesAsync();
        }
    }
}
