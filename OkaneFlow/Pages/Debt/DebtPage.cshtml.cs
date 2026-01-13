using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.Mappers;
using OkaneFlow.ViewModels;
using Service.Interface;
using System.Collections.Generic;

namespace OkaneFlow.Pages.Debt
{
    [Authorize]
    public class DebtPageModel : PageModel
    {
        private readonly IDebtService _debtService;
        private readonly IBankAccountService _accountService;
        private readonly ICurrentUserService _currentUser;

        public DebtPageModel(IDebtService debtService, IBankAccountService accountService, ICurrentUserService currentUser)
        {
            _debtService = debtService;
            _accountService = accountService;
            _currentUser = currentUser;
        }

        public List<DebtVM> Debts { get; set; } = new();

        public async Task OnGetAsync()
        {
            Debts = DebtMapper.ToViewModelList(await _debtService.GetDebtsByUserAsync(_currentUser.UserGuid));
        }
    }
}
