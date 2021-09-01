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

    public class BrandDA : IBrandDA
    {
        private readonly GoodsEnterpriseContext _goodsEnterpriseContext;
        public BrandDA(GoodsEnterpriseContext goodsEnterpriseContext)
        {
            _goodsEnterpriseContext = goodsEnterpriseContext;
        }

        public async Task InsertAsync()
        {
            //var bulkConfig = new BulkConfig()
            //{
            //    SetOutputIdentity = true,
            //    PreserveInsertOrder = true
            //};
            //var subEntities = new List<ItemHistory>();
            //entities = entities.ForEach(i => i.ID == 0);
            //await _goodsEnterpriseContext.BulkSaveChanges();


            //foreach (var entity in entities)
            //{
            //    foreach (var subEntity in entity.ItemHistories)
            //    {
            //        subEntity.ItemId = entity.ID; // setting FK to match its linked PK that was generated in DB
            //    }
            //    subEntities.AddRange(entity.ItemHistories);
            //}
            //await _airportDBContext.BulkInsertOrUpdateAsync(subEntities});
        }
    }
}
