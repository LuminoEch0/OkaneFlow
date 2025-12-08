using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.Helpers;
using Service;
using Service.Interface;

namespace OkaneFlow.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly IUserService _userService;

        public RegisterModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public void OnGet()
        {
            // show registration page
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var created = await _userService.CreateUserAsync(Input.Username, Input.Email, Input.Password);

            if (!created)
            {
                ModelState.AddModelError(string.Empty, "Failed to create user.");
                return Page();
            }

            // After successful registration redirect to login page
            return RedirectToPage("/Account/Login");
        }
    }
}
