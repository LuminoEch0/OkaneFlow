using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using OkaneFlow.Models;
using OkaneFlow.Services.Dashboard;
using OkaneFlow.Services.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OkaneFlow.Pages.Transactions
{
    public class CreateModel : PageModel
    {
        private readonly TransactionService _transactionService;
        private readonly CategoryService _categoryService;

        public CreateModel(TransactionService transactionService, CategoryService categoryService)
        {
            _transactionService = transactionService;
            _categoryService = categoryService;
        }

        [BindProperty]
        public TransactionModel Transaction { get; set; } = new TransactionModel();

        [BindProperty(SupportsGet = true)]
        public Guid AccountId { get; set; }

        public SelectList CategoryOptions { get; set; }

        public IActionResult OnGet(Guid accountId)
        {
            AccountId = accountId;
            var categories = _categoryService.GetAllCategories(accountId);
            CategoryOptions = new SelectList(categories, "CategoryID", "CategoryName");
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                var categories = _categoryService.GetAllCategories(AccountId);
                CategoryOptions = new SelectList(categories, "CategoryID", "CategoryName");
                return Page();
            }

            _transactionService.CreateTransaction(Transaction, AccountId);

            return RedirectToPage("/Transactions/Transactions", new { accountId = AccountId });
        }
    }
}
