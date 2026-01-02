using Service.Interface;
using Service.RepoInterface;
using Service.Models;

namespace Service
{
    public class UserPreferenceService : IUserPreferenceService
    {
        private readonly IUserPreferenceRepo _preferenceRepo;

        public UserPreferenceService(IUserPreferenceRepo preferenceRepo)
        {
            _preferenceRepo = preferenceRepo;
        }

        public async Task<UserPreferenceModel> GetOrCreateAsync(Guid userId)
        {
            var existing = await _preferenceRepo.GetByUserIdAsync(userId);
            if (existing != null)
            {
                return existing;
            }

            // Create default preferences for this user
            var newPreference = new UserPreferenceModel
            {
                PreferenceID = Guid.NewGuid(),
                UserID = userId,
                DarkMode = false,
                EmailNotifications = true,
                Currency = "EUR",
                DateFormat = "dd/MM/yyyy"
            };

            await _preferenceRepo.CreateAsync(newPreference);
            return newPreference;
        }

        public Task<bool> UpdateAsync(UserPreferenceModel preference)
        {
            return _preferenceRepo.UpdateAsync(preference);
        }
    }
}
