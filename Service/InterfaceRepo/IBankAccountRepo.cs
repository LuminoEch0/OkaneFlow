using Service.Models;

namespace Service.RepoInterface
{
    public interface IBankAccountRepo
    {
        public Task<List<BankAccountModel>> GetBankAccountsAsync(Guid id);

        public Task<BankAccountModel?> GetBankAccountByIdAsync(Guid id);

        public Task UpdateBankAccountAsync(BankAccountModel dto);

        public Task DeleteBankAccountAsync(Guid id);

        public Task CreateBankAccountAsync(BankAccountModel newAccount);
    }
}