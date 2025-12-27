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

        public IActionResult OnGet(Guid id)
        {
            var sub = _subscriptionService.GetSubscriptionById(id);
            if (sub == null) return NotFound();
            Subscription = SubscriptionMapper.ToViewModel(sub);
            return Page();
        }

        public IActionResult OnPost()
        {
            var sub = _subscriptionService.GetSubscriptionById(id);
            if (sub == null) return NotFound();
            
            _subscriptionService.DeleteSubscription(id);

            return RedirectToPage("/Dashboard/Subscription/SubscriptionPage", new { id = sub.AccountID });
        }
    }
}
