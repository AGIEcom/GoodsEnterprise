using GoodsEnterprise.DataAccess.Interface;
using GoodsEnterprise.Model.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoodsEnterprise.Web.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataBasePaginationController : ControllerBase
    {
        private readonly IGeneralRepository<ProductList> _product;
        private readonly IGeneralRepository<Category> _category;
        private readonly IGeneralRepository<SubCategory> _subCategory;
        private readonly IGeneralRepository<Brand> _brand;
       // public List<Product> lstproduct = new List<Product>();
        public DataBasePaginationController(IGeneralRepository<ProductList> product, IGeneralRepository<Category> category,
          IGeneralRepository<SubCategory> subCategory, IGeneralRepository<Brand> brand)
        {
            _product = product;
            _category = category;
            _subCategory = subCategory;
            _brand = brand;
        }
       

    }
}
