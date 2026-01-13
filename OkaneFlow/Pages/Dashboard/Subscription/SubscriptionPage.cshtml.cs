using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.Mappers;
using OkaneFlow.ViewModels;
using Service.Interface;
using System.Collections.Generic;

namespace OkaneFlow.Pages.Dashboard.Subscription
{
    [Authorize]
    public class SubscriptionPageModel : PageModel
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly IBankAccountService _accountService;

        public SubscriptionPageModel(ISubscriptionService subscriptionService, IBankAccountService accountService)
        {
            _subscriptionService = subscriptionService;
            _accountService = accountService;
        }

        [BindProperty(SupportsGet = true)]
        public Guid id { get; set; } // AccountID

        public List<SubscriptionVM> Subscriptions { get; set; } = new();
        public List<SubscriptionVM> UpcomingAlerts { get; set; } = new();
        public BankAccountVM? Account { get; set; } = new BankAccountVM();

        public async Task OnGetAsync(Guid id)
        {
            Subscriptions = SubscriptionMapper.ToViewModelList(await _subscriptionService.GetSubscriptionsByAccountAsync(id));
            UpcomingAlerts = SubscriptionMapper.ToViewModelList(await _subscriptionService.GetUpcomingInsufficientSubscriptionsAsync(id));

            var accountModel = await _accountService.GetAccountByIdAsync(id);
            if (accountModel != null)
            {
                Account = BankAccountMapper.ToViewModel(accountModel);
            }
        }
    }
}
