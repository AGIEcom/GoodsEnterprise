using GoodsEnterprise.Model.CustomerModel;
using GoodsEnterprise.Model.Models;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GoodsEnterprise.DataAccess.Interface
{
    public interface IAdoDA
    {
        Task<List<HomePageBrand>> GetHomePageBrandAsync();

        Task<List<HomePageCategory>> GetHomePageCategoryAsync();
    }
}
