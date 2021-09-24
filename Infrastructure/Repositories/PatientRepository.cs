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
    public interface IPatientRepository : IRepository<Patient, int>
    {
    }
    public class PatientRepository : Repository<Patient, int>, IPatientRepository
    {
        public PatientRepository(TeleMedicineContext dbContext) : base(dbContext)
        {

        }
    }
}
