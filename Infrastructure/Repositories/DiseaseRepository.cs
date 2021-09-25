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

    }
    public class DiseaseRepository : Repository<Disease, int>, IDiseaseRepository
    {
        public DiseaseRepository(TeleMedicineContext dbContext) : base(dbContext)
        {

        }
    }
}
