using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.Mappers;
using OkaneFlow.ViewModels;
using Service;
using Service.Interface;

namespace OkaneFlow.Pages.Dashboard.Category
{
    public class createModel : PageModel
    {
        private readonly ICategoryService _accountService;
        public createModel(ICategoryService accountService)
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
        public async Task<IActionResult> OnPostAsync()
        {
            var newAccount = new CategoryVM(id, InputName, AmountToAllocate, AmountUsed);

            CategoryMapper.ToModel(newAccount);
            await _accountService.CreateCategoryAsync(CategoryMapper.ToModel(newAccount));

            return RedirectToPage($"/Dashboard/Category/CategoryPage", new { id });
        }
    }
}

