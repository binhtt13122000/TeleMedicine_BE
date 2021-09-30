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
    public interface IAccountRepository : IRepository<Account, int>
    {
        Account GetAccountByEmail(string email);    
    }
    public class AccountRepository : Repository<Account, int>, IAccountRepository
    {
        public AccountRepository(TeleMedicineContext dbContext) : base(dbContext)
        {

        }

        public Account GetAccountByEmail(string email)
        {
            return GetAll(x => x.Role).Where(x => x.Email.ToUpper().Equals(email.ToUpper())).FirstOrDefault();
        }
    }
}
