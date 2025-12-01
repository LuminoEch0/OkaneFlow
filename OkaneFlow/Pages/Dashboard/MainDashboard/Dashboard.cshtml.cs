using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.ViewModels;
using Service;
using OkaneFlow.Mappers;

namespace OkaneFlow.Pages.Dashboard.MainDashboard
{
    public class DashboardModel : PageModel
    {
        private readonly BankAccountService _accountService;
        public DashboardModel(BankAccountService accountService)
        {
            _accountService = accountService;
        }
        public List<BankAccountViewModel> BankAccounts { get; set; } = new List<BankAccountViewModel>();

        public void OnGet()
        {
            var accountModels = _accountService.GetAllBankAccounts();
            BankAccounts = BankAccountMapper.ToViewModelList(accountModels);
        }
    }
}
