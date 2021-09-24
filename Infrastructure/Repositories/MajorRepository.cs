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
    public interface IMajorRepository : IRepository<Major, int>
    {

    }
    public class MajorRepository : Repository<Major, int>, IMajorRepository
    {
        public MajorRepository(TeleMedicineContext dbContext) : base(dbContext)
        {

        }
    }

}
