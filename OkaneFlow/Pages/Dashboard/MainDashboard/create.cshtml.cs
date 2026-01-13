using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.Helpers;
using OkaneFlow.Mappers;
using OkaneFlow.ViewModels;
using Service;
using Service.Interface;

namespace OkaneFlow.Pages.Dashboard.MainDashboard
{
    [Authorize]
    public class createModel : PageModel
    {
        private readonly IBankAccountService _accountService;
        private readonly ICurrentUserService _currentUser;

        public createModel(IBankAccountService accountService, ICurrentUserService currentUser)
        {
            _accountService = accountService;
            _currentUser = currentUser;
        }

        [BindProperty]
        public string InputName { get; set; } = string.Empty;

        [BindProperty]
        public decimal InitialBalance { get; set; } = 0;

        public IActionResult OnGet()
        {
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var newAccount = new BankAccountVM(_currentUser.UserGuid, InputName, InitialBalance);

            var dto = BankAccountMapper.ToModel(newAccount);
            await _accountService.CreateAccountAsync(dto);

            return RedirectToPage("/Dashboard/MainDashboard/Dashboard");
        }
    }
}