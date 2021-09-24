using Infrastructure.Models;
using Infrastructure.Repositories;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public interface IDrugService : IService<Drug, int>
    {

    }
    public class DrugService : IDrugService
    {
        private readonly DrugRepository _iDrugRepository;

        public DrugService(DrugRepository iDrugRepository)
        {
            _iDrugRepository = iDrugRepository;
        }

        public async Task<Drug> AddAsync(Drug entity)
        {
            return await _iDrugRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(Drug entity)
        {
            return await _iDrugRepository.Delete(entity);
        }

        public IQueryable<Drug> GetAll(params Expression<Func<Drug, object>>[] includes)
        {
            return _iDrugRepository.GetAll(includes);
        }

        public async Task<Drug> GetByIdAsync(int id)
        {
            return await _iDrugRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(Drug entity)
        {
            return await _iDrugRepository.UpdateAsync(entity);
        }
    }
}
