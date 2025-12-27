using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.Helpers.Enums;
using OkaneFlow.Mappers;
using OkaneFlow.ViewModels;
using Service.Interface;
using Service.Models;
using System;

namespace OkaneFlow.Pages.Debt
{
    public class paymentModel : PageModel
    {
        private readonly IDebtService _debtService;
        private readonly ITransactionService _transactionService;
        private readonly IBankAccountService _accountService;
        private readonly ICategoryService _categoryService;

        public paymentModel(IDebtService debtService, ITransactionService transactionService, IBankAccountService accountService, ICategoryService categoryService)
        {
            _debtService = debtService;
            _transactionService = transactionService;
            _accountService = accountService;
            _categoryService = categoryService;
        }

        [BindProperty(SupportsGet = true)]
        public Guid id { get; set; } // DebtID

        [BindProperty]
        public decimal PaymentAmount { get; set; }

        [BindProperty]
        public bool CreateTransaction { get; set; } = true;

        public DebtVM Debt { get; set; }
        public string AccountName { get; set; }

        public IActionResult OnGet(Guid id)
        {
            var debt = _debtService.GetDebtById(id);
            if (debt == null) return NotFound();

            Debt = DebtMapper.ToViewModel(debt);

            if (Debt.AccountID.HasValue)
            {
                var account = _accountService.GetAccountById(Debt.AccountID.Value);
                AccountName = account?.AccountName ?? "Unknown Account";
                CreateTransaction = true;
            }
            else
            {
                CreateTransaction = false;
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            var debt = _debtService.GetDebtById(id);
            if (debt == null) return NotFound();

            if (PaymentAmount <= 0)
            {
                ModelState.AddModelError("PaymentAmount", "Amount must be positive.");
                Debt = DebtMapper.ToViewModel(debt);
                return Page();
            }

            debt.RemainingAmount -= PaymentAmount;
            if (debt.RemainingAmount < 0) debt.RemainingAmount = 0;

            _debtService.UpdateDebt(debt);

            if (CreateTransaction && debt.AccountID.HasValue)
            {
                var categories = _categoryService.GetAllCategories(debt.AccountID.Value);
                var category = categories.FirstOrDefault(c => c.CategoryName == "Unassigned") ?? categories.FirstOrDefault();

                var transaction = new TransactionModel
                {
                    TransactionID = Guid.NewGuid(),
                    CategoryID = category?.CategoryID ?? Guid.Empty,
                    Amount = PaymentAmount,
                    Description = $"Payment for Debt: {debt.Name}",
                    Date = DateTime.UtcNow,
                    Type = (int)debt.Type
                };

                _transactionService.CreateTransaction(transaction, debt.AccountID.Value);
            }

            return RedirectToPage("/Debt/DebtPage");
        }
    }
}
