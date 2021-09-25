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
        private readonly IDrugRepository _drugRepository;

        public DrugService(IDrugRepository drugRepository)
        {
            _drugRepository = drugRepository;
        }

        public async Task<Drug> AddAsync(Drug entity)
        {
            return await _drugRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(Drug entity)
        {
            return await _drugRepository.Delete(entity);
        }

        public IQueryable<Drug> GetAll(params Expression<Func<Drug, object>>[] includes)
        {
            return _drugRepository.GetAll(includes);
        }

        public async Task<Drug> GetByIdAsync(int id)
        {
            return await _drugRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(Drug entity)
        {
            return await _drugRepository.UpdateAsync(entity);
        }
    }
}
