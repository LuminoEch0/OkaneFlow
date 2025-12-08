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
            _categoryService = categoryService; // small temporary to satisfy tool; will be applied correctly by smart editor
        }

        [BindProperty]
        public TransactionVM Transaction { get; set; } = new TransactionVM();

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

            var serviceModel = TransactionMapper.ToModel(Transaction);
            _transactionService.CreateTransaction(serviceModel, AccountId);

            return RedirectToPage("/Transactions/Transactions", new { accountId = AccountId });
        }
    }
}
