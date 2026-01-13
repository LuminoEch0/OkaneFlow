using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Service.Models;
using Service.RepoInterface;

namespace OkaneFlow_UnitTests.FakeRepos
{
    public class FakeBankAccountRepo : IBankAccountRepo
    {
        private readonly List<BankAccountModel> _accounts = new List<BankAccountModel>();

        public BankAccountModel? LastAccountCreated { get; private set; }
        public BankAccountModel? LastAccountUpdated { get; private set; }

        public virtual Task<List<BankAccountModel>> GetBankAccountsAsync(Guid id)
        {
            var result = _accounts.Where(a => a.UserID == id).ToList();
            return Task.FromResult(result);
        }

        public virtual Task<BankAccountModel?> GetBankAccountByIdAsync(Guid id)
        {
            var result = _accounts.FirstOrDefault(a => a.AccountID == id);
            return Task.FromResult(result);
        }

        public virtual Task UpdateBankAccountAsync(BankAccountModel account)
        {
            var existing = _accounts.FirstOrDefault(a => a.AccountID == account.AccountID);
            if (existing != null)
            {
                _accounts.Remove(existing);
                _accounts.Add(account);
                LastAccountUpdated = account;
            }
            return Task.CompletedTask;
        }

        public virtual Task DeleteBankAccountAsync(Guid id)
        {
            var existing = _accounts.FirstOrDefault(a => a.AccountID == id);
            if (existing != null)
            {
                _accounts.Remove(existing);
            }
            return Task.CompletedTask;
        }

        public virtual Task CreateBankAccountAsync(BankAccountModel account)
        {
            _accounts.Add(account);
            LastAccountCreated = account;
            return Task.CompletedTask;
        }
    }
}