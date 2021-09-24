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
    public interface IDoctorRepository : IRepository<Doctor, int>
    {
    }
    public class DoctorRepository : Repository<Doctor, int>, IDoctorRepository
    {
        public DoctorRepository(TeleMedicineContext dbContext) : base(dbContext)
        {

        }
    }
}
