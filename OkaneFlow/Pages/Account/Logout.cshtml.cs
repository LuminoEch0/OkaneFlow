using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OkaneFlow.Pages.Account
{
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public class LogoutModel : PageModel
    {

        // Handle GET requests (e.g., user clicks a standard <a href="/Account/Logout"> link)
        public async Task<IActionResult> OnGetAsync()
        {
            // 1. Securely terminate the session and remove the authentication cookie
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            // 2. Redirect the now-unauthenticated user to the login page
            return RedirectToPage("/Account/Login");
        }

        // Optional: Handle POST requests for extra security (CSRF protection built-in)
        public async Task<IActionResult> OnPostAsync()
        {
            return await OnGetAsync();
        }
    }
}

