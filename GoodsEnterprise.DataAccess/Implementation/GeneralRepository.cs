using GoodsEnterprise.DataAccess.Interface;
using GoodsEnterprise.Model;
using GoodsEnterprise.Model.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GoodsEnterprise.DataAccess.Implementation
{
    /// <summary>
    /// GeneralRepository - Generic class to datamodel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GeneralRepository<T> : IGeneralRepository<T> where T : class
    {
        private readonly GoodsEnterpriseContext context;
        private DbSet<T> entities;
        string errorMessage = string.Empty;

        /// <summary>
        /// GeneralRepository
        /// </summary>
        /// <param name="context"></param>
        public GeneralRepository(GoodsEnterpriseContext context)
        {
            this.context = context;
            entities = context.Set<T>();
        }
        /// <summary>
        /// GetAllAsync
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            IQueryable<T> query = entities.AsNoTracking();

            if (include != null)
            {
                query = include(query);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// GetAsync
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public async Task<T> GetAsync(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            IQueryable<T> query = entities.AsNoTracking();
            if (include != null)
            {
                query = include(query);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.FirstOrDefaultAsync();
        }

        /// <summary>
        /// GetAllWithPaginationAsync
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public async Task<List<T>> GetAllWithPaginationAsync(DBPaginationParams pagination)
        {
            try
            {
                Dictionary<string, SqlParameter> sqlParameters = new Dictionary<string, SqlParameter>();
                sqlParameters.Add("@OffsetValue", new SqlParameter("@OffsetValue", pagination.OffsetValue == 0 ? 1 : pagination.OffsetValue));
                sqlParameters.Add("@PagingSize", new SqlParameter("@PagingSize", pagination.PagingSize == 0 ? 10 : pagination.PagingSize));
                sqlParameters.Add("@sortOrder", new SqlParameter("@sortOrder", pagination.sortOrder));
                sqlParameters.Add("@sortColumn", new SqlParameter("@sortColumn", pagination.sortColumn));
                sqlParameters.Add("@SearchText", new SqlParameter("@SearchText", pagination.SearchText == null ? string.Empty : pagination.SearchText));
                //SqlParameter totalRecordsParam = new SqlParameter
                //{
                //    ParameterName = "@TotalRecords",
                //    SqlDbType = SqlDbType.Int,
                //    Direction = ParameterDirection.Output,
                //};
                //sqlParameters.Add("@TotalRecords OUTPUT", totalRecordsParam);

                var result = await entities.FromSqlRaw($"{pagination.StoredProcuder} {string.Join(",", sqlParameters?.Keys)}", sqlParameters?.Values.ToArray()).ToListAsync();
                //pagination.TotalRecords = Convert.ToInt32(totalRecordsParam.Value);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }


        }

        /// <summary>
        /// InsertAsync
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task InsertAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(entity.GetType().FullName);
            }

            entities.Add(entity);
            var dbEntityEntry = context.Entry(entity);
            dbEntityEntry.Property("CreatedDate").CurrentValue = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }
        /// <summary>
        /// UpdateAsync
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(entity.GetType().FullName);
            }
            var dbEntityEntry = context.Entry(entity);
            dbEntityEntry.Property("ModifiedDate").CurrentValue = DateTime.UtcNow;
            context.Attach(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
        /// <summary>
        /// PhysicalDeleteAsync
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> PhysicalDeleteAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(entity.GetType().FullName);
            }
            entities.Remove(entity);
            await context.SaveChangesAsync();
            return true;
        }
        /// <summary>
        /// LogicalDeleteAsync
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> LogicalDeleteAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(entity.GetType().FullName);
            }

            var dbEntityEntry = context.Entry(entity);
            dbEntityEntry.Property("ModifiedDate").CurrentValue = DateTime.Now;
            dbEntityEntry.Property("IsDelete").CurrentValue = true;
            context.Attach(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return true;
        }
        

        public async Task<bool> PostValueUsingUDTT(DataTable datatable, CommenParameters commenParameters)
        {
            try
            {
                Dictionary<string, SqlParameter> sqlParameters = new Dictionary<string, SqlParameter>();
                sqlParameters.Add("@UDTType_PromotionCost", new SqlParameter("@UDTType_PromotionCost", SqlDbType.Structured)
                {
                    //TypeName = commenParameters.SPName,
                    TypeName = "dbo.UDTType_PromotionCost",
                    Value = datatable
                });

                sqlParameters.Add("@CREATEDBY", new SqlParameter("@CREATEDBY", commenParameters.CreatedBy));
                ////var result = await entities.FromSqlRaw($"{commenParameters.SPName} {string.Join(",", sqlParameters?.Keys)}", sqlParameters?.Values.ToArray()).ToListAsync();
                //var result = await entities.FromSqlRaw("EXEC dbo.usp_INSERTPROMOTIONCOST @UDTType_PromotionCost, @CREATEDBY", sqlParameters?.Values.ToArray()).ToListAsync();
                ////pagination.TotalRecords = Convert.ToInt32(totalRecordsParam.Value);

                //return Convert.ToBoolean(result[0]);

                int rowsAffected = await context.Database.ExecuteSqlRawAsync(
                "EXEC dbo.usp_INSERTPROMOTIONCOST @UDTType_PromotionCost, @CREATEDBY",
                sqlParameters?.Values.ToArray());

                // rowsAffected = number of rows affected by the insert
                return rowsAffected > 0;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
