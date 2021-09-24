using Infrastructure.Models;
using Infrastructure.Repositories;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public interface IDrugTypeService : IService<DrugType, int>
    {

    }
    public class DrugTypeService : IDrugTypeService
    {
        private readonly DrugTypeRepository _iDrugTypeRepository;

        public DrugTypeService(DrugTypeRepository iDrugTypeRepository)
        {
            _iDrugTypeRepository = iDrugTypeRepository;
        }
        public async Task<DrugType> AddAsync(DrugType entity)
        {
            return await _iDrugTypeRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(DrugType entity)
        {
            return await _iDrugTypeRepository.Delete(entity);
        }

        public IQueryable<DrugType> GetAll(params Expression<Func<DrugType, object>>[] includes)
        {
            return _iDrugTypeRepository.GetAll(includes);
        }

        public async Task<DrugType> GetByIdAsync(int id)
        {
            return await _iDrugTypeRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(DrugType entity)
        {
            return await _iDrugTypeRepository.UpdateAsync(entity);
        }
    }
}
