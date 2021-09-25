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
    public interface ISymptomService : IService<Symptom, int>
    {
        bool IsDuplicated(string symptomCode);
    }
    public class SymptomService : ISymptomService
    {
        private readonly ISymptomRepository _symptomRepository;

        public SymptomService(ISymptomRepository symptomRepository)
        {
            _symptomRepository = symptomRepository;
        }
        public async Task<Symptom> AddAsync(Symptom entity)
        {
            return await _symptomRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(Symptom entity)
        {
            return await _symptomRepository.Delete(entity);
        }

        public IQueryable<Symptom> GetAll(params Expression<Func<Symptom, object>>[] includes)
        {
            return _symptomRepository.GetAll(includes);
        }

        public async Task<Symptom> GetByIdAsync(int id)
        {
            return await _symptomRepository.GetByIdAsync(id);
        }

        public bool IsDuplicated(string symptomCode)
        {
            return _symptomRepository.IsDuplicatedSymptomCode(symptomCode);
        }

        public async Task<bool> UpdateAsync(Symptom entity)
        {
            return await _symptomRepository.UpdateAsync(entity);
        }
    }
}
