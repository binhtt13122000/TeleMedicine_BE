using Infrastructure.Models;
using Infrastructure.Repositories;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public interface ICertificationService : IService<Certification, int>
    {

    }
    public class CertificationService : ICertificationService
    {
        private readonly ICertificationRepository _certificationRepository;

        public CertificationService(ICertificationRepository certificationRepository)
        {
            _certificationRepository = certificationRepository;
        }

        public async Task<Certification> AddAsync(Certification entity)
        {
            return await _certificationRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(Certification entity)
        {
            return await _certificationRepository.Delete(entity);
        }

        public IQueryable<Certification> GetAll(params Expression<Func<Certification, object>>[] includes)
        {
            return _certificationRepository.GetAll(includes);
        }

        public async Task<Certification> GetByIdAsync(int id)
        {
            return await _certificationRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(Certification entity)
        {
            return await _certificationRepository.UpdateAsync(entity);
        }
    }
}
