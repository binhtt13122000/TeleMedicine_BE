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
        private readonly DiseaseRepository _iDiseaseRepository;

        public DiseaseService(DiseaseRepository iDiseaseRepository)
        {
            _iDiseaseRepository = iDiseaseRepository;
        }

        public async Task<Disease> AddAsync(Disease entity)
        {
            return await _iDiseaseRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(Disease entity)
        {
            return await _iDiseaseRepository.Delete(entity);
        }

        public IQueryable<Disease> GetAll(params Expression<Func<Disease, object>>[] includes)
        {
            return _iDiseaseRepository.GetAll(includes);    
        }

        public async Task<Disease> GetByIdAsync(int id)
        {
            return await _iDiseaseRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(Disease entity)
        {
            return await _iDiseaseRepository.UpdateAsync(entity);
        }
    }
}
