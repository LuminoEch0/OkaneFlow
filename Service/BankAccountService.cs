
using Service.Models;
using Service.Interface;
using Service.RepoInterface;

namespace Service
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IBankAccountRepo _repository;

        public BankAccountService(IBankAccountRepo repository)
        {
            _repository = repository;
        }

        public List<BankAccountModel> GetAllBankAccounts(Guid id)
        {
            return _repository.GetBankAccounts(id);
        }

        public BankAccountModel? GetAccountById(Guid accountId)
        {
            return _repository.GetBankAccountById(accountId);
        }

        public void UpdateAccountDetails(BankAccountModel account)
        {
            _repository.UpdateBankAccount(account);
        }

        public void DeleteAccount(Guid accountId)
        {
            var account = GetAccountById(accountId);
            _repository.DeleteBankAccount(accountId);
        }
        public void CreateAccount(BankAccountModel account)
        {
            if (account.CurrentBalance < 0)
            {
                throw new ArgumentException("Initial balance cannot be negative.");
            }
            if (string.IsNullOrWhiteSpace(account.AccountName))
            {
                throw new ArgumentException("Account name cannot be empty.");
            }


            _repository.CreateBankAccount(account);
        }
    }
}