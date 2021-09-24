using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Models;
using Infrastructure.Interfaces;
using System.Linq.Expressions;

namespace BusinessLogic.Services
{
    public interface IHospitalService : IService<Hospital, int>
    {

    }
    public class HospitalService : IHospitalService
    {
        private readonly IRepository<Hospital, int> _iRepository;

        public HospitalService(IRepository<Hospital, int> iRepository)
        {
            _iRepository = iRepository;
        }

        public async Task<Hospital> AddAsync(Hospital entity)
        {
            return await _iRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(Hospital entity)
        {
            return await _iRepository.Delete(entity);
        }

        public IQueryable<Hospital> GetAll(params Expression<Func<Hospital, object>>[] includes)
        {
            return _iRepository.GetAll(includes);
        }

        public async Task<Hospital> GetByIdAsync(int id)
        {
            return await _iRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(Hospital entity)
        {
            return await _iRepository.UpdateAsync(entity);
        }
    }
}
