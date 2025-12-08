using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.ViewModels;
using Service;
using OkaneFlow.Mappers;
using Service.Interface;


namespace OkaneFlow.Pages.Dashboard.Category
{
    public class editModel : PageModel
    {
        private readonly ICategoryService _accountService;
        public editModel(ICategoryService accountService)
        {
            _accountService = accountService;
        }
        [BindProperty]
        required
        public CategoryVM CategoryDetails
        { get; set; }

        //[BindProperty]
        //public decimal AmountAllocated { get; set; }

        [BindProperty]
        public decimal AmountToAllocate { get; set; }

        [BindProperty(SupportsGet = true)]
        public Guid id { get; set; }

        public IActionResult OnGet(Guid id)
        {
            var account = _accountService.GetCategoryById(id);
            if (account == null)
            {
                return NotFound();
            }
            CategoryDetails = CategoryMapper.ToViewModel(account);
            return Page();
        }

        public IActionResult OnPost(Guid id)
        {
            var account = _accountService.GetCategoryById(id);
            if (account == null)
            {
                return NotFound();
            }
            account.CategoryName = CategoryDetails.CategoryName;
            //Console.WriteLine($"Loaded account: {AccountDetails.AccountName}, {AccountDetails.CurrentBalance}");


            if (AmountToAllocate != 0)
            {
                account.UpdateAllocatedAmount(AmountToAllocate);
            }
            var identity = account.AccountID;
            _accountService.UpdateCategoryDetails(account);
            return RedirectToPage("/Dashboard/Category/CategoryPage", new { id = identity });

        }
    }
}
