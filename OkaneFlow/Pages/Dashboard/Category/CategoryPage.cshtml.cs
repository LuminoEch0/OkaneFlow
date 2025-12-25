using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.ViewModels;
using Service;
using OkaneFlow.Mappers;
using Service.Interface;


namespace OkaneFlow.Pages.Dashboard.Category
{
    [Authorize]
    public class CategoryPageModel : PageModel
    {
        private readonly ICategoryService _categoryService;
        private readonly IBankAccountService _accountService;
        public CategoryPageModel(ICategoryService categoryService, IBankAccountService accountService)
        {
            _categoryService = categoryService;
            _accountService = accountService;
        }
        [BindProperty(SupportsGet = true)]
        public Guid id { get; set; }
        public List<CategoryVM> Categories { get; set; } = new();
        public BankAccountVM? Account { get; set; } = new BankAccountVM();
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
            Categories = CategoryMapper.ToViewModelList(_categoryService.GetAllCategories(id));
            UnallocatedAmount = _categoryService.GetUnallocatedAmount(id, _accountService);
            var accountModel = _accountService.GetAccountById(id);
            if (accountModel == null)
            {
                Account = new BankAccountVM();
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
