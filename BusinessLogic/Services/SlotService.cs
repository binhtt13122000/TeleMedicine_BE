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
    public interface ISlotService : IService<Slot, int>
    {
        Task<bool> AddSlotsAsync(List<Slot> slots);
        void DeleteRange(List<int> slotIds);
    }
    public class SlotService : ISlotService
    {
        private readonly ISlotRepository _slotRepository;

        public SlotService(ISlotRepository roleRepository)
        {
            _slotRepository = roleRepository;
        }
        public async Task<Slot> AddAsync(Slot entity)
        {
            return await _slotRepository.AddAsync(entity);
        }

        public async Task<bool> AddSlotsAsync(List<Slot> slots)
        {
            return await _slotRepository.AddSlotsAsync(slots);
        }

        public async Task<bool> DeleteAsync(Slot entity)
        {
            return await _slotRepository.Delete(entity);
        }

        public void DeleteRange(List<int> slotIds)
        {
            _slotRepository.DeleteRange(s => slotIds.Contains(s.Id));
        }

        public IQueryable<Slot> GetAll(params Expression<Func<Slot, object>>[] includes)
        {
            return _slotRepository.GetAll(includes);
        }

        public async Task<Slot> GetByIdAsync(int id)
        {
            return await _slotRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(Slot entity)
        {
            return await _slotRepository.UpdateAsync(entity);
        }
    }
}
