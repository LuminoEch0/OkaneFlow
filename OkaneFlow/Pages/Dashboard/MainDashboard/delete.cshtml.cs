using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.ViewModels;
using Service;
using OkaneFlow.Mappers;
using Service.Interface;

namespace OkaneFlow.Pages.Dashboard.MainDashboard
{
    [Authorize]
    public class deleteModel : PageModel
    {
        private readonly IBankAccountService _accountService;

        public deleteModel(IBankAccountService accountService)
        {
            _accountService = accountService;
        }
        public BankAccountVM? AccountDetails { get; set; }
        public IActionResult OnGet(Guid id)
        {
            var account = _accountService.GetAccountById(id);
            if (account == null)

            {
                return NotFound();
            }
            AccountDetails = BankAccountMapper.ToViewModel(account);
            return Page();
        }
        public IActionResult OnPost(Guid id)
        {
            var account = _accountService.GetAccountById(id);
            if (account == null)
            {
                return NotFound();
            }
            AccountDetails = BankAccountMapper.ToViewModel(account);
            _accountService.DeleteAccount(id);
            return RedirectToPage("/Dashboard/MainDashboard/Dashboard");
        }
    }
}

