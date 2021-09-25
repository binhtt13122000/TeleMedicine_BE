using Infrastructure.Repositories;
using Infrastructure.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public interface IDiseaseService : IService<Disease, int>
    {

    }
    public class DiseaseService : IDiseaseService
    {
        private readonly IDiseaseRepository _diseaseRepository;

        public DiseaseService(IDiseaseRepository diseaseRepository)
        {
            _diseaseRepository = diseaseRepository;
        }

        public async Task<Disease> AddAsync(Disease entity)
        {
            return await _diseaseRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(Disease entity)
        {
            return await _diseaseRepository.Delete(entity);
        }

        public IQueryable<Disease> GetAll(params Expression<Func<Disease, object>>[] includes)
        {
            return _diseaseRepository.GetAll(includes);    
        }

        public async Task<Disease> GetByIdAsync(int id)
        {
            return await _diseaseRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(Disease entity)
        {
            return await _diseaseRepository.UpdateAsync(entity);
        }
    }
}
