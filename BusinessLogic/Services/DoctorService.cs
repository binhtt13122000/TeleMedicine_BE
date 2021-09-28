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
        bool isDuplicatedCertificationCode(string certificationCode);

        bool isDuplicatedEmail(string email);
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

        public bool isDuplicatedCertificationCode(string certificationCode)
        {
            return _doctorRepository.isDuplicatedCertificationCode(certificationCode);
        }

        public bool isDuplicatedEmail(string email)
        {
            return _doctorRepository.isDuplicatedEmail(email);
        }

        public async Task<bool> UpdateAsync(Doctor entity)
        {
            return await _doctorRepository.UpdateAsync(entity);
        }
    }
}
