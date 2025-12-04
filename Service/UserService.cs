using DataAccessLayer.DataTransferObjects;
using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public class UserService
    {

        private readonly UserRepo _userRepository;

        public UserService(UserRepo userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDTO?> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);

            if (user == null)
            {
                return null;
            }

            if (PassswordManager.VerifyPassword(password, user.HashedPassword))
            {
                return user; 
            }
            Console.WriteLine("Authentication Failed");
            return null;
        }

        public Task<bool> CreateUserAsync(string username, string email, string password, string role = "User")
        {
            var hashed = PassswordManager.HashPassword(password);

            // generate a pseudo-random positive int id for the demo (replace with proper identity in production)
            var id = Guid.NewGuid();

            var dto = new UserDTO
            {
                Id = id,
                Username = username,
                Email = email,
                HashedPassword = hashed,
                Role = role ?? "User",
                CreationDate = DateTime.UtcNow
            };

            return _userRepository.CreateUserAsync(dto);
        }
    }
}
