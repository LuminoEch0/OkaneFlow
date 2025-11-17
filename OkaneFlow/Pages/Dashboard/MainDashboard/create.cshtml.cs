using OkaneFlow.Helpers;
using OkaneFlow.Models;
using OkaneFlow.Services.Dashboard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.Mappers;

namespace OkaneFlow.Pages.Dashboard.MainDashboard
{
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
            var newAccount = new BankAccountModel(InputName, InitialBalance);

            var dto = BankAccountMapper.ToDTO(newAccount);
            _accountService.CreateAccount(newAccount);

            return RedirectToPage("/Dashboard/MainDashboard/Dashboard");
        }
    }
}