using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.Mappers;
using OkaneFlow.ViewModels;
using Service.Interface;
using System;

namespace OkaneFlow.Pages.Debt
{
    public class createModel : PageModel
    {
        private readonly IDebtService _debtService;
        private readonly IBankAccountService _accountService;
        private readonly OkaneFlow.Helpers.ICurrentUserService _currentUser;

        public createModel(IDebtService debtService, IBankAccountService accountService, OkaneFlow.Helpers.ICurrentUserService currentUser)
        {
            _debtService = debtService;
            _accountService = accountService;
            _currentUser = currentUser;
        }



        [BindProperty(SupportsGet = true)]
        public int? type { get; set; } // OwedByMe or OwedToMe

        [BindProperty]
        public DebtVM Input { get; set; } = new DebtVM();

        public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Accounts { get; set; } = new();

        public void OnGet(int? type)
        {
            var userAccounts = _accountService.GetAccountsByUserId(_currentUser.UserGuid);
            Accounts = userAccounts.Select(a => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = a.AccountID.ToString(),
                Text = a.AccountName
            }).ToList();

            if (type.HasValue)
            {
                Input.Type = (OkaneFlow.Helpers.Enums.DebtType)type.Value;
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                // Republish accounts if failed
                var userAccounts = _accountService.GetAccountsByUserId(_currentUser.UserGuid);
                Accounts = userAccounts.Select(a => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = a.AccountID.ToString(),
                    Text = a.AccountName
                }).ToList();
                return Page();
            }

            Input.UserID = _currentUser.UserGuid;

            // Should I force AccountID to null if some checkbox says "standalone"? 
            // For now, if Input.AccountID is null (not selected), it works as standalone.

            _debtService.CreateDebt(DebtMapper.ToModel(Input));

            return RedirectToPage("/Debt/DebtPage");
        }
    }
}
