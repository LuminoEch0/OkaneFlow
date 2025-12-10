using DataAccessLayer.DataTransferObjects;

namespace DataAccessLayer.Repositories.Interface
{
    public interface IUserRepo
    {
        Task<UserDTO?> GetByUsernameAsync(string username);

        Task<UserDTO?> GetByIdAsync(int id);

        Task<bool> CreateUserAsync(UserDTO user);

        Task<bool> UpdateUserAsync(UserDTO user);

        Task<bool> DeleteUserAsync(int userId);

        Task<List<UserDTO>> GetAllUsersAsync();
    }
}