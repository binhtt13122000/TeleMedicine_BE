using Infrastructure.Interfaces;
using Infrastructure.Interfaces.Implements;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IHealthCheckRepository : IRepository<HealthCheck, int>
    {
        public DbSet<HealthCheck> access();
    }
    public class HealthCheckRepository : Repository<HealthCheck, int>, IHealthCheckRepository
    {
        public HealthCheckRepository(TeleMedicineContext dbContext) : base(dbContext)
        {

        }

        public DbSet<HealthCheck> access()
        {
            return _dbContext.Set<HealthCheck>();
        }
    }
}
