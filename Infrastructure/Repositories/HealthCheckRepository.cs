using Infrastructure.Interfaces;
using Infrastructure.Interfaces.Implements;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IHealthCheckRepository : IRepository<HealthCheck, int>
    {

    }
    public class HealthCheckRepository : Repository<HealthCheck, int>, IHealthCheckRepository
    {
        public HealthCheckRepository(TeleMedicineContext dbContext) : base(dbContext)
        {

        }
    }
}
