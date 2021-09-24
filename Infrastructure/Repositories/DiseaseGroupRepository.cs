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
    public interface IDiseaseGroupRepository : IRepository<DiseaseGroup, int>
    {

    }
    public class DiseaseGroupRepository : Repository<DiseaseGroup, int>, IDiseaseGroupRepository
    {
        public DiseaseGroupRepository(TeleMedicineContext dbContext) : base(dbContext)
        {

        }
    }
}
