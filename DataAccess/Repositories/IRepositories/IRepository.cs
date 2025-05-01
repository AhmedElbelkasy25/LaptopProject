using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.IRepositories
{
    public interface IRepository<T> where T : class
    {
        Task CreateAsync(T entity);
        void Alter(T entity);
        void Delete(T entity);
        //void Commit();
        IQueryable<T> Get(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? includeProps = null, bool tracked = true);
        T? GetOne(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IIncludableQueryable<T, object>>? includeProps = null, bool tracked = true);
    }

}
