using Service.Interface;
using Service.RepoInterface;
using Service.Models;
using System;

namespace Service
{
    public class UserService : IUserService
    {

        private readonly IUserRepo _userRepository;

        public UserService(IUserRepo userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserModel?> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);

            if (user == null)
            {
                return null;
            }

            if (PassswordManager.VerifyPassword(password, user.PasswordHash))
            {
                return user;
            }

            Console.WriteLine("Authentication Failed");
            return null;
        }

        public Task<bool> CreateUserAsync(string username, string email, string password, string role = "User")
        {
            var hashed = PassswordManager.HashPassword(password);

            var user = new UserModel
            {
                UserID = Guid.NewGuid(),
                Username = username,
                Email = email,
                PasswordHash = hashed,
                CreationDate = DateTime.UtcNow,
                IsAdmin = string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase)
            };

            return _userRepository.CreateUserAsync(user);
        }

        public async Task<bool> UpdateLastLoginAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.LastLoginDate = DateTime.UtcNow;
            return await _userRepository.UpdateUserAsync(user);
        }
    }
}
