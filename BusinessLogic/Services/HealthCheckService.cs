using Infrastructure.Models;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public interface IHealthCheckService : IService<HealthCheck, int>
    {
        DbSet<HealthCheck> access();

        HealthCheck GetNearestHealthCheckByCondition(List<Slot> slots, DateTime currentDate, TimeSpan currentTime);
    }
    public class HealthCheckService : IHealthCheckService
    {
        private readonly IHealthCheckRepository _healthCheckRepository;

        public HealthCheckService(IHealthCheckRepository healthCheckRepository)
        {
            _healthCheckRepository = healthCheckRepository;
        }

        public DbSet<HealthCheck> access()
        {
            return _healthCheckRepository.access();
        }

        public async Task<HealthCheck> AddAsync(HealthCheck entity)
        {
            return await _healthCheckRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(HealthCheck entity)
        {
            return await _healthCheckRepository.Delete(entity);
        }

        public IQueryable<HealthCheck> GetAll(params Expression<Func<HealthCheck, object>>[] includes)
        {
            return _healthCheckRepository.GetAll(includes);
        }

        public async Task<HealthCheck> GetByIdAsync(int id)
        {
            return await _healthCheckRepository.GetByIdAsync(id);
        }

        public HealthCheck GetNearestHealthCheckByCondition(List<Slot> slots, DateTime currentDate, TimeSpan currentTime)
        {
            HealthCheck healthCheckFind = null;
            IEnumerable<Slot> convertSlots = slots.AsEnumerable();
            Slot slot = convertSlots.Where(s => s.AssignedDate.CompareTo(currentDate) == 0).Where(s => s.StartTime.CompareTo(currentTime) >= 0).OrderBy(s => Math.Abs(s.StartTime.Ticks - currentTime.Ticks)).FirstOrDefault();
            if (slot != null)
            {
                healthCheckFind = access().Include(s => s.Slots).ThenInclude(s => s.Doctor)
                                                                    .Include(s => s.Patient)
                                                                    .Include(s => s.Prescriptions)
                                                                    .Include(s => s.HealthCheckDiseases).ThenInclude(s => s.Disease)
                                                                    .Include(s => s.SymptomHealthChecks).ThenInclude(s => s.Symptom)
                                                                    .Where(s => s.Id == slot.HealthCheckId).FirstOrDefault();
                return healthCheckFind;
            }
            bool checkNextDay = true;
            DateTime nextDate = currentDate;
            while (checkNextDay)
            {
                nextDate = nextDate.AddDays(1);
                TimeSpan timeSpan = new TimeSpan(0, 0, 0);
                convertSlots = convertSlots.Where(s => s.AssignedDate.CompareTo(nextDate.Date) >= 0);
                if (slots != null && slots.Count<Slot>() > 0)
                {
                    Slot checkSlot = slots.Where(s => s.AssignedDate.CompareTo(nextDate) == 0).OrderBy(s => Math.Abs(s.StartTime.Ticks - timeSpan.Ticks)).FirstOrDefault();
                    if (checkSlot != null)
                    {
                        healthCheckFind = access().Include(s => s.Slots).ThenInclude(s => s.Doctor)
                                                                    .Include(s => s.Patient)
                                                                    .Include(s => s.Prescriptions)
                                                                    .Include(s => s.HealthCheckDiseases).ThenInclude(s => s.Disease)
                                                                    .Include(s => s.SymptomHealthChecks).ThenInclude(s => s.Symptom)
                                                                    .Where(s => s.Id == checkSlot.HealthCheckId).FirstOrDefault();
                        checkNextDay = false;
                    }
                }
                else
                {
                    checkNextDay = false;
                }
            }
            return healthCheckFind;
        }

        public async Task<bool> UpdateAsync(HealthCheck entity)
        {
            return await _healthCheckRepository.UpdateAsync(entity);
        }
    }
}
