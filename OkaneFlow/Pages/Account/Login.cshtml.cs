using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Service;
using Service.Interface;
using OkaneFlow.Helpers;

namespace OkaneFlow.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly IUserService _userService;

        public LoginModel(IUserService userService)
        {
            _userService = userService;
        }

        // Input Model for the Login Form
        [BindProperty]
        public LoginInputModel Input { get; set; } = new LoginInputModel();


        public async Task OnGetAsync()
        {
            // Clear any existing external cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 1. Service Layer Call (Validation Logic)
            var user = await _userService.AuthenticateAsync(Input.Username, Input.Password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            // 2. Create Claims (The user identity data)
            var principal = _userService.CreateUserPrincipal(user);

            var authProperties = new AuthenticationProperties
            {
                // IsPersistent = true/false (for "Remember Me" functionality)
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            // 3. Sign In (Securely creates the authentication cookie)
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties);

            // 4. Update last login date
            await _userService.UpdateLastLoginAsync(user.UserID);

            // Redirect user to the requested page or the home page
            return LocalRedirect(returnUrl ?? "/Redirect");
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Account/Login");
        }

        public class LoginInputModel
        {
            [Required]
            [Display(Name = "Username")]
            public string Username { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = string.Empty;
        }
    }
}

