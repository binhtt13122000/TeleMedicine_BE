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
    }
    public class NotificationRepository : Repository<Notification, int>, INotificationRepository
    {
        public NotificationRepository(TeleMedicineContext dbContext) : base(dbContext)
        {

        }
    }
}
