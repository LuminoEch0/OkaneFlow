using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
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
        public RegisterInputModel Input { get; set; } = new RegisterInputModel();

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

        public class RegisterInputModel
        {
            [Required]
            [Display(Name = "Username")]
            public string Username { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
            [StrongPassword]
            public string Password { get; set; } = string.Empty;
        }
    }
}
