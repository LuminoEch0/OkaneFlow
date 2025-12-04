using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.Mappers;
using OkaneFlow.ViewModels;
using Service;

namespace OkaneFlow.Pages.Dashboard.MainDashboard
{
    [Authorize]
    public class createModel : PageModel
    {
        private readonly BankAccountService _accountService;
        public createModel(BankAccountService accountService)
        {
            _accountService = accountService;
        }

        [BindProperty]
        public string InputName { get; set; } = string.Empty;

        [BindProperty]
        public decimal InitialBalance { get; set; } = 0;

        public IActionResult OnGet()
        {
            return Page();
        }
        public IActionResult OnPost()
        {
            var newAccount = new BankAccountVM(InputName, InitialBalance);

            var dto = BankAccountMapper.ToModel(newAccount);
            _accountService.CreateAccount(dto);

            return RedirectToPage("/Dashboard/MainDashboard/Dashboard");
        }
    }
}