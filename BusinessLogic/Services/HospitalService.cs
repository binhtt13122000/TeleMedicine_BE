using Infrastructure.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Infrastructure.Repositories;

namespace BusinessLogic.Services
{
    public interface IHospitalService : IService<Hospital, int>
    {
        bool IsDuplicated(string hospitalCode);
    }
    public class HospitalService : IHospitalService
    {
        private readonly IHospitalRepository _hospitalRepository;

        public HospitalService(IHospitalRepository hospitalRepository)
        {
            _hospitalRepository = hospitalRepository;
        }

        public async Task<Hospital> AddAsync(Hospital entity)
        {
            return await _hospitalRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(Hospital entity)
        {
            return await _hospitalRepository.Delete(entity);
        }

        public IQueryable<Hospital> GetAll(params Expression<Func<Hospital, object>>[] includes)
        {
            return _hospitalRepository.GetAll(includes);
        }

        public async Task<Hospital> GetByIdAsync(int id)
        {
            return await _hospitalRepository.GetByIdAsync(id);
        }

        public bool IsDuplicated(string hospitalCode)
        {
            return _hospitalRepository.IsDuplicatedHospitalCode(hospitalCode);
        }

        public async Task<bool> UpdateAsync(Hospital entity)
        {
            return await _hospitalRepository.UpdateAsync(entity);
        }
    }
}
