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
        private readonly MajorRepository _iMajorRepository;

        public MajorService(MajorRepository iMajorRepository)
        {
            _iMajorRepository = iMajorRepository;
        }

        public async Task<Major> AddAsync(Major entity)
        {
            return await _iMajorRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(Major entity)
        {
            return await _iMajorRepository.Delete(entity);
        }

        public IQueryable<Major> GetAll(params Expression<Func<Major, object>>[] includes)
        {
            return _iMajorRepository.GetAll(includes);
        }

        public async Task<Major> GetByIdAsync(int id)
        {
            return await _iMajorRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(Major entity)
        {
            return await _iMajorRepository.UpdateAsync(entity);
        }
    }
}
