using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.Mappers;
using OkaneFlow.ViewModels;
using Service.Interface;
using System;

namespace OkaneFlow.Pages.Dashboard.Subscription
{
    public class deleteModel : PageModel
    {
        private readonly ISubscriptionService _subscriptionService;

        public deleteModel(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [BindProperty(SupportsGet = true)]
        public Guid id { get; set; } // SubscriptionID

        public SubscriptionVM Subscription { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var sub = await _subscriptionService.GetSubscriptionByIdAsync(id);
            if (sub == null) return NotFound();
            Subscription = SubscriptionMapper.ToViewModel(sub);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var sub = await _subscriptionService.GetSubscriptionByIdAsync(id);
            if (sub == null) return NotFound();

            await _subscriptionService.DeleteSubscriptionAsync(id);

            return RedirectToPage("/Dashboard/Subscription/SubscriptionPage", new { id = sub.AccountID });
        }
    }
}
