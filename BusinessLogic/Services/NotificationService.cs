using Infrastructure.Models;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public interface INotificationService : IService<Notification, int>
    {
        Task<bool> SetIsSeen(int userId);
    }
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }
        public async Task<Notification> AddAsync(Notification entity)
        {
            return await _notificationRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(Notification entity)
        {
            return await _notificationRepository.Delete(entity);
        }

        public IQueryable<Notification> GetAll(params Expression<Func<Notification, object>>[] includes)
        {
            return _notificationRepository.GetAll(includes);
        }

        public async Task<Notification> GetByIdAsync(int id)
        {
            return await _notificationRepository.GetByIdAsync(id);
        }

        public Task<bool> SetIsSeen(int userId)
        {
            return _notificationRepository.SetIsSeen(userId);
        }

        public async Task<bool> UpdateAsync(Notification entity)
        {
            return await _notificationRepository.UpdateAsync(entity);
        }
    }
}
