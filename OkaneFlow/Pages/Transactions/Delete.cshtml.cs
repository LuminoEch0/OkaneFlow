using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.Models;
using OkaneFlow.Services.Dashboard;
using OkaneFlow.Services.Transaction;
using System;

namespace OkaneFlow.Pages.Transactions
{
    public class DeleteModel : PageModel
    {
        private readonly TransactionService _transactionService;
        private readonly CategoryService _categoryService;

        public DeleteModel(TransactionService transactionService, CategoryService categoryService)
        {
            _transactionService = transactionService;
            _categoryService = categoryService;
        }

        [BindProperty]
        public TransactionModel Transaction { get; set; }

        public IActionResult OnGet(Guid id)
        {
            Transaction = _transactionService.GetTransactionById(id);
            if (Transaction == null)
            {
                return RedirectToPage("/Transactions/Transactions");
            }
            return Page();
        }

        public IActionResult OnPost(Guid id)
        {
            var transaction = _transactionService.GetTransactionById(id);
            if (transaction == null) return RedirectToPage("/Transactions/Transactions");

            // Get AccountID for redirect
            var category = _categoryService.GetCategoryById(transaction.CategoryID);
            var accountId = category?.AccountID;

            _transactionService.DeleteTransaction(id);

            return RedirectToPage("/Transactions/Transactions", new { accountId = accountId });
        }
    }
}
