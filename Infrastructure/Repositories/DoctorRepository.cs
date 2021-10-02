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
    public interface IDoctorRepository : IRepository<Doctor, int>
    {
        public DbSet<Doctor> access();
    }
    public class DoctorRepository : Repository<Doctor, int>, IDoctorRepository
    {
        public DoctorRepository(TeleMedicineContext dbContext) : base(dbContext)
        {

        }

        public DbSet<Doctor> access()
        {
            return _dbContext.Set<Doctor>();
        }
    }
}
