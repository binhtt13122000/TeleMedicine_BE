using Infrastructure.Models;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
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
        Doctor GetDoctorByEmail(string email);
        DbSet<Doctor> access();
    }
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;

        public DoctorService(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        public DbSet<Doctor> access()
        {
            return _doctorRepository.access();
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

        public Doctor GetDoctorByEmail(string email)
        {
            return _doctorRepository.GetAll().Where(x => x.Email.ToUpper().Equals(email.ToUpper())).FirstOrDefault();
        }

        public async Task<bool> UpdateAsync(Doctor entity)
        {
            return await _doctorRepository.UpdateAsync(entity);
        }
    }
}
