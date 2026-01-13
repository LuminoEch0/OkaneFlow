using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using OkaneFlow.Mappers;
using OkaneFlow.ViewModels;
using Service;
using Service.Interface;
using System;

namespace OkaneFlow.Pages.Transactions
{
    public class EditModel : PageModel
    {
        private readonly ITransactionService _transactionService;
        private readonly ICategoryService _categoryService;

        public EditModel(ITransactionService transactionService, ICategoryService categoryService)
        {
            _transactionService = transactionService;
            _categoryService = categoryService;
        }

        [BindProperty]
        public TransactionVM Transaction { get; set; }

        public SelectList CategoryOptions { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var serviceModel = await _transactionService.GetTransactionByIdAsync(id);
            if (serviceModel == null)
            {
                return RedirectToPage("/Transactions/Transactions");
            }
            Transaction = TransactionMapper.ToViewModel(serviceModel);

            var category = await _categoryService.GetCategoryByIdAsync(Transaction.CategoryID);
            if (category != null)
            {
                var categories = await _categoryService.GetAllCategoriesAsync(category.AccountID);
                CategoryOptions = new SelectList(categories, "CategoryID", "CategoryName");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var category = await _categoryService.GetCategoryByIdAsync(Transaction.CategoryID);
                if (category != null)
                {
                    var categories = await _categoryService.GetAllCategoriesAsync(category.AccountID);
                    CategoryOptions = new SelectList(categories, "CategoryID", "CategoryName");
                }
                return Page();
            }

            var model = TransactionMapper.ToModel(Transaction);
            await _transactionService.UpdateTransactionAsync(model);

            var cat = await _categoryService.GetCategoryByIdAsync(model.CategoryID);
            return RedirectToPage("/Transactions/Transactions", new { accountId = cat?.AccountID });
        }
    }
}
