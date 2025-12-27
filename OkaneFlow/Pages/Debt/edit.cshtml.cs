using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.Mappers;
using OkaneFlow.ViewModels;
using Service.Interface;
using System;

namespace OkaneFlow.Pages.Debt
{
    public class editModel : PageModel
    {
        private readonly IDebtService _debtService;
        private readonly IBankAccountService _accountService;
        private readonly OkaneFlow.Helpers.ICurrentUserService _currentUser;

        public editModel(IDebtService debtService, IBankAccountService accountService, OkaneFlow.Helpers.ICurrentUserService currentUser)
        {
            _debtService = debtService;
            _accountService = accountService;
            _currentUser = currentUser;
        }

        [BindProperty(SupportsGet = true)]
        public Guid id { get; set; } // DebtID

        [BindProperty]
        public DebtVM Input { get; set; }

        public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Accounts { get; set; } = new();

        public IActionResult OnGet(Guid id)
        {
            var debt = _debtService.GetDebtById(id);
            if (debt == null) return NotFound();

            // Basic ownership check
            if (debt.UserID != _currentUser.UserGuid && debt.UserID != Guid.Empty)
            {
                // In a real app we'd return Forbid(), but verifying via UserGuid is safer.
                // For now assuming if UserID is set, it must match.
                if (debt.UserID != _currentUser.UserGuid) return Forbid();
            }

            Input = DebtMapper.ToViewModel(debt);
            LoadAccounts();

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                LoadAccounts();
                return Page();
            }

            // Ensure UserID is set to current user (taking ownership or maintaining it)
            Input.UserID = _currentUser.UserGuid;
            Input.DebtID = id;

            _debtService.UpdateDebt(DebtMapper.ToModel(Input));

            return RedirectToPage("/Debt/DebtPage");
        }

        private void LoadAccounts()
        {
            var userAccounts = _accountService.GetAccountsByUserId(_currentUser.UserGuid);
            Accounts = userAccounts.Select(a => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = a.AccountID.ToString(),
                Text = a.AccountName,
                Selected = Input?.AccountID == a.AccountID
            }).ToList();
        }
    }
}
