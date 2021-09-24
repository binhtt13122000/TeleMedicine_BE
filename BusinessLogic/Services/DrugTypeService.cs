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
    public interface IDrugTypeService : IService<DrugType, int>
    {

    }
    public class DrugTypeService : IDrugTypeService
    {
        private readonly IRepository<DrugType, int> _iRepository;

        public DrugTypeService(IRepository<DrugType, int> iRepository)
        {
            _iRepository = iRepository;
        }
        public async Task<DrugType> AddAsync(DrugType entity)
        {
            return await _iRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(DrugType entity)
        {
            return await _iRepository.Delete(entity);
        }

        public IQueryable<DrugType> GetAll(params Expression<Func<DrugType, object>>[] includes)
        {
            return _iRepository.GetAll(includes);
        }

        public async Task<DrugType> GetByIdAsync(int id)
        {
            return await _iRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(DrugType entity)
        {
            return await _iRepository.UpdateAsync(entity);
        }
    }
}
