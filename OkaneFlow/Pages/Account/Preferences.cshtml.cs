using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.Helpers;
using Service.Interface;
using Service.Models;

namespace OkaneFlow.Pages.Account
{
    public class PreferencesModel : PageModel
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IUserPreferenceService _preferenceService;

        public PreferencesModel(ICurrentUserService currentUser, IUserPreferenceService preferenceService)
        {
            _currentUser = currentUser;
            _preferenceService = preferenceService;
        }

        [BindProperty]
        public PreferencesInputModel Preferences { get; set; } = new PreferencesInputModel();

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task OnGetAsync()
        {
            if (_currentUser.UserGuid == Guid.Empty)
            {
                return;
            }

            var prefs = await _preferenceService.GetOrCreateAsync(_currentUser.UserGuid);
            Preferences.DarkMode = prefs.DarkMode;
            Preferences.EmailNotifications = prefs.EmailNotifications;
            Preferences.Currency = prefs.Currency;
            Preferences.DateFormat = prefs.DateFormat;
            Preferences.PreferenceID = prefs.PreferenceID;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (_currentUser.UserGuid == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, "Unable to determine current user.");
                return Page();
            }

            var prefs = await _preferenceService.GetOrCreateAsync(_currentUser.UserGuid);

            // Update with form values
            prefs.DarkMode = Preferences.DarkMode;
            prefs.EmailNotifications = Preferences.EmailNotifications;
            prefs.Currency = Preferences.Currency;
            prefs.DateFormat = Preferences.DateFormat;

            var ok = await _preferenceService.UpdateAsync(prefs);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, "Failed to save preferences.");
                return Page();
            }

            StatusMessage = "Preferences saved successfully.";
            return RedirectToPage();
        }

        public class PreferencesInputModel
        {
            public Guid PreferenceID { get; set; }
            public bool DarkMode { get; set; }
            public bool EmailNotifications { get; set; } = true;
            public string Currency { get; set; } = "EUR";
            public string DateFormat { get; set; } = "dd/MM/yyyy";
        }
    }
}
