using DataAccessLayer.Repositories;
using Service.Mappers;
using Service.Models;
using Service.Interface;
using DataAccessLayer.Repositories.Interface;

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
            var dtos = _repository.GetBankAccounts(id);
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

            
            var dto = BankAccountMapper.ToDTO(account);
            _repository.CreateBankAccount(dto);
        }
    }
}