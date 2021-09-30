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
    public interface IAccountService : IService<Account, int>
    {
        Account GetAccountByEmail(string email);
    }
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public async Task<Account> AddAsync(Account entity)
        {
            return await _accountRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(Account entity)
        {
            return await _accountRepository.Delete(entity);
        }

        public Account GetAccountByEmail(string email)
        {
            return _accountRepository.GetAccountByEmail(email);
        }

        public IQueryable<Account> GetAll(params Expression<Func<Account, object>>[] includes)
        {
            return _accountRepository.GetAll(includes);
        }

        public async Task<Account> GetByIdAsync(int id)
        {
            return await _accountRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(Account entity)
        {
            return await _accountRepository.UpdateAsync(entity);
        }
    }
}
