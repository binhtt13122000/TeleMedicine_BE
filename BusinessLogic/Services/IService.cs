using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public interface IService<T, TKey>
    {
        IQueryable<T> GetAll(params Expression<Func<T, object>>[] includes);
        Task<T> AddAsync(T entity);
        Task<T> GetByIdAsync(TKey id);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(T entity);
    }
}
