using Infrastructure.Interfaces;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public interface IHealthCheckService : IService<HealthCheck, int>
    {

    }
    public class HealthCheckService : IHealthCheckService
    {
        private readonly IRepository<HealthCheck, int> _iRepository;

        public HealthCheckService(IRepository<HealthCheck, int> iRepository)
        {
            _iRepository = iRepository;
        }
        public async Task<HealthCheck> AddAsync(HealthCheck entity)
        {
            return await _iRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(HealthCheck entity)
        {
            return await _iRepository.Delete(entity);
        }

        public IQueryable<HealthCheck> GetAll(params Expression<Func<HealthCheck, object>>[] includes)
        {
            return _iRepository.GetAll(includes);
        }

        public async Task<HealthCheck> GetByIdAsync(int id)
        {
            return await _iRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(HealthCheck entity)
        {
            return await _iRepository.UpdateAsync(entity);
        }
    }
}
