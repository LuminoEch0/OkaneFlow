using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OkaneFlow.Pages
{
    public class RedirectModel : PageModel
    {
        public RedirectToPageResult OnGet()
        {
            // This page is intentionally left blank.
            // It serves as a secure redirection endpoint after login.
            return User.IsInRole("Admin")
                ? RedirectToPage("/Admin/Dashboard")
                : RedirectToPage("/Dashboard/MainDashboard/Dashboard");
        }
    }
}
