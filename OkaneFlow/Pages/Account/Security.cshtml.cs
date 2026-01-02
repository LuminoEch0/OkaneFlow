using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.Helpers;
using Service.RepoInterface;
using Service.Models;
using Service.Interface;
using Service;

namespace OkaneFlow.Pages.Account
{
    public class SecurityModel : PageModel
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IUserRepo _userRepo;
        private readonly IUserService _userService;

        public SecurityModel(ICurrentUserService currentUser, IUserRepo userRepo, IUserService userService)
        {
            _currentUser = currentUser;
            _userRepo = userRepo;
            _userService = userService;
        }

        // Display properties
        public DateTime CreationDate { get; set; }
        public DateTime? LastLoginDate { get; set; }

        // Password change bind model
        [BindProperty]
        public PasswordInputModel PasswordChange { get; set; } = new PasswordInputModel();

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task OnGetAsync()
        {
            await PopulateDisplayFieldsAsync();
        }

        private async Task PopulateDisplayFieldsAsync()
        {
            if (string.IsNullOrEmpty(_currentUser.UserName))
            {
                return;
            }

            var user = await _userRepo.GetByUsernameAsync(_currentUser.UserName);
            if (user == null)
            {
                return;
            }

            CreationDate = user.CreationDate;
            LastLoginDate = user.LastLoginDate;
        }

        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            // Re-populate display fields
            await PopulateDisplayFieldsAsync();

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

            var user = await _userRepo.GetByUsernameAsync(_currentUser.UserName);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return Page();
            }

            // Hash new password and save
            user.PasswordHash = PassswordManager.HashPassword(PasswordChange.NewPassword);
            var ok = await _userRepo.UpdateUserAsync(user);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, "Failed to change password.");
                return Page();
            }

            StatusMessage = "Password changed successfully.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAccountAsync(string confirmPassword)
        {
            await PopulateDisplayFieldsAsync();

            if (string.IsNullOrEmpty(_currentUser.UserName))
            {
                ModelState.AddModelError(string.Empty, "Unable to determine current user.");
                return Page();
            }

            // Verify password before deletion
            var verified = await _userService.AuthenticateAsync(_currentUser.UserName, confirmPassword);
            if (verified == null)
            {
                ModelState.AddModelError(string.Empty, "Password is incorrect. Account not deleted.");
                return Page();
            }

            var user = await _userRepo.GetByUsernameAsync(_currentUser.UserName);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return Page();
            }

            var deleted = await _userRepo.DeleteUserAsync(user.UserID);
            if (!deleted)
            {
                ModelState.AddModelError(string.Empty, "Failed to delete account.");
                return Page();
            }

            // Sign out and redirect to login
            await HttpContext.SignOutAsync();
            return RedirectToPage("/Account/Login");
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
            [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
            [StrongPassword]
            public string NewPassword { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare(nameof(NewPassword), ErrorMessage = "The new password and confirmation do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }
    }
}
