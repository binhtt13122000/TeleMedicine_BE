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
    public interface IDrugRepository : IRepository<Drug, int>
    {

    }
    public class DrugRepository : Repository<Drug, int>, IDrugRepository
    {
        public DrugRepository(TeleMedicineContext dbContext) : base(dbContext)
        {

        }
    }
}
