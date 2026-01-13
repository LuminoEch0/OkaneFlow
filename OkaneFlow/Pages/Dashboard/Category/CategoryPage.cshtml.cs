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
        private readonly ICurrentUserService _currentUser;
        public CategoryPageModel(ICategoryService categoryService, IBankAccountService accountService, ICurrentUserService currentUser)
        {
            _categoryService = categoryService;
            _accountService = accountService;
            _currentUser = currentUser;
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

        public async Task OnGetAsync(Guid id)
        {
            Categories = CategoryMapper.ToViewModelList(await _categoryService.GetAllCategoriesAsync(id));
            var item = Categories.FirstOrDefault(x => x.CategoryName == "Unallocated");
            if(item != null)
            {
                Categories.Remove(item);
                Categories.Add(item);
            }

            UnallocatedAmount = await _categoryService.GetUnallocatedAmountAsync(id, _accountService);

            var accountModel = await _accountService.GetAccountByIdAsync(id);
            if (accountModel == null)
            {
                Account = new BankAccountVM();
            }
            else
            {
                Account = BankAccountMapper.ToViewModel(accountModel);
            }


        }

        public async Task<IActionResult> OnPostAssignAsync()
        {
            if (AmountToAllocate > 0 && AssignCategoryId != Guid.Empty)
            {
                await _categoryService.AssignAmountToAllocateAsync(AssignCategoryId, AmountToAllocate);
            }
            return RedirectToPage(new { id });
        }
    }

}
