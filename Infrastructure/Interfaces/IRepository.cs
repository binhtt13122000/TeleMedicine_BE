using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IRepository<T, TKey>
    {
        IQueryable<T> GetAll(params Expression<Func<T, object>>[] predicate);
        Task<T> GetByIdAsync(TKey id);
        Task<T> AddAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> Delete(T entity);
        void DeleteRange(Expression<Func<T, bool>> predicate);
    }
}
