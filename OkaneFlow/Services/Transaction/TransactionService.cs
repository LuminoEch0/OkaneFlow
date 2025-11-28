using DataAccessLayer.Repositories;
using OkaneFlow.Mappers;
using OkaneFlow.Models;
using OkaneFlow.Helpers.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OkaneFlow.Services.Transaction
{
    public class TransactionService
    {
        private readonly TransactionRepository _transactionRepository;
        private readonly BankAccountRepository _bankAccountRepository;
        private readonly CategoryRepository _categoryRepository;

        public TransactionService(TransactionRepository transactionRepository, BankAccountRepository bankAccountRepository, CategoryRepository categoryRepository)
        {
            _transactionRepository = transactionRepository;
            _bankAccountRepository = bankAccountRepository;
            _categoryRepository = categoryRepository;
        }

        public List<TransactionModel> GetTransactionsByAccountId(Guid accountId)
        {
            var dtos = _transactionRepository.GetTransactionsByAccountId(accountId);
            return dtos.Select(TransactionMapper.ToModel).ToList();
        }

        public void CreateTransaction(TransactionModel transaction, Guid accountId)
        {
            // 1. Handle Unassigned Category for Income if not specified
            if (transaction.Type == TransactionType.Income)
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
                if (transaction.Type == TransactionType.Income)
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
            if (transaction.Type == TransactionType.Expense)
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
                    if (transaction.Type == TransactionType.Income)
                    {
                        account.CurrentBalance -= transaction.Amount;
                    }
                    else
                    {
                        account.CurrentBalance += transaction.Amount;
                    }
                    _bankAccountRepository.UpdateBankAccount(account);
                }

                if (transaction.Type == TransactionType.Expense)
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
                    if (oldTransaction.Type == TransactionType.Income)
                    {
                        account.CurrentBalance -= oldTransaction.Amount;
                    }
                    else
                    {
                        account.CurrentBalance += oldTransaction.Amount;
                    }
                    _bankAccountRepository.UpdateBankAccount(account);
                }

                if (oldTransaction.Type == TransactionType.Expense)
                {
                    oldCategory.AmountUsed -= oldTransaction.Amount;
                    _categoryRepository.UpdateCategory(oldCategory);
                }
            }

            // 2. Apply New
            Guid accountId = oldCategory?.AccountID ?? Guid.Empty;

            if (transaction.Type == TransactionType.Income)
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
                    if (transaction.Type == TransactionType.Income)
                    {
                        account.CurrentBalance += transaction.Amount;
                    }
                    else
                    {
                        account.CurrentBalance -= transaction.Amount;
                    }
                    _bankAccountRepository.UpdateBankAccount(account);
                }

                if (transaction.Type == TransactionType.Expense)
                {
                    newCategory.AmountUsed += transaction.Amount;
                    _categoryRepository.UpdateCategory(newCategory);
                }
            }
        }
    }
}
