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
        bool IsDuplicatedSymptomCode(string symptomCode);
    }
    public class SymptomRepository : Repository<Symptom, int>, ISymptomRepository
    {
        public SymptomRepository(TeleMedicineContext dbContext) : base(dbContext)
        {

        }

        public bool IsDuplicatedSymptomCode(string symptomCode)
        {
            Symptom currentSymptom = GetAll().Where(x => x.SymptomCode == symptomCode).FirstOrDefault();

            return currentSymptom != null; 
        }
    }
}
