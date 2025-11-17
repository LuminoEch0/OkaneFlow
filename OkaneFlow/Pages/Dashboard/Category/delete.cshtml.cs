using OkaneFlow.Helpers;
using OkaneFlow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.Services.Dashboard;

namespace OkaneFlow.Pages.Dashboard.Category
{
    public class deleteModel : PageModel
    {
        private readonly CategoryService _accountService;

        public deleteModel(CategoryService accountService)
        {
            _accountService = accountService;
        }
        public CategoryModel? AccountDetails { get; set; }
        public IActionResult OnGet(Guid id)
        {
            var account = _accountService.GetCategoryById(id);
            if (account == null)

            {
                return NotFound();
            }
            AccountDetails = account;
            return Page();
        }
        public IActionResult OnPost(Guid id)
        {
            var account = _accountService.GetCategoryById(id);
            if (account == null)
            {
                return NotFound();
            }
            var identity = account.AccountID;
            _accountService.DeleteCategory(id);
            return RedirectToPage($"/Dashboard/Category/CategoryPage", new { id = identity });
        }
    }
}

