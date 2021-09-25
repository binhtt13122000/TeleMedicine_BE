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
        private readonly DrugTypeRepository _drugTypeRepository;

        public DrugTypeService(DrugTypeRepository drugTypeRepository)
        {
            _drugTypeRepository = drugTypeRepository;
        }
        public async Task<DrugType> AddAsync(DrugType entity)
        {
            return await _drugTypeRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(DrugType entity)
        {
            return await _drugTypeRepository.Delete(entity);
        }

        public IQueryable<DrugType> GetAll(params Expression<Func<DrugType, object>>[] includes)
        {
            return _drugTypeRepository.GetAll(includes);
        }

        public async Task<DrugType> GetByIdAsync(int id)
        {
            return await _drugTypeRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(DrugType entity)
        {
            return await _drugTypeRepository.UpdateAsync(entity);
        }
    }
}
