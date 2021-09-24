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
    public interface IRoleService : IService<Role, int>
    {
    }
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }
        public async Task<Role> AddAsync(Role entity)
        {
            return await _roleRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(Role entity)
        {
            return await _roleRepository.Delete(entity);
        }

        public IQueryable<Role> GetAll(params Expression<Func<Role, object>>[] includes)
        {
            return _roleRepository.GetAll(includes);
        }

        public async Task<Role> GetByIdAsync(int id)
        {
            return await _roleRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(Role entity)
        {
            return await _roleRepository.UpdateAsync(entity);
        }
    }
}
