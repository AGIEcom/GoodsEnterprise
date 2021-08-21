using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GoodsEnterprise.DataAccess.Interface
{
    public interface IGeneralRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);
        Task<T> GetAsync(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);
        Task<List<T>> GetAllWithPaginationAsync(Pagination pagination);
        Task InsertAsync(T entity);
        Task UpdateAsync(T entity);
        Task<bool> PhysicalDeleteAsync(T entity);
        Task<bool> LogicalDeleteAsync(T entity);
    }
}
