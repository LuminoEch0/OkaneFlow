using Service.Models;

namespace Service.RepoInterface
{
    public interface IUserPreferenceRepo
    {
        Task<UserPreferenceModel?> GetByUserIdAsync(Guid userId);

        Task<bool> CreateAsync(UserPreferenceModel preference);

        Task<bool> UpdateAsync(UserPreferenceModel preference);
    }
}
