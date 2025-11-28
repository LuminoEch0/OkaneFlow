using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using OkaneFlow.Models;
using OkaneFlow.Services.Dashboard;
using OkaneFlow.Services.Transaction;
using System;

namespace OkaneFlow.Pages.Transactions
{
    public class EditModel : PageModel
    {
        private readonly TransactionService _transactionService;
        private readonly CategoryService _categoryService;

        public EditModel(TransactionService transactionService, CategoryService categoryService)
        {
            _transactionService = transactionService;
            _categoryService = categoryService;
        }

        [BindProperty]
        public TransactionModel Transaction { get; set; }

        public SelectList CategoryOptions { get; set; }

        public IActionResult OnGet(Guid id)
        {
            Transaction = _transactionService.GetTransactionById(id);
            if (Transaction == null)
            {
                return RedirectToPage("/Transactions/Transactions");
            }

            // We need AccountID to fetch categories. 
            // Transaction doesn't have AccountID directly, but we can get it from the Category.
            // Or we can assume the user context.
            // Let's fetch the category of the transaction to find the AccountID.
            // Actually, `GetTransactionById` returns a model which has `CategoryID`.
            // We can use `CategoryService` to get the category and then the AccountID.

            var category = _categoryService.GetCategoryById(Transaction.CategoryID);
            if (category != null)
            {
                var categories = _categoryService.GetAllCategories(category.AccountID);
                CategoryOptions = new SelectList(categories, "CategoryID", "CategoryName");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                // Refetch options
                var category = _categoryService.GetCategoryById(Transaction.CategoryID);
                if (category != null)
                {
                    var categories = _categoryService.GetAllCategories(category.AccountID);
                    CategoryOptions = new SelectList(categories, "CategoryID", "CategoryName");
                }
                return Page();
            }

            _transactionService.UpdateTransaction(Transaction);

            // Redirect back to transactions list. We need AccountID.
            // We can get it from the category again.
            var cat = _categoryService.GetCategoryById(Transaction.CategoryID);
            return RedirectToPage("/Transactions/Transactions", new { accountId = cat?.AccountID });
        }
    }
}
