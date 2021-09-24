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
    public interface ISymptomRepository : IRepository<Symptom, int>
    {
    }
    public class SymptomRepository : Repository<Symptom, int>, ISymptomRepository
    {
        public SymptomRepository(TeleMedicineContext dbContext) : base(dbContext)
        {

        }
    }
}
