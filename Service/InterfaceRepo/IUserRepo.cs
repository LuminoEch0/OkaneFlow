using Service.Models;

namespace Service.RepoInterface
{
    public interface IUserRepo
    {
        Task<UserModel?> GetByUsernameAsync(string username);

        Task<UserModel?> GetByIdAsync(Guid id);

        Task<bool> CreateUserAsync(UserModel user);

        Task<bool> UpdateUserAsync(UserModel user);

        Task<bool> DeleteUserAsync(Guid userId);

        Task<List<UserModel>> GetAllUsersAsync();
    }
}