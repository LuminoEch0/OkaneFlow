using Service.Models;

namespace Service.Interface
{
    public interface IUserPreferenceService
    {
        /// <summary>
        /// Gets user preferences, creating default preferences if none exist.
        /// </summary>
        Task<UserPreferenceModel> GetOrCreateAsync(Guid userId);

        /// <summary>
        /// Updates user preferences.
        /// </summary>
        Task<bool> UpdateAsync(UserPreferenceModel preference);
    }
}
