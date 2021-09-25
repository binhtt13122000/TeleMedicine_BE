using Infrastructure.Models;
using Infrastructure.Repositories;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public interface IMajorService : IService<Major, int>
    {

    }
    public class MajorService : IMajorService
    {
        private readonly IMajorRepository _majorRepository;

        public MajorService(IMajorRepository majorRepository)
        {
            _majorRepository = majorRepository;
        }

        public async Task<Major> AddAsync(Major entity)
        {
            return await _majorRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(Major entity)
        {
            return await _majorRepository.Delete(entity);
        }

        public IQueryable<Major> GetAll(params Expression<Func<Major, object>>[] includes)
        {
            return _majorRepository.GetAll(includes);
        }

        public async Task<Major> GetByIdAsync(int id)
        {
            return await _majorRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(Major entity)
        {
            return await _majorRepository.UpdateAsync(entity);
        }
    }
}
