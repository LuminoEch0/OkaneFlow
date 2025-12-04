using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.Helpers;
using OkaneFlow.Mappers;
using OkaneFlow.ViewModels;
using Service;

namespace OkaneFlow.Pages.Dashboard.MainDashboard
{
    [Authorize]
    public class DashboardModel : PageModel
    {
        private readonly BankAccountService _accountService;
        private readonly CurrentUserService _currentUser;
        
        public DashboardModel(BankAccountService accountService, CurrentUserService currentUser)
        {
            _accountService = accountService;
            _currentUser = currentUser;
        }
        public List<BankAccountVM> BankAccounts { get; set; } = new List<BankAccountVM>();

        public void OnGet()
        {
            var accountModels = _accountService.GetAllBankAccounts(_currentUser.UserGuid);
            BankAccounts = BankAccountMapper.ToViewModelList(accountModels);
        }
    }
}
