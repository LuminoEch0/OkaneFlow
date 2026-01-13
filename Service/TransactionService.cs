using Service.RepoInterface;
using Service.Interface;
using Service.Models;
using System.Transactions;

namespace Service
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepo _transactionRepository;
        private readonly IBankAccountRepo _bankAccountRepository;
        private readonly ICategoryRepo _categoryRepository;
        private readonly ITransactionTypeLookupService _transactionTypeLookupService;
        private readonly ICurrentUserService _currentUserService;


        public TransactionService(
            ITransactionRepo transactionRepository,
            IBankAccountRepo bankAccountRepository,
            ICategoryRepo categoryRepository,
            ITransactionTypeLookupService transactionTypeLookupService,
            ICurrentUserService currentUserService)
        {
            _transactionRepository = transactionRepository;
            _bankAccountRepository = bankAccountRepository;
            _categoryRepository = categoryRepository;
            _transactionTypeLookupService = transactionTypeLookupService;
            _currentUserService = currentUserService;
        }

        public async Task<List<TransactionModel>> GetTransactionsByAccountIdAsync(Guid accountId)
        {
            var account = await _bankAccountRepository.GetBankAccountByIdAsync(accountId);
            if (account == null)
            {
                throw new KeyNotFoundException("Account not found.");
            }

            if (account.UserID != _currentUserService.UserGuid)
            {
                throw new UnauthorizedAccessException("You are not authorized to view transactions for this account.");
            }

            return await _transactionRepository.GetTransactionsByAccountIdAsync(accountId);
        }

        public async Task CreateTransactionAsync(TransactionModel transaction, Guid accountId)
        {
            var account = await _bankAccountRepository.GetBankAccountByIdAsync(accountId);
            if (account == null) throw new KeyNotFoundException("Account not found.");
            if (account.UserID != _currentUserService.UserGuid)
            {
                throw new UnauthorizedAccessException("You are not authorized to add transactions to this account.");
            }

            int incomeTypeId = await _transactionTypeLookupService.GetTypeIdFromNameAsync("Income");
            int expenseTypeId = await _transactionTypeLookupService.GetTypeIdFromNameAsync("Expense");

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (transaction.TransactionID == Guid.Empty)
                {
                    transaction.TransactionID = Guid.NewGuid();
                }

                // If no category selected, auto-assign to "Unallocated" category
                if (transaction.CategoryID == Guid.Empty)
                {
                    var unallocated = await _categoryRepository.GetUnallocatedCategoryAsync(accountId);
                    transaction.CategoryID = unallocated.CategoryID;
                }
                else
                {
                    // Verify category exists
                    var category = await _categoryRepository.GetCategoryByIdAsync(transaction.CategoryID);
                    if (category == null)
                    {
                        var unallocated = await _categoryRepository.GetUnallocatedCategoryAsync(accountId);
                        transaction.CategoryID = unallocated.CategoryID;
                    }
                }

                await _transactionRepository.AddTransactionAsync(transaction);

                if (transaction.Type == incomeTypeId)
                {
                    account.CurrentBalance += transaction.Amount;
                }
                else
                {
                    account.CurrentBalance -= transaction.Amount;
                }
                await _bankAccountRepository.UpdateBankAccountAsync(account);

                if (transaction.Type == expenseTypeId)
                {
                    var category = await _categoryRepository.GetCategoryByIdAsync(transaction.CategoryID);
                    if (category != null)
                    {
                        category.AmountUsed += transaction.Amount;
                        await _categoryRepository.UpdateCategoryAsync(category);
                    }
                }

                scope.Complete();
            }
        }

        public async Task DeleteTransactionAsync(Guid id)
        {
            var transaction = await _transactionRepository.GetTransactionByIdAsync(id);
            if (transaction == null) return;

            int incomeTypeId = await _transactionTypeLookupService.GetTypeIdFromNameAsync("Income");
            int expenseTypeId = await _transactionTypeLookupService.GetTypeIdFromNameAsync("Expense");

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var category = await _categoryRepository.GetCategoryByIdAsync(transaction.CategoryID);
                if (category != null)
                {
                    var account = await _bankAccountRepository.GetBankAccountByIdAsync(category.AccountID);
                    if (account != null)
                    {
                        if (account.UserID != _currentUserService.UserGuid)
                        {
                            throw new UnauthorizedAccessException("You are not authorized to delete this transaction.");
                        }

                        // Reverse effects on Balance
                        if (transaction.Type == incomeTypeId)
                        {
                            account.CurrentBalance -= transaction.Amount;
                        }
                        else
                        {
                            account.CurrentBalance += transaction.Amount;
                        }
                        await _bankAccountRepository.UpdateBankAccountAsync(account);
                    }

                    // Reverse effects on Category
                    if (transaction.Type == expenseTypeId)
                    {
                        category.AmountUsed -= transaction.Amount;
                        await _categoryRepository.UpdateCategoryAsync(category);
                    }
                }

                await _transactionRepository.DeleteTransactionAsync(id);
                scope.Complete();
            }
        }

        public async Task<TransactionModel?> GetTransactionByIdAsync(Guid id)
        {
            var transaction = await _transactionRepository.GetTransactionByIdAsync(id);
            if (transaction == null) return null;

            var category = await _categoryRepository.GetCategoryByIdAsync(transaction.CategoryID);
            if (category != null)
            {
                var account = await _bankAccountRepository.GetBankAccountByIdAsync(category.AccountID);
                if (account != null && account.UserID != _currentUserService.UserGuid)
                {
                    throw new UnauthorizedAccessException("You are not authorized to view this transaction.");
                }
            }
            return transaction;
        }

        public async Task UpdateTransactionAsync(TransactionModel transaction)
        {
            var oldTransaction = await _transactionRepository.GetTransactionByIdAsync(transaction.TransactionID);
            if (oldTransaction == null) return;

            int incomeTypeId = await _transactionTypeLookupService.GetTypeIdFromNameAsync("Income");
            int expenseTypeId = await _transactionTypeLookupService.GetTypeIdFromNameAsync("Expense");

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var oldCategory = await _categoryRepository.GetCategoryByIdAsync(oldTransaction.CategoryID);
                Guid accountId = Guid.Empty;

                if (oldCategory != null)
                {
                    var account = await _bankAccountRepository.GetBankAccountByIdAsync(oldCategory.AccountID);
                    if (account != null)
                    {
                        if (account.UserID != _currentUserService.UserGuid)
                        {
                            throw new UnauthorizedAccessException("You are not authorized to update this transaction.");
                        }

                        accountId = account.AccountID;

                        if (oldTransaction.Type == incomeTypeId)
                        {
                            account.CurrentBalance -= oldTransaction.Amount;
                        }
                        else
                        {
                            account.CurrentBalance += oldTransaction.Amount;
                        }
                        await _bankAccountRepository.UpdateBankAccountAsync(account);
                    }

                    if (oldTransaction.Type == expenseTypeId)
                    {
                        oldCategory.AmountUsed -= oldTransaction.Amount;
                        await _categoryRepository.UpdateCategoryAsync(oldCategory);
                    }
                }

                if (accountId == Guid.Empty) accountId = oldCategory?.AccountID ?? Guid.Empty;

                var newCheckingCategory = await _categoryRepository.GetCategoryByIdAsync(transaction.CategoryID);
                if (newCheckingCategory != null)
                {
                    var newAccount = await _bankAccountRepository.GetBankAccountByIdAsync(newCheckingCategory.AccountID);
                    if (newAccount != null && newAccount.UserID != _currentUserService.UserGuid)
                    {
                        throw new UnauthorizedAccessException("You cannot move transaction to a category you do not own.");
                    }
                }


                if (transaction.Type == incomeTypeId)
                {
                    var category = await _categoryRepository.GetCategoryByIdAsync(transaction.CategoryID);
                    if (category == null && accountId != Guid.Empty)
                    {
                        var unallocated = await _categoryRepository.GetUnallocatedCategoryAsync(accountId);
                        transaction.CategoryID = unallocated.CategoryID;
                    }
                }

                await _transactionRepository.UpdateTransactionAsync(transaction);

                // Update Balance/Category for NEW transaction
                var newCategory = await _categoryRepository.GetCategoryByIdAsync(transaction.CategoryID);
                if (newCategory != null)
                {
                    var account = await _bankAccountRepository.GetBankAccountByIdAsync(newCategory.AccountID);
                    if (account != null)
                    {
                        if (transaction.Type == incomeTypeId)
                        {
                            account.CurrentBalance += transaction.Amount;
                        }
                        else
                        {
                            account.CurrentBalance -= transaction.Amount;
                        }
                        await _bankAccountRepository.UpdateBankAccountAsync(account);
                    }

                    if (transaction.Type == expenseTypeId)
                    {
                        newCategory.AmountUsed += transaction.Amount;
                        await _categoryRepository.UpdateCategoryAsync(newCategory);
                    }
                }

                scope.Complete();
            }
        }
    }
}
