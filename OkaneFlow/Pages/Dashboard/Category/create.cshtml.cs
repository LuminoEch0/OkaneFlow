using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.Mappers;
using OkaneFlow.ViewModels;
using Service;

namespace OkaneFlow.Pages.Dashboard.Category
{
    public class createModel : PageModel
    {
        private readonly CategoryService _accountService;
        public createModel(CategoryService accountService)
        {
            _accountService = accountService;
        }

        [BindProperty(SupportsGet = true)]
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
            var newAccount = new CategoryViewModel(id, InputName, AmountToAllocate, AmountUsed);

            CategoryMapper.ToModel(newAccount);
            _accountService.CreateCategory(CategoryMapper.ToModel(newAccount));

            return RedirectToPage($"/Dashboard/Category/CategoryPage", new { id });
        }
    }
}

