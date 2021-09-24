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
    public interface IDiseaseGroupService : IService<DiseaseGroup, int>
    {

    }
    public class DiseaseGroupService : IDiseaseGroupService
    {
        private readonly IRepository<DiseaseGroup, int> _iRepository;

        public DiseaseGroupService(IRepository<DiseaseGroup, int> iRepository)
        {
            _iRepository = iRepository;
        }
        public async Task<DiseaseGroup> AddAsync(DiseaseGroup entity)
        {
            return await _iRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(DiseaseGroup entity)
        {
            return await _iRepository.Delete(entity);
        }

        public IQueryable<DiseaseGroup> GetAll(params Expression<Func<DiseaseGroup, object>>[] includes)
        {
            return _iRepository.GetAll(includes);
        }

        public async Task<DiseaseGroup> GetByIdAsync(int id)
        {
            return await _iRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(DiseaseGroup entity)
        {
            return await _iRepository.UpdateAsync(entity);
        }
    }
}
