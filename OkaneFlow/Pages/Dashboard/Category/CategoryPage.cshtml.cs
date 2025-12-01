using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.ViewModels;
using Service;
using Service.Models;
using OkaneFlow.Mappers;

namespace OkaneFlow.Pages.Dashboard.Category
{
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
        public List<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
        public BankAccountViewModel? Account { get; set; } = new BankAccountViewModel();
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
            var categoryModels = _categoryService.GetAllCategories(id);
            Categories = CategoryMapper.ToViewModelList(categoryModels);

            UnallocatedAmount = _categoryService.GetUnallocatedAmount(id, _accountService);
            var accountModel = _accountService.GetAccountById(id);

            if (accountModel == null)
            {
                Account = new BankAccountViewModel();
            }
            else
            { 
                Account = BankAccountMapper.ToViewModel(accountModel);
            }
                

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
