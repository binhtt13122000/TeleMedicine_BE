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
    public interface IDrugService : IService<Drug, int>
    {

    }
    public class DrugService : IDrugService
    {
        private readonly IRepository<Drug, int> _iRepository;

        public DrugService(IRepository<Drug, int> iRepository)
        {
            _iRepository = iRepository;
        }

        public async Task<Drug> AddAsync(Drug entity)
        {
            return await _iRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(Drug entity)
        {
            return await _iRepository.Delete(entity);
        }

        public IQueryable<Drug> GetAll(params Expression<Func<Drug, object>>[] includes)
        {
            return _iRepository.GetAll(includes);
        }

        public async Task<Drug> GetByIdAsync(int id)
        {
            return await _iRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(Drug entity)
        {
            return await _iRepository.UpdateAsync(entity);
        }
    }
}
