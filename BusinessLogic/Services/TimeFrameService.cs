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
    public interface ITimeFrameService : IService<TimeFrame, int>
    {
        Task<bool> ChangeTimeFrame(int minutes, int rest);
        void DeleteListTimeFrame(List<int> timeFrameIds);
    }
    public class TimeFrameService : ITimeFrameService
    {
        private readonly ITimeFrameRepository _timeFrameRepository;

        public TimeFrameService(ITimeFrameRepository timeFrameRepository)
        {
            _timeFrameRepository = timeFrameRepository;
        }

        public async Task<TimeFrame> AddAsync(TimeFrame entity)
        {
            return await _timeFrameRepository.AddAsync(entity);
        }

        public Task<bool> ChangeTimeFrame(int minutes, int rest)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(TimeFrame entity)
        {
            return await _timeFrameRepository.Delete(entity);
        }

        public void DeleteListTimeFrame(List<int> timeFrameIds)
        {
            _timeFrameRepository.DeleteRange(s => timeFrameIds.Contains(s.Id));
        }

        public IQueryable<TimeFrame> GetAll(params Expression<Func<TimeFrame, object>>[] includes)
        {
            return _timeFrameRepository.GetAll(includes);
        }

        public async Task<TimeFrame> GetByIdAsync(int id)
        {
            return await _timeFrameRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(TimeFrame entity)
        {
            return await _timeFrameRepository.UpdateAsync(entity);
        }
    }
}
