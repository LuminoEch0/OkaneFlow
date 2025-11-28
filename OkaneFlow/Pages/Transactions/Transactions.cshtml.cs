using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.Models;
using OkaneFlow.Services.Transaction;
using System;
using System.Collections.Generic;

namespace OkaneFlow.Pages.Transactions
{
    public class TransactionsModel : PageModel
    {
        private readonly TransactionService _transactionService;

        public TransactionsModel(TransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public List<TransactionModel> Transactions { get; set; } = new List<TransactionModel>();

        [BindProperty(SupportsGet = true)]
        public Guid AccountId { get; set; } // We need AccountId context

        public IActionResult OnGet(Guid? accountId)
        {
            if (accountId.HasValue)
            {
                AccountId = accountId.Value;
                Transactions = _transactionService.GetTransactionsByAccountId(AccountId);
            }
            else
            {
                // If no account selected, maybe redirect or show empty?
                // For now, let's assume we need an account ID.
                // Or maybe we fetch ALL transactions for the user? 
                // The prompt says "make the transactions page to also look nicer".
                // Usually transactions are per account or global.
                // Given the repo `GetTransactionsByAccountId`, it seems per account.
                // But the `Transactions` page in the root might be global?
                // Let's assume it requires an ID for now, or I'll need to add `GetAllTransactions` to service.
                // Let's stick to AccountId for now.
            }
            return Page();
        }
    }
}