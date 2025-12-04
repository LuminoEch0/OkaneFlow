using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.ViewModels;
using OkaneFlow.Mappers;
using Service;
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

        public List<TransactionVM> Transactions { get; set; } = new List<TransactionVM>();

        [BindProperty(SupportsGet = true)]
        public Guid AccountId { get; set; } // We need AccountId context

        public IActionResult OnGet(Guid? accountId)
        {
            if (accountId.HasValue)
            {
                AccountId = accountId.Value;
                var serviceModels = _transactionService.GetTransactionsByAccountId(AccountId);
                Transactions = TransactionMapper.ToViewModelList(serviceModels);
            }
            else
            {
                // No account selected - show empty
            }
            return Page();
        }
    }
}