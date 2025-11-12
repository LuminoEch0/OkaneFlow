using OkaneFlow.Helpers;
using OkaneFlow.Models;
using OkaneFlow.Services.Dashboard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OkaneFlow.Pages.Dashboard.MainDashboard
{
    public class DashboardModel : PageModel
    {
        private readonly BankAccountService _accountService;
        public DashboardModel(BankAccountService accountService)
        {
            _accountService = accountService;
        }
        public List<BankAccountModel> BankAccounts { get; set; } = new List<BankAccountModel>();

        public void OnGet()
        {
            BankAccounts = _accountService.GetAllBankAccounts();
        }
    }
}
