using Service.Interface;
using Service.RepoInterface;
using Service.Models;
using System;

namespace Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepository;
        private readonly IPasswordManager _passwordManager;

        public UserService(IUserRepo userRepository, IPasswordManager passwordManager)
        {
            _userRepository = userRepository;
            _passwordManager = passwordManager;
        }

        public async Task<UserModel?> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);

            if (user == null)
            {
                return null;
            }

            if (_passwordManager.VerifyPassword(password, user.PasswordHash))
            {
                return user;
            }

            Console.WriteLine("Authentication Failed");
            return null;
        }

        public Task<bool> CreateUserAsync(string username, string email, string password, string role = "User")
        {
            var hashed = _passwordManager.HashPassword(password);
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Username, email, and password are required.");
            }
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

        public Task<List<UserModel>> GetAllUsersAsync()
        {
            return _userRepository.GetAllUsersAsync();
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            return await _userRepository.DeleteUserAsync(userId);
        }

        public System.Security.Claims.ClaimsPrincipal CreateUserPrincipal(UserModel user)
        {
            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Username),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
            };

            var claimsIdentity = new System.Security.Claims.ClaimsIdentity(claims, "Cookies");
            return new System.Security.Claims.ClaimsPrincipal(claimsIdentity);
        }
    }
}
