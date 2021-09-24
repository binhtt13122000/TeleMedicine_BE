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
        private readonly CertificationRepository _iCertificationRepository;

        public CertificationService(CertificationRepository iCertificationRepository)
        {
            _iCertificationRepository = iCertificationRepository;
        }

        public async Task<Certification> AddAsync(Certification entity)
        {
            return await _iCertificationRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(Certification entity)
        {
            return await _iCertificationRepository.Delete(entity);
        }

        public IQueryable<Certification> GetAll(params Expression<Func<Certification, object>>[] includes)
        {
            return _iCertificationRepository.GetAll(includes);
        }

        public async Task<Certification> GetByIdAsync(int id)
        {
            return await _iCertificationRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(Certification entity)
        {
            return await _iCertificationRepository.UpdateAsync(entity);
        }
    }
}
