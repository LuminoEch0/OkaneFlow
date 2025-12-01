using DataAccessLayer.Repositories;
using Service.Mappers;
using Service.Models;

namespace Service
{
    public class BankAccountService
    {
        private readonly BankAccountRepository _repository;

        public BankAccountService(BankAccountRepository repository)
        {
            _repository = repository;
        }

        public List<BankAccountModel> GetAllBankAccounts()
        {
            var dtos = _repository.GetBankAccounts();
            return BankAccountMapper.ToModelList(dtos);
        }

        public BankAccountModel? GetAccountById(Guid accountId)
        {
            var dto = _repository.GetBankAccountById(accountId);
            return dto == null ? null : BankAccountMapper.ToModel(dto);
        }

        public void UpdateAccountDetails(BankAccountModel account)
        {
            var dto = BankAccountMapper.ToDTO(account);
            _repository.UpdateBankAccount(dto);
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

            account.UserID = Guid.Parse("0588CE68-E40A-402E-86E4-A902265B2A9B");
            var dto = BankAccountMapper.ToDTO(account);
            _repository.CreateBankAccount(dto);
        }
    }
}