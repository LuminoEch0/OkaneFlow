using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.Helpers;
using DataAccessLayer.Repositories.Interface;
using DataAccessLayer.DataTransferObjects;
using Service.Interface;
using Service; // for PassswordManager (used by UserService)
using System.ComponentModel.DataAnnotations;

namespace OkaneFlow.Pages.Account
{
    public class AccountModel : PageModel
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IUserRepo _userRepo;
        private readonly IUserService _userService;

        public AccountModel(IConfiguration configuration, ICurrentUserService userService, IUserRepo userRepo, IUserService userServiceLayer)
        {
            _currentUser = userService;
            _userRepo = userRepo;
            _userService = userServiceLayer;
        }

        // Display-only fields
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }

        // Profile editing bind model
        [BindProperty]
        public ProfileInputModel Profile { get; set; } = new ProfileInputModel();

        // Password change bind model
        [BindProperty]
        public PasswordInputModel PasswordChange { get; set; } = new PasswordInputModel();

        // Small status message for user feedback
        [TempData]
        public string? StatusMessage { get; set; }

        public async Task OnGetAsync()
        {
            if (string.IsNullOrEmpty(_currentUser.UserName))
            {
                // not authenticated or username unavailable
                return;
            }

            var dto = await _userRepo.GetByUsernameAsync(_currentUser.UserName);
            if (dto == null)
            {
                return;
            }

            UserName = dto.Username;
            Role = dto.Role;
            CreationDate = dto.CreationDate;

            // Populate the edit form with current values
            Profile.Username = dto.Username;
            Profile.Email = dto.Email;
        }

        public async Task<IActionResult> OnPostUpdateProfileAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (string.IsNullOrEmpty(_currentUser.UserName))
            {
                ModelState.AddModelError(string.Empty, "Unable to determine current user.");
                return Page();
            }

            var dto = await _userRepo.GetByUsernameAsync(_currentUser.UserName);
            if (dto == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return Page();
            }

            // Apply changes
            dto.Username = Profile.Username?.Trim() ?? dto.Username;
            dto.Email = Profile.Email?.Trim() ?? dto.Email;

            var ok = await _userRepo.UpdateUserAsync(dto);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, "Failed to update profile.");
                return Page();
            }

            // If username changed we don't automatically re-issue cookie here; user will still be authenticated with old claims.
            StatusMessage = "Profile updated successfully.";
            return RedirectToPage(); // reload to refresh displayed values
        }

        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (string.IsNullOrEmpty(_currentUser.UserName))
            {
                ModelState.AddModelError(string.Empty, "Unable to determine current user.");
                return Page();
            }

            // Verify current password
            var verified = await _userService.AuthenticateAsync(_currentUser.UserName, PasswordChange.CurrentPassword);
            if (verified == null)
            {
                ModelState.AddModelError(string.Empty, "Current password is incorrect.");
                return Page();
            }

            if (PasswordChange.NewPassword != PasswordChange.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "New password and confirmation do not match.");
                return Page();
            }

            var dto = await _userRepo.GetByUsernameAsync(_currentUser.UserName);
            if (dto == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return Page();
            }

            // Hash new password and save
            dto.HashedPassword = PassswordManager.HashPassword(PasswordChange.NewPassword);
            var ok = await _userRepo.UpdateUserAsync(dto);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, "Failed to change password.");
                return Page();
            }

            StatusMessage = "Password changed successfully.";
            return RedirectToPage();
        }

        // Input models used only for binding on the page
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

        public class PasswordInputModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string CurrentPassword { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            [StringLength(100, MinimumLength = 6)]
            public string NewPassword { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare(nameof(NewPassword), ErrorMessage = "The new password and confirmation do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }
    }
}
