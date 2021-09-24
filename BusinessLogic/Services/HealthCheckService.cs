using Infrastructure.Models;
using Infrastructure.Repositories;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public interface IHealthCheckService : IService<HealthCheck, int>
    {

    }
    public class HealthCheckService : IHealthCheckService
    {
        private readonly HealthCheckRepository _iHealthCheckRepository;

        public HealthCheckService(HealthCheckRepository iHealthCheckRepository)
        {
            _iHealthCheckRepository = iHealthCheckRepository;
        }
        public async Task<HealthCheck> AddAsync(HealthCheck entity)
        {
            return await _iHealthCheckRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(HealthCheck entity)
        {
            return await _iHealthCheckRepository.Delete(entity);
        }

        public IQueryable<HealthCheck> GetAll(params Expression<Func<HealthCheck, object>>[] includes)
        {
            return _iHealthCheckRepository.GetAll(includes);
        }

        public async Task<HealthCheck> GetByIdAsync(int id)
        {
            return await _iHealthCheckRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(HealthCheck entity)
        {
            return await _iHealthCheckRepository.UpdateAsync(entity);
        }
    }
}
