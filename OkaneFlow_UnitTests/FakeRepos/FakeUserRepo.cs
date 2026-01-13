using Service.RepoInterface;
using Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OkaneFlow_UnitTests.FakeRepos
{
    public class FakeUserRepo : IUserRepo
    {
        private readonly List<UserModel> _users = new List<UserModel>();

        public UserModel? LastCreatedUser { get; private set; }
        public UserModel? LastUpdatedUser { get; private set; }

        public Task<UserModel?> GetByUsernameAsync(string username)
        {
            var user = _users.FirstOrDefault(u => u.Username == username);
            return Task.FromResult(user);
        }

        public Task<UserModel?> GetByIdAsync(Guid id)
        {
            var user = _users.FirstOrDefault(u => u.UserID == id);
            return Task.FromResult(user);
        }

        public Task<bool> CreateUserAsync(UserModel user)
        {
            _users.Add(user);
            LastCreatedUser = user;
            return Task.FromResult(true);
        }

        public Task<bool> UpdateUserAsync(UserModel user)
        {
            var existing = _users.FirstOrDefault(u => u.UserID == user.UserID);
            if (existing != null)
            {
                _users.Remove(existing);
                _users.Add(user);
                LastUpdatedUser = user;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<bool> DeleteUserAsync(Guid userId)
        {
            var user = _users.FirstOrDefault(u => u.UserID == userId);
            if (user != null)
            {
                _users.Remove(user);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<List<UserModel>> GetAllUsersAsync()
        {
            return Task.FromResult(_users.ToList());
        }
    }
}