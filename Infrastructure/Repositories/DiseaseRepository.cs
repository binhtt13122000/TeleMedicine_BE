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
    public interface IDiseaseRepository : IRepository<Disease, int>
    {
        bool IsDuplicatedDiseaseCode(String diseaseCode);
    }
    public class DiseaseRepository : Repository<Disease, int>, IDiseaseRepository
    {
        public DiseaseRepository(TeleMedicineContext dbContext) : base(dbContext)
        {

        }

        public bool IsDuplicatedDiseaseCode(string diseaseCode)
        {
            Disease checkDiseaseExisted = GetAll().Where(s => diseaseCode.Trim().ToUpper().Equals(s.DiseaseCode.ToUpper())).FirstOrDefault();
            return checkDiseaseExisted != null;
        }
    }
}
