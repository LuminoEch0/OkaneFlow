using Service.RepoInterface;
using Service.Interface;
using Service.Models;
using System.Transactions;

namespace Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepo _repository;
        private readonly ITransactionRepo _transactionRepository;
        private readonly IBankAccountRepo _bankAccountRepository;
        private readonly ISubscriptionRepo _subscriptionRepository;
        private readonly ITransactionTypeLookupService _transactionTypeLookupService;
        private readonly ICurrentUserService _currentUserService;

        public CategoryService(
            ICategoryRepo repository,
            ITransactionRepo transactionRepository,
            IBankAccountRepo bankAccountRepository,
            ISubscriptionRepo subscriptionRepository,
            ITransactionTypeLookupService transactionTypeLookupService,
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _transactionRepository = transactionRepository;
            _bankAccountRepository = bankAccountRepository;
            _subscriptionRepository = subscriptionRepository;
            _transactionTypeLookupService = transactionTypeLookupService;
            _currentUserService = currentUserService;
        }

        public async Task<List<CategoryModel>> GetAllCategoriesAsync(Guid accountId)
        {
            var account = await _bankAccountRepository.GetBankAccountByIdAsync(accountId);
            if (account == null) throw new KeyNotFoundException("Account not found.");

            if (account.UserID != _currentUserService.UserGuid)
            {
                throw new UnauthorizedAccessException("You are not authorized to view categories for this account.");
            }

            return await _repository.GetCategoriesAsync(accountId);
        }

        public async Task<CategoryModel?> GetCategoryByIdAsync(Guid categoryId)
        {
            var category = await _repository.GetCategoryByIdAsync(categoryId);
            if (category == null) return null;

            var account = await _bankAccountRepository.GetBankAccountByIdAsync(category.AccountID);
            if (account != null && account.UserID != _currentUserService.UserGuid)
            {
                throw new UnauthorizedAccessException("You are not authorized to view this category.");
            }

            return category;
        }

        public async Task UpdateCategoryDetailsAsync(CategoryModel category)
        {
            var existing = await _repository.GetCategoryByIdAsync(category.CategoryID);
            if (existing == null) return;

            var account = await _bankAccountRepository.GetBankAccountByIdAsync(existing.AccountID);
            if (account == null || account.UserID != _currentUserService.UserGuid)
            {
                throw new UnauthorizedAccessException("You are not authorized to update this category.");
            }

            await _repository.UpdateCategoryAsync(category);
        }

        public async Task DeleteCategoryAsync(Guid categoryId)
        {
            var category = await _repository.GetCategoryByIdAsync(categoryId);
            if (category == null) return;

            var account = await _bankAccountRepository.GetBankAccountByIdAsync(category.AccountID);
            if (account == null || account.UserID != _currentUserService.UserGuid)
            {
                throw new UnauthorizedAccessException("You are not authorized to delete this category.");
            }

            // Prevent deleting "Unallocated"
            if (string.Equals(category.CategoryName, "Unallocated", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("You cannot delete the 'Unallocated' category.");
            }

            // Prevent deleting "Subscriptions" if it has subscriptions
            if (string.Equals(category.CategoryName, "Subscriptions", StringComparison.OrdinalIgnoreCase))
            {
                var subCount = await _subscriptionRepository.GetSubscriptionCountByCategoryIdAsync(categoryId);
                if (subCount > 0)
                {
                    // More user-friendly message
                    throw new InvalidOperationException($"Cannot delete 'Subscriptions' category. It has {subCount} active subscription(s). Please move or delete them first.");
                }
            }

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var unallocated = await _repository.GetUnallocatedCategoryAsync(category.AccountID);

                int expenseTypeId = await _transactionTypeLookupService.GetTypeIdFromNameAsync("Expense");
                await _transactionRepository.UpdateCategoryForTransactionsAsync(category.CategoryID, unallocated.CategoryID, expenseTypeId);

                await _repository.DeleteCategoryAsync(categoryId);

                scope.Complete();
            }
        }

        public async Task CreateCategoryAsync(CategoryModel category)
        {
            if (category.AllocatedAmount < 0 || category.AmountUsed < 0)
            {
                throw new ArgumentException("Allocated Amount or Amount used cannot be negative.");
            }

            if (string.IsNullOrWhiteSpace(category.CategoryName))
            {
                throw new ArgumentException("Category name cannot be empty.");
            }

            var account = await _bankAccountRepository.GetBankAccountByIdAsync(category.AccountID);
            if (account != null && account.UserID != _currentUserService.UserGuid)
            {
                throw new UnauthorizedAccessException("You are not authorized to create a category for this account.");
            }

            await _repository.CreateCategoryAsync(category);
        }

        public async Task AssignAmountToAllocateAsync(Guid categoryId, decimal amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Amount cannot be negative.");
            }

            var category = await _repository.GetCategoryByIdAsync(categoryId);
            if (category == null) return;

            var account = await _bankAccountRepository.GetBankAccountByIdAsync(category.AccountID);
            if (account != null && account.UserID != _currentUserService.UserGuid)
            {
                throw new UnauthorizedAccessException("You are not authorized to update this category.");
            }

            await _repository.AssignAmountAllocatedAsync(categoryId, amount);
        }

        public async Task<decimal> GetUnallocatedAmountAsync(Guid accountId, IBankAccountService bankAccountService)
        {
            var account = await _bankAccountRepository.GetBankAccountByIdAsync(accountId);
            if (account != null && account.UserID != _currentUserService.UserGuid)
            {
                throw new UnauthorizedAccessException("You are not authorized to view this account.");
            }
            if (account == null) return 0;

            var categories = await GetAllCategoriesAsync(accountId); // This already checks IDOR
            var totalAllocated = categories.Sum(c => c.AllocatedAmount);
            var totalSpent = categories.Sum(c => c.AmountUsed);

            return account.CurrentBalance + totalSpent - totalAllocated;
        }
    }
}
