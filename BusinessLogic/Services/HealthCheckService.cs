using Infrastructure.Models;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public interface IHealthCheckService : IService<HealthCheck, int>
    {
        DbSet<HealthCheck> access();
    }
    public class HealthCheckService : IHealthCheckService
    {
        private readonly IHealthCheckRepository _healthCheckRepository;

        public HealthCheckService(IHealthCheckRepository healthCheckRepository)
        {
            _healthCheckRepository = healthCheckRepository;
        }

        public DbSet<HealthCheck> access()
        {
            return _healthCheckRepository.access();
        }

        public async Task<HealthCheck> AddAsync(HealthCheck entity)
        {
            return await _healthCheckRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(HealthCheck entity)
        {
            return await _healthCheckRepository.Delete(entity);
        }

        public IQueryable<HealthCheck> GetAll(params Expression<Func<HealthCheck, object>>[] includes)
        {
            return _healthCheckRepository.GetAll(includes);
        }

        public async Task<HealthCheck> GetByIdAsync(int id)
        {
            return await _healthCheckRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(HealthCheck entity)
        {
            return await _healthCheckRepository.UpdateAsync(entity);
        }
    }
}
