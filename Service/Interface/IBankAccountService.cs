using Service.Models;

namespace Service.Interface
{
    public interface IBankAccountService
    {
        public List<BankAccountModel> GetAllBankAccounts(Guid id);

        public BankAccountModel? GetAccountById(Guid accountId);

        public void UpdateAccountDetails(BankAccountModel account);

        public void DeleteAccount(Guid accountId);
        public void CreateAccount(BankAccountModel account);
    }
}