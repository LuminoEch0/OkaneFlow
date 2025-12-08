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

        public IActionResult OnGet(Guid id)
        {
            var serviceModel = _transactionService.GetTransactionById(id);
            if (serviceModel == null)
            {
                return RedirectToPage("/Transactions/Transactions");
            }
            Transaction = TransactionMapper.ToViewModel(serviceModel);

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
                var category = _categoryService.GetCategoryById(Transaction.CategoryID);
                if (category != null)
                {
                    var categories = _categoryService.GetAllCategories(category.AccountID);
                    CategoryOptions = new SelectList(categories, "CategoryID", "CategoryName");
                }
                return Page();
            }

            var model = TransactionMapper.ToModel(Transaction);
            _transactionService.UpdateTransaction(model);

            var cat = _categoryService.GetCategoryById(model.CategoryID);
            return RedirectToPage("/Transactions/Transactions", new { accountId = cat?.AccountID });
        }
    }
}
