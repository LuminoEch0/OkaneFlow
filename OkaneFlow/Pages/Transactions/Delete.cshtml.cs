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

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var model = await _transaction_service.GetTransactionByIdAsync(id);
            if (model == null)
            {
                return RedirectToPage("/Transactions/Transactions");
            }
            Transaction = TransactionMapper.ToViewModel(model);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            var transaction = await _transaction_service.GetTransactionByIdAsync(id);
            if (transaction == null) return RedirectToPage("/Transactions/Transactions");

            var category = await _category_service.GetCategoryByIdAsync(transaction.CategoryID);
            var accountId = category?.AccountID;

            await _transaction_service.DeleteTransactionAsync(id);

            return RedirectToPage("/Transactions/Transactions", new { accountId = accountId });
        }
    }
}
