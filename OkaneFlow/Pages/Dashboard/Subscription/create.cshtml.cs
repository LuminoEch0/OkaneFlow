using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using OkaneFlow.Mappers;
using OkaneFlow.ViewModels;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OkaneFlow.Pages.Dashboard.Subscription
{
    public class createModel : PageModel
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ICategoryService _categoryService;

        public createModel(ISubscriptionService subscriptionService, ICategoryService categoryService)
        {
            _subscriptionService = subscriptionService;
            _categoryService = categoryService;
        }

        [BindProperty(SupportsGet = true)]
        public Guid id { get; set; } // AccountID

        [BindProperty]
        public SubscriptionVM Input { get; set; } = new SubscriptionVM();

        public SelectList CategoryList { get; set; }

        public async Task OnGetAsync(Guid id)
        {
            var categories = await _categoryService.GetAllCategoriesAsync(id);
            // Default "Unallocated" shouldn't probably be selectable for Subscription? Or yes?
            // User said: "when ... they don't select the category ... a new category is created called subscriptions"
            // So we show all existing categories.
            CategoryList = new SelectList(categories.Select(c => new { c.CategoryID, c.CategoryName }), "CategoryID", "CategoryName");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Reload categories if invalid (though minimal validation here)
                var categories = await _categoryService.GetAllCategoriesAsync(id);
                CategoryList = new SelectList(categories.Select(c => new { c.CategoryID, c.CategoryName }), "CategoryID", "CategoryName");
                return Page();
            }

            Input.AccountID = id;
            // Input.CategoryID will be Guid.Empty if not selected (and value="")

            try
            {
                await _subscriptionService.CreateSubscriptionAsync(SubscriptionMapper.ToModel(Input));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                // Reload categories
                var categories = await _categoryService.GetAllCategoriesAsync(id);
                CategoryList = new SelectList(categories.Select(c => new { c.CategoryID, c.CategoryName }), "CategoryID", "CategoryName");
                return Page();
            }

            return RedirectToPage("/Dashboard/Subscription/SubscriptionPage", new { id });
        }
    }
}
