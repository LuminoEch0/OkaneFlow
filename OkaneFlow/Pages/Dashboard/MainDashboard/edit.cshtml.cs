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
    public class editModel : PageModel
    {
        private readonly IBankAccountService _accountService;
        public editModel(IBankAccountService accountService)
        {
            _accountService = accountService;
        }
        [BindProperty]
        required
        public BankAccountVM AccountDetails
        { get; set; }

        [BindProperty]
        public decimal AmountToAdd { get; set; }

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
            account.AccountName = AccountDetails.AccountName;
            //Console.WriteLine($"Loaded account: {AccountDetails.AccountName}, {AccountDetails.CurrentBalance}");


            if (AmountToAdd != 0)
            {
                account.UpdateBalance(AmountToAdd);
            }
            await _accountService.UpdateAccountDetailsAsync(account);
            return RedirectToPage("/Dashboard/MainDashboard/Dashboard");

        }
    }
}
