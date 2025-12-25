using Service.Models;

namespace Service.RepoInterface
{
    public interface IBankAccountRepo
    {
        public List<BankAccountModel> GetBankAccounts(Guid id);

        public BankAccountModel? GetBankAccountById(Guid id);

        public void UpdateBankAccount(BankAccountModel dto);

        public void DeleteBankAccount(Guid id);

        public void CreateBankAccount(BankAccountModel newAccount);
    }
}