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
    public interface ITimeFrameRepository : IRepository<TimeFrame, int>
    {
        Task<bool> AddTimeFramesAsync(List<TimeFrame> timeFrames);
    }
    public class TimeFrameRepository : Repository<TimeFrame, int>, ITimeFrameRepository
    {
        public TimeFrameRepository(TeleMedicineContext dbContext) : base(dbContext)
        {

        }

        public async Task<bool> AddTimeFramesAsync(List<TimeFrame> timeFrames)
        {
            if (timeFrames == null)
            {
                throw new ArgumentNullException($"{nameof(AddTimeFramesAsync)} entity must not be null");
            }
            var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                await _dbContext.AddRangeAsync(timeFrames);
                int updatedEntry = await _dbContext.SaveChangesAsync();
                transaction.Commit();
                return updatedEntry > 0;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception($"{nameof(timeFrames)} could not be saved: {ex.Message}");
            }
        }
    }
}
