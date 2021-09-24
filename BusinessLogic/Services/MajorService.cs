using Infrastructure.Interfaces;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public interface IMajorService : IService<Major, int>
    {

    }
    public class MajorService : IMajorService
    {
        private readonly IRepository<Major, int> _iRepository;

        public MajorService(IRepository<Major,int> iRepository)
        {
            _iRepository = iRepository;
        }

        public async Task<Major> AddAsync(Major entity)
        {
            return await _iRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(Major entity)
        {
            return await _iRepository.Delete(entity);
        }

        public IQueryable<Major> GetAll(params Expression<Func<Major, object>>[] includes)
        {
            return _iRepository.GetAll(includes);
        }

        public async Task<Major> GetByIdAsync(int id)
        {
            return await _iRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(Major entity)
        {
            return await _iRepository.UpdateAsync(entity);
        }
    }
}
