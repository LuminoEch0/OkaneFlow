using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OkaneFlow.Pages.Transactions
{
    public class TransactionsModel : PageModel
    {
        public IActionResult OnGet()
        {
            return Page();
        }
    }
}