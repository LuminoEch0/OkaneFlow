using Service.Models;

namespace Service.Interface
{
    public interface IUserService
    {
        Task<UserModel?> AuthenticateAsync(string username, string password);

        Task<bool> CreateUserAsync(string username, string email, string password, string role = "User");

        Task<bool> UpdateLastLoginAsync(Guid userId);
    }
}
