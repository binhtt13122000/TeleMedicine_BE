using Infrastructure.Interfaces;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public interface ICertificationService : IService<Certification, int>
    {

    }
    public class CertificationService : ICertificationService
    {
        private readonly IRepository<Certification, int> _iRepository;

        public CertificationService(IRepository<Certification, int> iRepository)
        {
            _iRepository = iRepository;
        }

        public async Task<Certification> AddAsync(Certification entity)
        {
            return await _iRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(Certification entity)
        {
            return await _iRepository.Delete(entity);
        }

        public IQueryable<Certification> GetAll(params Expression<Func<Certification, object>>[] includes)
        {
            return _iRepository.GetAll(includes);
        }

        public async Task<Certification> GetByIdAsync(int id)
        {
            return await _iRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(Certification entity)
        {
            return await _iRepository.UpdateAsync(entity);
        }
    }
}
