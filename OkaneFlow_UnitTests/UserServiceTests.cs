using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;
using Service.Models;
using OkaneFlow_UnitTests.FakeRepos;
using System;

namespace OkaneFlow_UnitTests
{
    [TestClass]
    public class UserServiceTests
    {
        private FakeUserRepo _fakeUserRepo = null!;
        private FakePasswordManager _fakePasswordManager = null!;
        private UserService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _fakeUserRepo = new FakeUserRepo();
            _fakePasswordManager = new FakePasswordManager();
            _service = new UserService(_fakeUserRepo, _fakePasswordManager);
        }

        #region CreateUserAsync Tests

        [TestMethod]
        public async Task CreateUserAsync_ValidInputs_CreatesUserSuccessfully()
        {
            // Arrange
            string username = "validuser";
            string email = "valid@example.com";
            string password = "ValidPassword123";

            // Act
            bool result = await _service.CreateUserAsync(username, email, password);

            // Assert
            Assert.IsTrue(result, "User should be created successfully with valid inputs.");
            Assert.IsNotNull(_fakeUserRepo.LastCreatedUser, "A user should have been created.");
            Assert.AreEqual(username, _fakeUserRepo.LastCreatedUser!.Username);
            Assert.AreEqual(email, _fakeUserRepo.LastCreatedUser.Email);
            Assert.IsTrue(_fakePasswordManager.WasHashPasswordCalled, "Password should be hashed.");
        }

        [TestMethod]
        public async Task CreateUserAsync_WithUserRole_CreatesNonAdminUser()
        {
            // Arrange
            string username = "regularuser";
            string email = "user@example.com";
            string password = "Password123";
            string role = "User";

            // Act
            bool result = await _service.CreateUserAsync(username, email, password, role);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(_fakeUserRepo.LastCreatedUser);
            Assert.IsFalse(_fakeUserRepo.LastCreatedUser!.IsAdmin, "User with 'User' role should not be admin.");
        }

        [TestMethod]
        public async Task CreateUserAsync_WithAdminRole_CreatesAdminUser()
        {
            // Arrange
            string username = "adminuser";
            string email = "admin@example.com";
            string password = "AdminPassword123";
            string role = "Admin";

            // Act
            bool result = await _service.CreateUserAsync(username, email, password, role);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(_fakeUserRepo.LastCreatedUser);
            Assert.IsTrue(_fakeUserRepo.LastCreatedUser!.IsAdmin, "User with 'Admin' role should be admin.");
        }

        [TestMethod]
        public async Task CreateUserAsync_WithAdminRoleLowercase_CreatesAdminUser()
        {
            // Arrange
            string username = "adminuser2";
            string email = "admin2@example.com";
            string password = "AdminPassword123";
            string role = "admin"; // lowercase

            // Act
            bool result = await _service.CreateUserAsync(username, email, password, role);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(_fakeUserRepo.LastCreatedUser);
            Assert.IsTrue(_fakeUserRepo.LastCreatedUser!.IsAdmin, "Role comparison should be case-insensitive.");
        }

        [TestMethod]
        public async Task CreateUserAsync_DefaultRole_CreatesNonAdminUser()
        {
            // Arrange
            string username = "defaultuser";
            string email = "default@example.com";
            string password = "Password123";

            // Act - Not passing role parameter, should default to "User"
            bool result = await _service.CreateUserAsync(username, email, password);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(_fakeUserRepo.LastCreatedUser);
            Assert.IsFalse(_fakeUserRepo.LastCreatedUser!.IsAdmin, "Default role should be non-admin.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task CreateUserAsync_NullUsername_ThrowsArgumentException()
        {
            // Arrange
            string? username = null;
            string email = "test@example.com";
            string password = "ValidPassword123";

            // Act
            await _service.CreateUserAsync(username!, email, password);

            // Assert - Exception handled already
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task CreateUserAsync_EmptyUsername_ThrowsArgumentException()
        {
            // Arrange
            string username = "";
            string email = "test@example.com";
            string password = "ValidPassword123";

            // Act
            await _service.CreateUserAsync(username, email, password);

            // Assert - Exception expected
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task CreateUserAsync_NullEmail_ThrowsArgumentException()
        {
            // Arrange
            string username = "testuser";
            string? email = null;
            string password = "ValidPassword123";

            // Act
            await _service.CreateUserAsync(username, email!, password);

            // Assert - Exception expected
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task CreateUserAsync_EmptyEmail_ThrowsArgumentException()
        {
            // Arrange
            string username = "testuser";
            string email = "";
            string password = "ValidPassword123";

            // Act
            await _service.CreateUserAsync(username, email, password);

            // Assert - Exception expected
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task CreateUserAsync_NullPassword_ThrowsArgumentException()
        {
            // Arrange
            string username = "testuser";
            string email = "test@example.com";
            string? password = null;

            // Act
            await _service.CreateUserAsync(username, email, password!);

            // Assert - Exception expected
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task CreateUserAsync_EmptyPassword_ThrowsArgumentException()
        {
            // Arrange
            string username = "testuser";
            string email = "test@example.com";
            string password = "";

            // Act
            await _service.CreateUserAsync(username, email, password);

            // Assert - Exception expected
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task CreateUserAsync_AllFieldsNull_ThrowsArgumentException()
        {
            // Arrange
            string? username = null;
            string? email = null;
            string? password = null;

            // Act
            await _service.CreateUserAsync(username!, email!, password!);

            // Assert - Exception expected
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task CreateUserAsync_AllFieldsEmpty_ThrowsArgumentException()
        {
            // Arrange
            string username = "";
            string email = "";
            string password = "";

            // Act
            await _service.CreateUserAsync(username, email, password);

            // Assert - Exception expected
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task CreateUserAsync_WhitespaceUsername_ThrowsArgumentException()
        {
            // Arrange
            string username = "   ";
            string email = "test@example.com";
            string password = "ValidPassword123";

            // Act
            await _service.CreateUserAsync(username, email, password);

            // Assert - Exception expected
        }

        #endregion

        #region AuthenticateAsync Tests

        [TestMethod]
        public async Task AuthenticateAsync_ValidCredentials_ReturnsUser()
        {
            // Arrange
            string username = "testuser";
            string password = "Password123";
            var user = new UserModel
            {
                UserID = Guid.NewGuid(),
                Username = username,
                Email = "test@example.com",
                PasswordHash = "HASHED_Password123", // Matches FakePasswordManager pattern
                CreationDate = DateTime.UtcNow
            };
            await _fakeUserRepo.CreateUserAsync(user);

            // Act
            var result = await _service.AuthenticateAsync(username, password);

            // Assert
            Assert.IsNotNull(result, "Valid credentials should return a user.");
            Assert.AreEqual(username, result!.Username);
            Assert.IsTrue(_fakePasswordManager.WasVerifyPasswordCalled, "Password verification should be called.");
        }

        [TestMethod]
        public async Task AuthenticateAsync_InvalidPassword_ReturnsNull()
        {
            // Arrange
            string username = "testuser";
            string wrongPassword = "WrongPassword";
            var user = new UserModel
            {
                UserID = Guid.NewGuid(),
                Username = username,
                Email = "test@example.com",
                PasswordHash = "HASHED_Password123", // Matches correct password
                CreationDate = DateTime.UtcNow
            };
            await _fakeUserRepo.CreateUserAsync(user);

            // Act
            var result = await _service.AuthenticateAsync(username, wrongPassword);

            // Assert
            Assert.IsNull(result, "Invalid password should return null.");
            Assert.IsTrue(_fakePasswordManager.WasVerifyPasswordCalled, "Password verification should still be attempted.");
        }

        [TestMethod]
        public async Task AuthenticateAsync_UserNotFound_ReturnsNull()
        {
            // Arrange
            string username = "nonexistentuser";
            string password = "Password123";

            // Act
            var result = await _service.AuthenticateAsync(username, password);

            // Assert
            Assert.IsNull(result, "Non-existent user should return null.");
        }

        [TestMethod]
        public async Task AuthenticateAsync_EmptyUsername_ReturnsNull()
        {
            // Arrange
            string username = "";
            string password = "Password123";

            // Act
            var result = await _service.AuthenticateAsync(username, password);

            // Assert
            Assert.IsNull(result, "Empty username should return null.");
        }

        [TestMethod]
        public async Task AuthenticateAsync_EmptyPassword_ReturnsNull()
        {
            // Arrange
            string username = "testuser";
            string password = "";
            var user = new UserModel
            {
                UserID = Guid.NewGuid(),
                Username = username,
                Email = "test@example.com",
                PasswordHash = "HASHED_Password123",
                CreationDate = DateTime.UtcNow
            };
            await _fakeUserRepo.CreateUserAsync(user);

            // Act
            var result = await _service.AuthenticateAsync(username, password);

            // Assert
            Assert.IsNull(result, "Empty password should fail authentication.");
        }

        #endregion

        #region UpdateLastLoginAsync Tests

        [TestMethod]
        public async Task UpdateLastLoginAsync_ValidUserId_UpdatesLastLoginAndReturnsTrue()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new UserModel
            {
                UserID = userId,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "HASHED_Password123",
                CreationDate = DateTime.UtcNow,
                LastLoginDate = null // Initially null
            };
            await _fakeUserRepo.CreateUserAsync(user);

            var beforeUpdate = DateTime.UtcNow;

            // Act
            bool result = await _service.UpdateLastLoginAsync(userId);

            // Assert
            Assert.IsTrue(result, "Update should return true for valid user.");
            Assert.IsNotNull(_fakeUserRepo.LastUpdatedUser, "User should have been updated.");
            Assert.IsNotNull(_fakeUserRepo.LastUpdatedUser!.LastLoginDate, "LastLoginDate should be set.");
            Assert.IsTrue(_fakeUserRepo.LastUpdatedUser.LastLoginDate >= beforeUpdate, 
                "LastLoginDate should be recent.");
        }

        [TestMethod]
        public async Task UpdateLastLoginAsync_UserNotFound_ReturnsFalse()
        {
            // Arrange
            var nonExistentUserId = Guid.NewGuid();

            // Act
            bool result = await _service.UpdateLastLoginAsync(nonExistentUserId);

            // Assert
            Assert.IsFalse(result, "Update should return false when user is not found.");
            Assert.IsNull(_fakeUserRepo.LastUpdatedUser, "No user should have been updated.");
        }

        [TestMethod]
        public async Task UpdateLastLoginAsync_UpdatesExistingLastLoginDate()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var oldLoginDate = DateTime.UtcNow.AddDays(-7); // A week ago
            var user = new UserModel
            {
                UserID = userId,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "HASHED_Password123",
                CreationDate = DateTime.UtcNow.AddDays(-30),
                LastLoginDate = oldLoginDate
            };
            await _fakeUserRepo.CreateUserAsync(user);

            // Act
            bool result = await _service.UpdateLastLoginAsync(userId);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(_fakeUserRepo.LastUpdatedUser);
            Assert.IsTrue(_fakeUserRepo.LastUpdatedUser!.LastLoginDate > oldLoginDate,
                "LastLoginDate should be updated to a more recent time.");
        }

        [TestMethod]
        public async Task UpdateLastLoginAsync_EmptyGuid_ReturnsFalse()
        {
            // Arrange
            var emptyGuid = Guid.Empty;

            // Act
            bool result = await _service.UpdateLastLoginAsync(emptyGuid);

            // Assert
            Assert.IsFalse(result, "Update should return false for empty GUID.");
        }

        #endregion
    }
}
