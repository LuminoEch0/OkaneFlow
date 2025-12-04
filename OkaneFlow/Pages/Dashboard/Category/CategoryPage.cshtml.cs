using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.ViewModels;
using Service;
using Service.Models;
using OkaneFlow.Mappers;

namespace OkaneFlow.Pages.Dashboard.Category
{
    [Authorize]
    public class CategoryPageModel : PageModel
    {
        private readonly CategoryService _categoryService;
        private readonly BankAccountService _accountService;
        public CategoryPageModel(CategoryService categoryService, BankAccountService accountService)
        {
            _categoryService = categoryService;
            _accountService = accountService;
        }
        [BindProperty(SupportsGet = true)]
        public Guid id { get; set; }
        public List<CategoryModel> Categories { get; set; } = new List<CategoryModel>();
        public BankAccountModel? Account { get; set; } = new BankAccountModel();
        public decimal UnallocatedAmount { get; set; }

        [BindProperty]
        public decimal AmountToAllocate { get; set; }
        [BindProperty]
        public Guid AssignCategoryId { get; set; }
        public string UnallocatedFontColor =>
            UnallocatedAmount < 0 ? "red"
            : UnallocatedAmount == 0 ? "black"
            : "green";

        public void OnGet(Guid id)
        {
            Categories = _categoryService.GetAllCategories(id);
            UnallocatedAmount = _categoryService.GetUnallocatedAmount(id, _accountService);
            if (Account == null)
            {
                Account = new BankAccountModel();
            }
            Account = _accountService.GetAccountById(id);

        }

        public IActionResult OnPostAssign()
        {
            if (AmountToAllocate > 0 && AssignCategoryId != Guid.Empty)
            {
                _categoryService.AssignAmountToAllocate(AssignCategoryId, AmountToAllocate);
            }
            return RedirectToPage(new { id });
        }
    }

}
