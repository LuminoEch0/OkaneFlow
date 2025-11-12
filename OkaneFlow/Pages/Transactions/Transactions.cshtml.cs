using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

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