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
        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account == null)

            {
                return NotFound();
            }
            AccountDetails = BankAccountMapper.ToViewModel(account);
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            AccountDetails = BankAccountMapper.ToViewModel(account);
            await _accountService.DeleteAccountAsync(id);
            return RedirectToPage("/Dashboard/MainDashboard/Dashboard");
        }
    }
}

