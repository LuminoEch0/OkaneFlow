using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.Mappers;
using OkaneFlow.ViewModels;
using Service;
using Service.Interface;
using System;

namespace OkaneFlow.Pages.Transactions
{
    public class DeleteModel : PageModel
    {
        private readonly ITransactionService _transaction_service;
        private readonly ICategoryService _category_service;

        public DeleteModel(ITransactionService transactionService, ICategoryService categoryService)
        {
            _transaction_service = transactionService;
            _category_service = categoryService;
        }

        [BindProperty]
        public TransactionVM Transaction { get; set; }

        public IActionResult OnGet(Guid id)
        {
            var model = _transaction_service.GetTransactionById(id);
            if (model == null)
            {
                return RedirectToPage("/Transactions/Transactions");
            }
            Transaction = TransactionMapper.ToViewModel(model);
            return Page();
        }

        public IActionResult OnPost(Guid id)
        {
            var transaction = _transaction_service.GetTransactionById(id);
            if (transaction == null) return RedirectToPage("/Transactions/Transactions");

            var category = _category_service.GetCategoryById(transaction.CategoryID);
            var accountId = category?.AccountID;

            _transaction_service.DeleteTransaction(id);

            return RedirectToPage("/Transactions/Transactions", new { accountId = accountId });
        }
    }
}
