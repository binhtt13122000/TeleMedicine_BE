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
    public interface IDrugTypeRepository : IRepository<DrugType, int>
    {

    }
    public class DrugTypeRepository : Repository<DrugType, int>, IDrugTypeRepository
    {
        public DrugTypeRepository(TeleMedicineContext dbContext) : base(dbContext)
        {

        }
    }
}
