using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces.Implements
{
    public class Repository<T, TKey> : IRepository<T, TKey> where T : class
    {
        protected TeleMedicineContext _dbContext;

        public Repository(TeleMedicineContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<T> GetAll(params Expression<Func<T, object>>[] predicate)
        {
            try
            {
                IQueryable<T> queryList = _dbContext.Set<T>().AsNoTracking();
                foreach (Expression<Func<T, object>> expression in predicate)
                {
                    queryList = queryList.Include(expression);
                }
                return queryList;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException($"Couldn't retrieve entities: {nameof(GetAll)} because: {ex.Message}");
            }
        }

        public async Task<T> GetByIdAsync(TKey id)
        {
            try
            {
                return await _dbContext.FindAsync<T>(id);
            } catch(Exception ex)
            {
                throw new ArgumentNullException($"Couldn't retrieve entity: {nameof(GetByIdAsync)} because: {ex.Message}");
            }
        }

        public async Task<T> AddAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(AddAsync)} entity must not be null");
            }
            try
            {
                await _dbContext.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entity)} could not be saved: {ex.Message}");
            }
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(AddAsync)} entity must not be null");
            }

            try
            {
                _dbContext.Update(entity);
                int updatedEntries = await _dbContext.SaveChangesAsync();

                return updatedEntries > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entity)} could not be updated: {ex.Message}");
            }
        }

        public async Task<bool> Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(AddAsync)} entity must not be null");
            }
            try
            {
                _dbContext.Set<T>().Remove(entity);
                int updatedEntries = await _dbContext.SaveChangesAsync();

                return updatedEntries > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entity)} could not be deleted: {ex.Message}");
            }
        }

        public void DeleteRange(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> lstRemove = _dbContext.Set<T>().Where(predicate);
            _dbContext.RemoveRange(lstRemove);
        }

        public int Count(params Expression<Func<T, object>>[] predicate)
        {
            try
            {
                IQueryable<T> queryList = _dbContext.Set<T>().AsNoTracking();
                foreach (var expression in predicate)
                {
                    queryList = queryList.Include(expression);
                }
                return queryList.Count();
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException($"Couldn't retrieve entities: {nameof(GetAll)} because: {ex.Message}");
            }
        }
    }
}
