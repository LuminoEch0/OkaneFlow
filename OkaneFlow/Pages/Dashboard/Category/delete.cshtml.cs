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
        public IActionResult OnGet(Guid id)
        {
            var account = _accountService.GetCategoryById(id);
            if (account == null)

            {
                return NotFound();
            }
            
            AccountDetails = CategoryMapper.ToViewModel(account);
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

