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
    public interface IDoctorService: IService<Doctor, int>
    {
    }
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;

        public DoctorService(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }
        public async Task<Doctor> AddAsync(Doctor entity)
        {
            return await _doctorRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(Doctor entity)
        {
            return await _doctorRepository.Delete(entity);
        }

        public IQueryable<Doctor> GetAll(params Expression<Func<Doctor, object>>[] includes)
        {
            return _doctorRepository.GetAll(includes);
        }

        public async Task<Doctor> GetByIdAsync(int id)
        {
            return await _doctorRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(Doctor entity)
        {
            return await _doctorRepository.UpdateAsync(entity);
        }
    }
}
