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
    public interface INotificationRepository : IRepository<Notification, int>
    {
        Task<bool> SetIsSeen(int userId);

        void AddManyAsync(List<Notification> notifications);
    }
    public class NotificationRepository : Repository<Notification, int>, INotificationRepository
    {
        public NotificationRepository(TeleMedicineContext dbContext) : base(dbContext)
        {

        }

        public async void AddManyAsync(List<Notification> notifications)
        {
            await _dbContext.Set<Notification>().AddRangeAsync(notifications);
        }

        public async Task<bool> SetIsSeen(int userId)
        {
            IQueryable<Notification> notifications = _dbContext.Set<Notification>().Where(s => s.UserId == userId);

            foreach(Notification notification in notifications)
            {
                notification.IsSeen = true;
            }

            _dbContext.UpdateRange(notifications);

            int result = await _dbContext.SaveChangesAsync();

            return result > 0;
        }
    }
}
