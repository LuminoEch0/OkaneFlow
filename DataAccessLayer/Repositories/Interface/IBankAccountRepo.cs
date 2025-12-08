using DataAccessLayer.DataTransferObjects;

namespace DataAccessLayer.Repositories.Interface
{
    public interface IBankAccountRepo
    {
        public List<BankAccountDTO> GetBankAccounts(Guid id);

        public BankAccountDTO? GetBankAccountById(Guid id);

        public void UpdateBankAccount(BankAccountDTO dto);

        public void DeleteBankAccount(Guid id);

        public void CreateBankAccount(BankAccountDTO newAccount);
    }
}