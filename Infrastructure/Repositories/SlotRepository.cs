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
    public interface ISlotRepository : IRepository<Slot, int>
    {
    }
    public class SlotRepository : Repository<Slot, int>, ISlotRepository
    {
        public SlotRepository(TeleMedicineContext dbContext) : base(dbContext)
        {

        }
    }
}
