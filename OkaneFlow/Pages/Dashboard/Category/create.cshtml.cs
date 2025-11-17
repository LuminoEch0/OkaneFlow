using OkaneFlow.Helpers;
using OkaneFlow.Models;
using OkaneFlow.Services.Dashboard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.Mappers;

namespace OkaneFlow.Pages.Dashboard.Category
{
    public class createModel : PageModel
    {
        private readonly CategoryService _accountService;
        public createModel(CategoryService accountService)
        {
            _accountService = accountService;
        }

        [BindProperty(SupportsGet =true)]
        public Guid id { get; set; }

        [BindProperty]
        public string InputName { get; set; } = string.Empty;

        [BindProperty]
        public decimal AmountToAllocate { get; set; } = 0;
        private decimal AmountUsed = 0;

        public IActionResult OnGet()
        {
            return Page();
        }
        public IActionResult OnPost()
        {
            var newAccount = new CategoryModel(id, InputName, AmountToAllocate, AmountUsed);

            CategoryMapper.ToDTO(newAccount);
            _accountService.CreateCategory(newAccount);

            return RedirectToPage($"/Dashboard/Category/CategoryPage", new { id });
        }
    }
}

