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
    public interface ISlotRepository : IRepository<Slot, int>
    {
        Task<bool> AddSlotsAsync(List<Slot> slots);

        public DbSet<Slot> Access();
    }
    public class SlotRepository : Repository<Slot, int>, ISlotRepository
    {
        public SlotRepository(TeleMedicineContext dbContext) : base(dbContext)
        {

        }

        public DbSet<Slot> Access()
        {
            return _dbContext.Set<Slot>();
        }

        public async Task<bool> AddSlotsAsync(List<Slot> slots)
        {
            if (slots == null)
            {
                throw new ArgumentNullException($"{nameof(AddSlotsAsync)} entity must not be null");
            }
            var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                await _dbContext.AddRangeAsync(slots);
                int updatedEntry =  await _dbContext.SaveChangesAsync();
                transaction.Commit();
                return updatedEntry > 0;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception($"{nameof(slots)} could not be saved: {ex.Message}");
            }
        }
    }
}
