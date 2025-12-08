using DataAccessLayer.DataTransferObjects;

namespace Service.Interface
{
    public interface IUserService
    {
        Task<UserDTO?> AuthenticateAsync(string username, string password);

        Task<bool> CreateUserAsync(string username, string email, string password, string role = "User");
    }
}
