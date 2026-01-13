using Service.Models;
using Service.Interface;
using Service.RepoInterface;

namespace Service
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IBankAccountRepo _repository;
        private readonly ICurrentUserService _currentUserService;

        public BankAccountService(IBankAccountRepo repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        [Obsolete("Use GetAccountsByUserId instead. This method will be removed in a future version.")]
        public async Task<List<BankAccountModel>> GetAllBankAccountsAsync(Guid id)
        {
            return await GetAccountsByUserIdAsync(id);
        }

        public async Task<List<BankAccountModel>> GetAccountsByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty) throw new ArgumentException("User ID cannot be empty.", nameof(userId));

            if (userId != _currentUserService.UserGuid)
            {
                throw new UnauthorizedAccessException("You are not authorized to view these accounts.");
            }

            try
            {
                return await _repository.GetBankAccountsAsync(userId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while retrieving accounts for user with ID {userId}.", ex);
            }
        }

        public async Task<BankAccountModel?> GetAccountByIdAsync(Guid accountId)
        {
            if (accountId == Guid.Empty) throw new ArgumentException("Account ID cannot be empty.", nameof(accountId));

            try
            {
                var account = await _repository.GetBankAccountByIdAsync(accountId);
                if (account == null) return null;

                if (account.UserID != _currentUserService.UserGuid)
                {
                    throw new UnauthorizedAccessException("You are not authorized to view this account.");
                }

                return account;
            }
            catch (UnauthorizedAccessException) { throw; }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while retrieving account with ID {accountId}.", ex);
            }
        }

        public async Task UpdateAccountDetailsAsync(BankAccountModel account)
        {
            if (account == null) throw new ArgumentNullException(nameof(account), "Account cannot be null.");
            if (account.AccountID == Guid.Empty) throw new ArgumentException("Account ID cannot be empty.", nameof(account));
            if (string.IsNullOrWhiteSpace(account.AccountName)) throw new ArgumentException("Account name cannot be empty.", nameof(account));

            try
            {
                var existing = await _repository.GetBankAccountByIdAsync(account.AccountID);
                if (existing == null) throw new KeyNotFoundException($"Account with ID {account.AccountID} was not found.");

                if (existing.UserID != _currentUserService.UserGuid)
                {
                    throw new UnauthorizedAccessException("You are not authorized to update this account.");
                }

                await _repository.UpdateBankAccountAsync(account);
            }
            catch (UnauthorizedAccessException) { throw; }
            catch (KeyNotFoundException) { throw; }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while updating account with ID {account.AccountID}.", ex);
            }
        }

        public async Task DeleteAccountAsync(Guid accountId)
        {
            if (accountId == Guid.Empty) throw new ArgumentException("Account ID cannot be empty.", nameof(accountId));

            try
            {
                var account = await _repository.GetBankAccountByIdAsync(accountId);
                if (account == null)
                {
                    throw new KeyNotFoundException($"Account with ID {accountId} was not found.");
                }

                if (account.UserID != _currentUserService.UserGuid)
                {
                    throw new UnauthorizedAccessException("You are not authorized to delete this account.");
                }

                await _repository.DeleteBankAccountAsync(accountId);
            }
            catch (UnauthorizedAccessException) { throw; }
            catch (KeyNotFoundException) { throw; }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while deleting account with ID {accountId}.", ex);
            }
        }

        public async Task CreateAccountAsync(BankAccountModel account)
        {
            if (account == null) throw new ArgumentNullException(nameof(account), "Account cannot be null.");
            if (account.UserID == Guid.Empty) throw new ArgumentException("User ID cannot be empty.", nameof(account));
            if (account.CurrentBalance < 0) throw new ArgumentException("Initial balance cannot be negative.", nameof(account));
            if (string.IsNullOrWhiteSpace(account.AccountName)) throw new ArgumentException("Account name cannot be empty.", nameof(account));

            if (account.UserID != _currentUserService.UserGuid)
            {
                throw new UnauthorizedAccessException("You cannot create an account for another user.");
            }

            try
            {
                await _repository.CreateBankAccountAsync(account);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while creating the bank account.", ex);
            }
        }
    }
}
