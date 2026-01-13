using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using OkaneFlow.ViewModels;
using OkaneFlow.Mappers;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using Service.Interface;

namespace OkaneFlow.Pages.Transactions
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ITransactionService _transactionService;
        private readonly ICategoryService _categoryService;

        public CreateModel(ITransactionService transactionService, ICategoryService categoryService)
        {
            _transactionService = transactionService;
            _categoryService = categoryService;
        }

        [BindProperty]
        public TransactionVM Transaction { get; set; } = new TransactionVM();

        [BindProperty(SupportsGet = true)]
        public Guid AccountId { get; set; }

        public SelectList CategoryOptions { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid accountId)
        {
            AccountId = accountId;
            var categories = await _categoryService.GetAllCategoriesAsync(accountId);
            CategoryOptions = new SelectList(categories, "CategoryID", "CategoryName");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryService.GetAllCategoriesAsync(AccountId);
                CategoryOptions = new SelectList(categories, "CategoryID", "CategoryName");
                return Page();
            }

            var serviceModel = TransactionMapper.ToModel(Transaction);
            await _transactionService.CreateTransactionAsync(serviceModel, AccountId);

            return RedirectToPage("/Transactions/Transactions", new { accountId = AccountId });
        }
    }
}
