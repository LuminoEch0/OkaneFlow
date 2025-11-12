using OkaneFlow.Helpers;
using OkaneFlow.Models;
using OkaneFlow.Services.Dashboard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
namespace OkaneFlow.Pages.Dashboard.Category
{
    public class editModel : PageModel
    {
        private readonly CategoryService _accountService;
        public editModel(CategoryService accountService)
        {
            _accountService = accountService;
        }
        [BindProperty]
        required
        public CategoryModel CategoryDetails { get; set; }

        //[BindProperty]
        //public decimal AmountAllocated { get; set; }

        [BindProperty]
        public decimal AmountToAllocate { get; set; }

        public IActionResult OnGet(Guid id)
        {
            var account = _accountService.GetCategoryById(id);
            if(account == null)
            {
                return NotFound();
            }
            CategoryDetails = account;
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
            _accountService.UpdateCategoryDetails(account);
            return RedirectToPage("/Dashboard/MainDashboard/Dashboard");

        }
    }
}
