using Infrastructure.Models;
using Infrastructure.Repositories;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public interface IDiseaseGroupService : IService<DiseaseGroup, int>
    {

    }
    public class DiseaseGroupService : IDiseaseGroupService
    {
        private readonly DiseaseGroupRepository _iDiseaseGroupRepository;

        public DiseaseGroupService(DiseaseGroupRepository iDiseaseGroupRepository)
        {
            _iDiseaseGroupRepository = iDiseaseGroupRepository;
        }
        public async Task<DiseaseGroup> AddAsync(DiseaseGroup entity)
        {
            return await _iDiseaseGroupRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(DiseaseGroup entity)
        {
            return await _iDiseaseGroupRepository.Delete(entity);
        }

        public IQueryable<DiseaseGroup> GetAll(params Expression<Func<DiseaseGroup, object>>[] includes)
        {
            return _iDiseaseGroupRepository.GetAll(includes);
        }

        public async Task<DiseaseGroup> GetByIdAsync(int id)
        {
            return await _iDiseaseGroupRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(DiseaseGroup entity)
        {
            return await _iDiseaseGroupRepository.UpdateAsync(entity);
        }
    }
}
