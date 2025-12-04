using DataAccessLayer.Repositories;
using Service.Mappers;
using Service.Models;

namespace Service
{
    public class TransactionService
    {
        private readonly TransactionRepo _transactionRepository;
        private readonly BankAccountRepo _bankAccountRepository;
        private readonly CategoryRepo _categoryRepository;
        private readonly TransactionTypeLookupService _transactionTypeLookupService;

        private readonly int _expenseTypeId;
        private readonly int _incomeTypeId;

        public TransactionService(TransactionRepo transactionRepository, BankAccountRepo bankAccountRepository, CategoryRepo categoryRepository, TransactionTypeLookupService transactionTypeLookupService)
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
            var dtos = _transactionRepository.GetTransactionsByAccountId(accountId);
            return dtos.Select(TransactionMapper.ToModel).ToList();
        }

        public void CreateTransaction(TransactionModel transaction, Guid accountId)
        {
            // 1. Handle Unassigned Category for Income if not specified
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
            _transactionRepository.AddTransaction(TransactionMapper.ToDTO(transaction));

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
            var transactionDto = _transactionRepository.GetTransactionById(id);
            if (transactionDto == null) return;

            var transaction = TransactionMapper.ToModel(transactionDto);

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
            var dto = _transactionRepository.GetTransactionById(id);
            return dto == null ? null : TransactionMapper.ToModel(dto);
        }

        public void UpdateTransaction(TransactionModel transaction)
        {
            var oldTransactionDto = _transactionRepository.GetTransactionById(transaction.TransactionID);
            if (oldTransactionDto == null) return;
            var oldTransaction = TransactionMapper.ToModel(oldTransactionDto);

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

            _transactionRepository.UpdateTransaction(TransactionMapper.ToDTO(transaction));

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
