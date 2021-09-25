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
        private readonly IDiseaseGroupRepository _diseaseGroupRepository;

        public DiseaseGroupService(IDiseaseGroupRepository diseaseGroupRepository)
        {
            _diseaseGroupRepository = diseaseGroupRepository;
        }
        public async Task<DiseaseGroup> AddAsync(DiseaseGroup entity)
        {
            return await _diseaseGroupRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(DiseaseGroup entity)
        {
            return await _diseaseGroupRepository.Delete(entity);
        }

        public IQueryable<DiseaseGroup> GetAll(params Expression<Func<DiseaseGroup, object>>[] includes)
        {
            return _diseaseGroupRepository.GetAll(includes);
        }

        public async Task<DiseaseGroup> GetByIdAsync(int id)
        {
            return await _diseaseGroupRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(DiseaseGroup entity)
        {
            return await _diseaseGroupRepository.UpdateAsync(entity);
        }
    }
}
