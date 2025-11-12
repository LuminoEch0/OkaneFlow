using OkaneFlow.Helpers;
using OkaneFlow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.Services.Dashboard;

namespace OkaneFlow.Pages.Dashboard.Category
{
    public class deleteModel : PageModel
    {
        private readonly BankAccountService _accountService;

        public deleteModel(BankAccountService accountService)
        {
            _accountService = accountService;
        }
        public BankAccountModel? AccountDetails { get; set; }
        public IActionResult OnGet(Guid id)
        {
            var account = _accountService.GetAccountById(id);
            if (account == null)

            {
                return NotFound();
            }
            AccountDetails = account;
            return Page();
        }
        public IActionResult OnPost(Guid id)
        {
            var account = _accountService.GetAccountById(id);
            if (account == null)
            {
                return NotFound();
            }
            AccountDetails = account;
            _accountService.DeleteAccount(id);
            return RedirectToPage("/Dashboard/MainDashboard/Dashboard");
        }
    }
}

