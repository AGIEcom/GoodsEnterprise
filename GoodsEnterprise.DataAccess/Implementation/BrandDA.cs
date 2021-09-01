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
        private readonly GoodsEnterpriseContext _goodsEnterpriseContext;
        public BrandDA(GoodsEnterpriseContext goodsEnterpriseContext)
        {
            _goodsEnterpriseContext = goodsEnterpriseContext;
        }
    }
}
