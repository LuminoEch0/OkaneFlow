using Service.Models;

namespace Service.Interface
{
    public interface IBankAccountService
    {
        [Obsolete("Use GetAccountsByUserId instead. This method will be removed in a future version.")]
        public Task<List<BankAccountModel>> GetAllBankAccountsAsync(Guid id);

        public Task<List<BankAccountModel>> GetAccountsByUserIdAsync(Guid userId);

        public Task<BankAccountModel?> GetAccountByIdAsync(Guid accountId);

        public Task UpdateAccountDetailsAsync(BankAccountModel account);

        public Task DeleteAccountAsync(Guid accountId);

        public Task CreateAccountAsync(BankAccountModel account);
    }
}