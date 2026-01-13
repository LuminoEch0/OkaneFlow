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
        private readonly ICurrentUserService _currentUser;

        public editModel(IDebtService debtService, IBankAccountService accountService, ICurrentUserService currentUser)
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

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var debt = await _debtService.GetDebtByIdAsync(id);
            if (debt == null) return NotFound();

            // Basic ownership check
            if (debt.UserID != _currentUser.UserGuid && debt.UserID != Guid.Empty)
            {
                // In a real app we'd return Forbid(), but verifying via UserGuid is safer.
                // For now assuming if UserID is set, it must match.
                if (debt.UserID != _currentUser.UserGuid) return Forbid();
            }

            Input = DebtMapper.ToViewModel(debt);
            await LoadAccountsAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadAccountsAsync();
                return Page();
            }

            // Ensure UserID is set to current user (taking ownership or maintaining it)
            Input.UserID = _currentUser.UserGuid;
            Input.DebtID = id;

            await _debtService.UpdateDebtAsync(DebtMapper.ToModel(Input));

            return RedirectToPage("/Debt/DebtPage");
        }

        private async Task LoadAccountsAsync()
        {
            var userAccounts = await _accountService.GetAccountsByUserIdAsync(_currentUser.UserGuid);
            Accounts = userAccounts.Select(a => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = a.AccountID.ToString(),
                Text = a.AccountName,
                Selected = Input?.AccountID == a.AccountID
            }).ToList();
        }
    }
}
