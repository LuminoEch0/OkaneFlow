using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.Helpers;
using OkaneFlow.Mappers;
using OkaneFlow.ViewModels;
using Service;
using Service.Interface;

namespace OkaneFlow.Pages.Dashboard.MainDashboard
{
    [Authorize]
    public class DashboardModel : PageModel
    {
        private readonly IBankAccountService _accountService;
        private readonly ICurrentUserService _currentUser;
        
        public DashboardModel(IBankAccountService accountService, ICurrentUserService currentUser)
        {
            _accountService = accountService;
            _currentUser = currentUser;
        }
        public Guid AccountGuid { get; private set; }
        public List<BankAccountVM> BankAccounts { get; set; } = new List<BankAccountVM>();

        public void OnGet()
        {
            AccountGuid = _currentUser.UserGuid;
            var accountModels = _accountService.GetAllBankAccounts(_currentUser.UserGuid);
            BankAccounts = BankAccountMapper.ToViewModelList(accountModels);
        }
    }
}
