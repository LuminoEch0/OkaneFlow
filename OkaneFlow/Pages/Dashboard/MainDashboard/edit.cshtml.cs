using OkaneFlow.Helpers;
using OkaneFlow.Models;
using OkaneFlow.Services.Dashboard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace OkaneFlow.Pages.Dashboard.MainDashboard
{
    public class editModel : PageModel
    {
        private readonly BankAccountService _accountService;
        public editModel(BankAccountService accountService)
        {
            _accountService = accountService;
        }
        [BindProperty]
        required
        public BankAccountModel AccountDetails { get; set; }

        [BindProperty]
        public decimal AmountToAdd { get; set; }

        public IActionResult OnGet(Guid id)
        {
            var account = _accountService.GetAccountById(id);
            if(account == null)
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
            account.AccountName = AccountDetails.AccountName;
            //Console.WriteLine($"Loaded account: {AccountDetails.AccountName}, {AccountDetails.CurrentBalance}");


            if (AmountToAdd != 0)
            {
                account.UpdateBalance(AmountToAdd);
            }
            _accountService.UpdateAccountDetails(account);
            return RedirectToPage("/Dashboard/MainDashboard/Dashboard");

        }
    }
}
