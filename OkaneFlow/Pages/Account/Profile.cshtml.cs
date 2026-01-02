using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.Helpers;
using Service.RepoInterface;
using Service.Models;
using Service.Interface;
using System.ComponentModel.DataAnnotations;

namespace OkaneFlow.Pages.Account
{
    public class AccountModel : PageModel
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IUserRepo _userRepo;

        public AccountModel(ICurrentUserService userService, IUserRepo userRepo)
        {
            _currentUser = userService;
            _userRepo = userRepo;
        }

        // Display-only fields
        public string Role { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }

        // Profile editing bind model
        [BindProperty]
        public ProfileInputModel Profile { get; set; } = new ProfileInputModel();

        // Small status message for user feedback
        [TempData]
        public string? StatusMessage { get; set; }

        public async Task OnGetAsync()
        {
            if (string.IsNullOrEmpty(_currentUser.UserName))
            {
                return;
            }

            var model = await _userRepo.GetByUsernameAsync(_currentUser.UserName);
            if (model == null)
            {
                return;
            }

            Role = model.IsAdmin ? "Admin" : "User";
            CreationDate = model.CreationDate;

            // Populate the edit form with current values
            Profile.Username = model.Username;
            Profile.Email = model.Email;
        }

        public async Task<IActionResult> OnPostUpdateProfileAsync()
        {
            if (!ModelState.IsValid)
            {
                // Re-populate display fields on validation failure
                await PopulateDisplayFieldsAsync();
                return Page();
            }

            if (string.IsNullOrEmpty(_currentUser.UserName))
            {
                ModelState.AddModelError(string.Empty, "Unable to determine current user.");
                await PopulateDisplayFieldsAsync();
                return Page();
            }

            var model = await _userRepo.GetByUsernameAsync(_currentUser.UserName);
            if (model == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                await PopulateDisplayFieldsAsync();
                return Page();
            }

            // Apply changes
            model.Username = Profile.Username?.Trim() ?? model.Username;
            model.Email = Profile.Email?.Trim() ?? model.Email;

            var ok = await _userRepo.UpdateUserAsync(model);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, "Failed to update profile.");
                await PopulateDisplayFieldsAsync();
                return Page();
            }

            StatusMessage = "Profile updated successfully.";
            return RedirectToPage();
        }

        private async Task PopulateDisplayFieldsAsync()
        {
            if (!string.IsNullOrEmpty(_currentUser.UserName))
            {
                var model = await _userRepo.GetByUsernameAsync(_currentUser.UserName);
                if (model != null)
                {
                    Role = model.IsAdmin ? "Admin" : "User";
                    CreationDate = model.CreationDate;
                }
            }
        }

        // Input model for profile editing only
        public class ProfileInputModel
        {
            [Required]
            [StringLength(50)]
            [Display(Name = "Username")]
            public string Username { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            [StringLength(100)]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;
        }
    }
}
