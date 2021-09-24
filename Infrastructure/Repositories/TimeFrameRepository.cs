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
    public interface ITimeFrameRepository : IRepository<TimeFrame, int>
    {
    }
    public class TimeFrameRepository : Repository<TimeFrame, int>, ITimeFrameRepository
    {
        public TimeFrameRepository(TeleMedicineContext dbContext) : base(dbContext)
        {

        }
    }
}
