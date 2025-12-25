using Service.RepoInterface;
using Service.Interface;
using Service.Models;

namespace Service
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepo _transactionRepository;
        private readonly IBankAccountRepo _bankAccountRepository;
        private readonly ICategoryRepo _categoryRepository;
        private readonly ITransactionTypeLookupService _transactionTypeLookupService;

        private readonly int _expenseTypeId;
        private readonly int _incomeTypeId;

        public TransactionService(ITransactionRepo transactionRepository, IBankAccountRepo bankAccountRepository, ICategoryRepo categoryRepository, ITransactionTypeLookupService transactionTypeLookupService)
        {
            _transactionRepository = transactionRepository;
            _bankAccountRepository = bankAccountRepository;
            _categoryRepository = categoryRepository; // temporary to satisfy compiler
            _transactionTypeLookupService = transactionTypeLookupService;

            _expenseTypeId = _transactionTypeLookupService.GetTypeIdFromName("Expense");
            _incomeTypeId = _transactionTypeLookupService.GetTypeIdFromName("Income");
        }

        public List<TransactionModel> GetTransactionsByAccountId(Guid accountId)
        {
            return _transactionRepository.GetTransactionsByAccountId(accountId);
        }

        public void CreateTransaction(TransactionModel transaction, Guid accountId)
        {
            if (transaction.TransactionID == Guid.Empty)
            {
                transaction.TransactionID = Guid.NewGuid();
            }
            if (transaction.Type == _incomeTypeId)
            {
                var category = _categoryRepository.GetCategoryById(transaction.CategoryID);
                if (category == null)
                {
                    var unassigned = _categoryRepository.GetUnassignedCategory(accountId);
                    transaction.CategoryID = unassigned.CategoryID;
                }
            }

            // 2. Save Transaction
            _transactionRepository.AddTransaction(transaction);

            // 3. Update Bank Account Balance
            var account = _bankAccountRepository.GetBankAccountById(accountId);
            if (account != null)
            {
                if (transaction.Type == _incomeTypeId)
                {
                    account.CurrentBalance += transaction.Amount;
                }
                else
                {
                    account.CurrentBalance -= transaction.Amount;
                }
                _bankAccountRepository.UpdateBankAccount(account);
            }

            // 4. Update Category AmountUsed
            if (transaction.Type == _expenseTypeId)
            {
                var category = _categoryRepository.GetCategoryById(transaction.CategoryID);
                if (category != null)
                {
                    category.AmountUsed += transaction.Amount;
                    _categoryRepository.UpdateCategory(category);
                }
            }
        }

        public void DeleteTransaction(Guid id)
        {
            var transaction = _transactionRepository.GetTransactionById(id);
            if (transaction == null) return;

            // Reverse effects
            var category = _categoryRepository.GetCategoryById(transaction.CategoryID);
            if (category != null)
            {
                var account = _bankAccountRepository.GetBankAccountById(category.AccountID);
                if (account != null)
                {
                    if (transaction.Type == _incomeTypeId)
                    {
                        account.CurrentBalance -= transaction.Amount;
                    }
                    else
                    {
                        account.CurrentBalance += transaction.Amount;
                    }
                    _bankAccountRepository.UpdateBankAccount(account);
                }

                if (transaction.Type == _expenseTypeId)
                {
                    category.AmountUsed -= transaction.Amount;
                    _categoryRepository.UpdateCategory(category);
                }
            }

            _transactionRepository.DeleteTransaction(id);
        }

        public TransactionModel? GetTransactionById(Guid id)
        {
            return _transactionRepository.GetTransactionById(id);
        }

        public void UpdateTransaction(TransactionModel transaction)
        {
            var oldTransaction = _transactionRepository.GetTransactionById(transaction.TransactionID);
            if (oldTransaction == null) return;

            // 1. Revert Old
            var oldCategory = _categoryRepository.GetCategoryById(oldTransaction.CategoryID);
            if (oldCategory != null)
            {
                var account = _bankAccountRepository.GetBankAccountById(oldCategory.AccountID);
                if (account != null)
                {
                    if (oldTransaction.Type == _incomeTypeId)
                    {
                        account.CurrentBalance -= oldTransaction.Amount;
                    }
                    else
                    {
                        account.CurrentBalance += oldTransaction.Amount;
                    }
                    _bankAccountRepository.UpdateBankAccount(account);
                }

                if (oldTransaction.Type == _expenseTypeId)
                {
                    oldCategory.AmountUsed -= oldTransaction.Amount;
                    _categoryRepository.UpdateCategory(oldCategory);
                }
            }

            // 2. Apply New
            Guid accountId = oldCategory?.AccountID ?? Guid.Empty;

            if (transaction.Type == _incomeTypeId)
            {
                var category = _categoryRepository.GetCategoryById(transaction.CategoryID);
                if (category == null && accountId != Guid.Empty)
                {
                    var unassigned = _categoryRepository.GetUnassignedCategory(accountId);
                    transaction.CategoryID = unassigned.CategoryID;
                }
            }

            _transactionRepository.UpdateTransaction(transaction);

            // Update Balance/Category for NEW transaction
            var newCategory = _categoryRepository.GetCategoryById(transaction.CategoryID);
            if (newCategory != null)
            {
                var account = _bankAccountRepository.GetBankAccountById(newCategory.AccountID);
                if (account != null)
                {
                    if (transaction.Type == _incomeTypeId)
                    {
                        account.CurrentBalance += transaction.Amount;
                    }
                    else
                    {
                        account.CurrentBalance -= transaction.Amount;
                    }
                    _bankAccountRepository.UpdateBankAccount(account);
                }

                if (transaction.Type == _expenseTypeId)
                {
                    newCategory.AmountUsed += transaction.Amount;
                    _categoryRepository.UpdateCategory(newCategory);
                }
            }
        }
    }
}
