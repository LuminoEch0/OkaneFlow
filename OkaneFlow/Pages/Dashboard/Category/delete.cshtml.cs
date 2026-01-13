using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.ViewModels;
using Service;
using OkaneFlow.Mappers;
using Service.Interface;

namespace OkaneFlow.Pages.Dashboard.Category
{
    public class deleteModel : PageModel
    {
        private readonly ICategoryService _accountService;

        public deleteModel(ICategoryService accountService)
        {
            _accountService = accountService;
        }
        public CategoryVM? AccountDetails { get; set; }
        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var account = await _accountService.GetCategoryByIdAsync(id);
            if (account == null)

            {
                return NotFound();
            }

            AccountDetails = CategoryMapper.ToViewModel(account);
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            var account = await _accountService.GetCategoryByIdAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            var identity = account.AccountID;
            try
            {
                await _accountService.DeleteCategoryAsync(id);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                AccountDetails = CategoryMapper.ToViewModel(account);
                return Page();
            }
            return RedirectToPage($"/Dashboard/Category/CategoryPage", new { id = identity });
        }
    }
}

