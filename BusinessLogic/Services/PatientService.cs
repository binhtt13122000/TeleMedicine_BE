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
    public interface IPatientService : IService<Patient, int>
    {
        Patient GetPatientByEmail(string email);
    }
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;

        public PatientService(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }
        public async Task<Patient> AddAsync(Patient entity)
        {
            return await _patientRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(Patient entity)
        {
            return await _patientRepository.Delete(entity);
        }

        public IQueryable<Patient> GetAll(params Expression<Func<Patient, object>>[] includes)
        {
            return _patientRepository.GetAll(includes);
        }

        public async Task<Patient> GetByIdAsync(int id)
        {
            return await _patientRepository.GetByIdAsync(id);
        }

        public Patient GetPatientByEmail(string email)
        {
            return _patientRepository.GetAll().Where(x => x.Email.ToUpper().Equals(email.ToUpper())).FirstOrDefault();
        }

        public async Task<bool> UpdateAsync(Patient entity)
        {
            return await _patientRepository.UpdateAsync(entity);
        }
    }
}
