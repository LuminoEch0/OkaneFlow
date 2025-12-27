using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using OkaneFlow.Mappers;
using OkaneFlow.ViewModels;
using Service.Interface;
using System;
using System.Linq;

namespace OkaneFlow.Pages.Dashboard.Subscription
{
    public class editModel : PageModel
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ICategoryService _categoryService;

        public editModel(ISubscriptionService subscriptionService, ICategoryService categoryService)
        {
            _subscriptionService = subscriptionService;
            _categoryService = categoryService;
        }

        [BindProperty(SupportsGet = true)]
        public Guid id { get; set; } // SubscriptionID

        [BindProperty]
        public SubscriptionVM Input { get; set; }

        public SelectList CategoryList { get; set; }

        public IActionResult OnGet(Guid id)
        {
            var sub = _subscriptionService.GetSubscriptionById(id);
            if (sub == null) return NotFound();

            Input = SubscriptionMapper.ToViewModel(sub);

            var categories = _categoryService.GetAllCategories(Input.AccountID);
            CategoryList = new SelectList(categories.Select(c => new { c.CategoryID, c.CategoryName }), "CategoryID", "CategoryName", Input.CategoryID);

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                 var categories = _categoryService.GetAllCategories(Input.AccountID);
                 CategoryList = new SelectList(categories.Select(c => new { c.CategoryID, c.CategoryName }), "CategoryID", "CategoryName", Input.CategoryID);
                 return Page();
            }

            Input.SubscriptionID = id;
            var model = SubscriptionMapper.ToModel(Input);
            
            _subscriptionService.UpdateSubscription(model);

            return RedirectToPage("/Dashboard/Subscription/SubscriptionPage", new { id = Input.AccountID });
        }
    }
}
