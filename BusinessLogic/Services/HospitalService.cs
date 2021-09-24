using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Models;
using Infrastructure.Interfaces;
using System.Linq.Expressions;
using Infrastructure.Repositories;

namespace BusinessLogic.Services
{
    public interface IHospitalService : IService<Hospital, int>
    {

    }
    public class HospitalService : IHospitalService
    {
        private readonly HospitalRepository _iHospitalRepository;

        public HospitalService(HospitalRepository iHospitalRepository)
        {
            _iHospitalRepository = iHospitalRepository;
        }

        public async Task<Hospital> AddAsync(Hospital entity)
        {
            return await _iHospitalRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(Hospital entity)
        {
            return await _iHospitalRepository.Delete(entity);
        }

        public IQueryable<Hospital> GetAll(params Expression<Func<Hospital, object>>[] includes)
        {
            return _iHospitalRepository.GetAll(includes);
        }

        public async Task<Hospital> GetByIdAsync(int id)
        {
            return await _iHospitalRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(Hospital entity)
        {
            return await _iHospitalRepository.UpdateAsync(entity);
        }
    }
}
