using Infrastructure.Interfaces;
using Infrastructure.Interfaces.Implements;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IRoleRepository : IRepository<Role, int>
    {
        bool IsDuplicated(String name);
    }
    public class RoleRepository: Repository<Role, int>, IRoleRepository
    {
        public RoleRepository(TeleMedicineContext dbContext) : base(dbContext)
        {

        }

        public bool IsDuplicated(string name)
        {
            Role checkRoleExisted = GetAll().Where(s => name.Trim().ToUpper().Equals(s.Name.ToUpper())).FirstOrDefault();
            return checkRoleExisted != null;
        }
    }
}
