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
    public interface IDiseaseService : IService<Disease, int>
    {

    }
    public class DiseaseService : IDiseaseService
    {
        private readonly IRepository<Disease, int> _iRepository;

        public DiseaseService(IRepository<Disease, int> iRepository)
        {
            _iRepository = iRepository;
        }

        public async Task<Disease> AddAsync(Disease entity)
        {
            return await _iRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(Disease entity)
        {
            return await _iRepository.Delete(entity);
        }

        public IQueryable<Disease> GetAll(params Expression<Func<Disease, object>>[] includes)
        {
            return _iRepository.GetAll(includes);    
        }

        public async Task<Disease> GetByIdAsync(int id)
        {
            return await _iRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(Disease entity)
        {
            return await _iRepository.UpdateAsync(entity);
        }
    }
}
