using Microsoft.VisualStudio.TestTools.UnitTesting;
using OkaneFlow_UnitTests.FakeRepos;
using Service;
using Service.Models;

namespace OkaneFlow_UnitTests
{
    [TestClass]
    public class BankAccountServiceTests
    {
        private FakeBankAccountRepo _fakeRepo = null!;
        private FakeCurrentUserService _fakeCurrentUser = null!;
        private BankAccountService _service = null!;
        private Guid _testUserId;

        [TestInitialize]
        public void Setup()
        {
            _testUserId = Guid.NewGuid();
            _fakeRepo = new FakeBankAccountRepo();
            _fakeCurrentUser = new FakeCurrentUserService(_testUserId);
            _service = new BankAccountService(_fakeRepo, _fakeCurrentUser);
        }

        #region GetAccountsByUserIdAsync Tests

        [TestMethod]
        public async Task GetAccountsByUserIdAsync_UserHasMultipleAccounts_ReturnsAllAccounts()
        {
            // Arrange
            var account1 = new BankAccountModel(Guid.NewGuid(), _testUserId, "Checking", 100m);
            var account2 = new BankAccountModel(Guid.NewGuid(), _testUserId, "Savings", 500m);
            var unrelatedAccount = new BankAccountModel(Guid.NewGuid(), Guid.NewGuid(), "Other User", 0m);

            await _fakeRepo.CreateBankAccountAsync(account1);
            await _fakeRepo.CreateBankAccountAsync(account2);
            await _fakeRepo.CreateBankAccountAsync(unrelatedAccount);

            // Act
            var result = await _service.GetAccountsByUserIdAsync(_testUserId);

            // Assert
            Assert.AreEqual(2, result.Count, "Should return exactly the 2 accounts belonging to this user.");
            Assert.IsTrue(result.Any(a => a.AccountName == "Checking"));
            Assert.IsTrue(result.Any(a => a.AccountName == "Savings"));
        }

        [TestMethod]
        public async Task GetAccountsByUserIdAsync_UserHasNoAccounts_ReturnsEmptyList()
        {
            // Arrange
            var unrelatedAccount = new BankAccountModel(Guid.NewGuid(), Guid.NewGuid(), "Other User", 0m);
            await _fakeRepo.CreateBankAccountAsync(unrelatedAccount);

            // Act
            var result = await _service.GetAccountsByUserIdAsync(_testUserId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count, "Should return an empty list if no accounts exist for the user.");
        }

        [TestMethod]
        public async Task GetAccountsByUserIdAsync_EmptyUserId_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _service.GetAccountsByUserIdAsync(Guid.Empty));
        }

        [TestMethod]
        public async Task GetAccountsByUserIdAsync_UnauthorizedUserId_ThrowsUnauthorizedAccessException()
        {
            // Arrange - try to access another user's accounts
            var otherUserId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(async () => await _service.GetAccountsByUserIdAsync(otherUserId));
        }

        [TestMethod]
        public async Task GetAccountsByUserIdAsync_RepositoryThrows_ThrowsInvalidOperationException()
        {
            // Arrange
            var serviceWithFailingRepo = new BankAccountService(new FailingGetBankAccountsRepo(), _fakeCurrentUser);

            // Act & Assert
            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => await serviceWithFailingRepo.GetAccountsByUserIdAsync(_testUserId));
            Assert.IsTrue(ex.Message.Contains("An error occurred while retrieving accounts"));
        }

        #endregion

        #region GetAccountByIdAsync Tests

        [TestMethod]
        public async Task GetAccountByIdAsync_ValidBankAccountID_ReturnsAccount()
        {
            // Arrange
            var bankAccountID = Guid.NewGuid();
            var account = new BankAccountModel(bankAccountID, _testUserId, "Checking", 100m);
            await _fakeRepo.CreateBankAccountAsync(account);

            // Act
            var result = await _service.GetAccountByIdAsync(bankAccountID);

            // Assert
            Assert.IsNotNull(result, "Account should be found.");
            Assert.AreEqual(account.AccountID, result!.AccountID);
            Assert.AreEqual(account.AccountName, result.AccountName);
        }

        [TestMethod]
        public async Task GetAccountByIdAsync_EmptyBankAccountID_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _service.GetAccountByIdAsync(Guid.Empty));
        }

        [TestMethod]
        public async Task GetAccountByIdAsync_InvalidBankAccountID_ReturnsNull()
        {
            // Act
            var result = await _service.GetAccountByIdAsync(Guid.NewGuid());

            // Assert
            Assert.IsNull(result, "Non-existent account should return null.");
        }

        [TestMethod]
        public async Task GetAccountByIdAsync_UnauthorizedAccess_ThrowsUnauthorizedAccessException()
        {
            // Arrange - create account owned by another user
            var bankAccountID = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var account = new BankAccountModel(bankAccountID, otherUserId, "Other User Account", 100m);
            await _fakeRepo.CreateBankAccountAsync(account);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(async () => await _service.GetAccountByIdAsync(bankAccountID));
        }

        [TestMethod]
        public async Task GetAccountByIdAsync_RepositoryThrows_ThrowsInvalidOperationException()
        {
            // Arrange
            var serviceWithFailingRepo = new BankAccountService(new FailingGetByIdRepo(), _fakeCurrentUser);
            var accountId = Guid.NewGuid();

            // Act & Assert
            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => await serviceWithFailingRepo.GetAccountByIdAsync(accountId));
            Assert.IsTrue(ex.Message.Contains("An error occurred while retrieving account"));
        }

        #endregion

        #region UpdateAccountDetailsAsync Tests

        [TestMethod]
        public async Task UpdateAccountDetailsAsync_ValidAccount_UpdatesSuccessfully()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var account = new BankAccountModel(accountId, _testUserId, "Original Name", 500m);
            await _fakeRepo.CreateBankAccountAsync(account);

            var updatedAccount = new BankAccountModel(accountId, _testUserId, "Updated Name", 750m);

            // Act
            await _service.UpdateAccountDetailsAsync(updatedAccount);

            // Assert
            Assert.IsNotNull(_fakeRepo.LastAccountUpdated, "Account should have been updated.");
            Assert.AreEqual("Updated Name", _fakeRepo.LastAccountUpdated!.AccountName);
            Assert.AreEqual(750m, _fakeRepo.LastAccountUpdated.CurrentBalance);
        }

        [TestMethod]
        public async Task UpdateAccountDetailsAsync_NullAccount_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await _service.UpdateAccountDetailsAsync(null!));
        }

        [TestMethod]
        public async Task UpdateAccountDetailsAsync_EmptyAccountId_ThrowsArgumentException()
        {
            // Arrange
            var account = new BankAccountModel(Guid.Empty, _testUserId, "Name", 100m);

            // Act & Assert
            var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _service.UpdateAccountDetailsAsync(account));
            Assert.AreEqual("account", ex.ParamName);
            Assert.IsTrue(ex.Message.Contains("Account ID cannot be empty."));
        }

        [TestMethod]
        public async Task UpdateAccountDetailsAsync_EmptyAccountName_ThrowsArgumentException()
        {
            // Arrange
            var account = new BankAccountModel(Guid.NewGuid(), _testUserId, string.Empty, 100m);

            // Act & Assert
            var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _service.UpdateAccountDetailsAsync(account));
            Assert.AreEqual("account", ex.ParamName);
            Assert.IsTrue(ex.Message.Contains("Account name cannot be empty."));
        }

        [TestMethod]
        public async Task UpdateAccountDetailsAsync_WhitespaceAccountName_ThrowsArgumentException()
        {
            // Arrange
            var account = new BankAccountModel(Guid.NewGuid(), _testUserId, "   ", 100m);

            // Act & Assert
            var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _service.UpdateAccountDetailsAsync(account));
            Assert.IsTrue(ex.Message.Contains("Account name cannot be empty."));
        }

        [TestMethod]
        public async Task UpdateAccountDetailsAsync_UnauthorizedAccess_ThrowsUnauthorizedAccessException()
        {
            // Arrange - create account owned by another user
            var accountId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var account = new BankAccountModel(accountId, otherUserId, "Other Account", 100m);
            await _fakeRepo.CreateBankAccountAsync(account);

            var updatedAccount = new BankAccountModel(accountId, otherUserId, "Updated Name", 200m);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(async () => await _service.UpdateAccountDetailsAsync(updatedAccount));
        }

        [TestMethod]
        public async Task UpdateAccountDetailsAsync_RepositoryThrows_ThrowsInvalidOperationException()
        {
            // Arrange
            var failingRepo = new FailingUpdateRepo();
            var serviceWithFailingRepo = new BankAccountService(failingRepo, _fakeCurrentUser);
            var account = new BankAccountModel(Guid.NewGuid(), _testUserId, "Name", 100m);
            // Add existing account first
            await failingRepo.CreateBankAccountAsync(account);

            // Act & Assert
            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => await serviceWithFailingRepo.UpdateAccountDetailsAsync(account));
            Assert.IsTrue(ex.Message.Contains("An error occurred while updating account"));
        }

        #endregion

        #region DeleteAccountAsync Tests

        [TestMethod]
        public async Task DeleteAccountAsync_ValidBankAccountID_DeletesSuccessfully()
        {
            // Arrange
            var bankAccountID = Guid.NewGuid();
            var account = new BankAccountModel(bankAccountID, _testUserId, "Checking", 100m);
            await _fakeRepo.CreateBankAccountAsync(account);

            // Act
            await _service.DeleteAccountAsync(bankAccountID);

            // Assert
            Assert.IsNull(await _fakeRepo.GetBankAccountByIdAsync(bankAccountID), "Account should be deleted.");
        }

        [TestMethod]
        public async Task DeleteAccountAsync_EmptyBankAccountID_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _service.DeleteAccountAsync(Guid.Empty));
        }

        [TestMethod]
        public async Task DeleteAccountAsync_InvalidBankAccountID_ThrowsKeyNotFoundException()
        {
            // Act & Assert
            var ex = await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () => await _service.DeleteAccountAsync(Guid.NewGuid()));
            Assert.IsTrue(ex.Message.Contains("was not found"), "Exception message should indicate account not found.");
        }

        [TestMethod]
        public async Task DeleteAccountAsync_UnauthorizedAccess_ThrowsUnauthorizedAccessException()
        {
            // Arrange - create account owned by another user
            var bankAccountID = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var account = new BankAccountModel(bankAccountID, otherUserId, "Other Account", 100m);
            await _fakeRepo.CreateBankAccountAsync(account);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(async () => await _service.DeleteAccountAsync(bankAccountID));
        }

        [TestMethod]
        public async Task DeleteAccountAsync_RepositoryThrows_ThrowsInvalidOperationException()
        {
            // Arrange
            var failingRepo = new FailingDeleteRepo();
            var serviceWithFailingRepo = new BankAccountService(failingRepo, _fakeCurrentUser);
            var accountId = Guid.NewGuid();
            var account = new BankAccountModel(accountId, _testUserId, "Test", 100m);
            await failingRepo.CreateBankAccountAsync(account);

            // Act & Assert
            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => await serviceWithFailingRepo.DeleteAccountAsync(accountId));
            Assert.IsTrue(ex.Message.Contains("An error occurred while deleting account"));
        }

        #endregion

        #region CreateAccountAsync Tests

        [TestMethod]
        public async Task CreateAccountAsync_ValidBankAccount_CreatesSuccessfully()
        {
            // Arrange
            var bankAccountID = Guid.NewGuid();
            var account = new BankAccountModel(bankAccountID, _testUserId, "Checking", 100m);

            // Act
            await _service.CreateAccountAsync(account);

            // Assert
            Assert.IsNotNull(_fakeRepo.LastAccountCreated, "Account should have been created.");
            Assert.AreEqual(account.AccountID, _fakeRepo.LastAccountCreated!.AccountID);
            Assert.AreEqual(account.AccountName, _fakeRepo.LastAccountCreated.AccountName);
            Assert.AreEqual(100m, _fakeRepo.LastAccountCreated.CurrentBalance);
        }

        [TestMethod]
        public async Task CreateAccountAsync_ZeroBalance_CreatesSuccessfully()
        {
            // Arrange
            var account = new BankAccountModel(Guid.NewGuid(), _testUserId, "Checking", 0m);

            // Act
            await _service.CreateAccountAsync(account);

            // Assert
            Assert.IsNotNull(_fakeRepo.LastAccountCreated);
            Assert.AreEqual(0m, _fakeRepo.LastAccountCreated!.CurrentBalance);
        }

        [TestMethod]
        public async Task CreateAccountAsync_NullAccount_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await _service.CreateAccountAsync(null!));
        }

        [TestMethod]
        public async Task CreateAccountAsync_EmptyUserID_ThrowsArgumentException()
        {
            // Arrange
            var account = new BankAccountModel(Guid.NewGuid(), Guid.Empty, "Checking", 100m);

            // Act & Assert
            var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _service.CreateAccountAsync(account));
            Assert.IsTrue(ex.Message.Contains("User ID cannot be empty."));
        }

        [TestMethod]
        public async Task CreateAccountAsync_NegativeBalance_ThrowsArgumentException()
        {
            // Arrange
            var account = new BankAccountModel(Guid.NewGuid(), _testUserId, "Checking", -100m);

            // Act & Assert
            var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _service.CreateAccountAsync(account));
            Assert.IsTrue(ex.Message.Contains("Initial balance cannot be negative."));
        }

        [TestMethod]
        public async Task CreateAccountAsync_EmptyAccountName_ThrowsArgumentException()
        {
            // Arrange
            var account = new BankAccountModel(Guid.NewGuid(), _testUserId, "", 100m);

            // Act & Assert
            var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _service.CreateAccountAsync(account));
            Assert.IsTrue(ex.Message.Contains("Account name cannot be empty."));
        }

        [TestMethod]
        public async Task CreateAccountAsync_WhitespaceAccountName_ThrowsArgumentException()
        {
            // Arrange
            var account = new BankAccountModel(Guid.NewGuid(), _testUserId, "   ", 100m);

            // Act & Assert
            var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _service.CreateAccountAsync(account));
            Assert.IsTrue(ex.Message.Contains("Account name cannot be empty."));
        }

        [TestMethod]
        public async Task CreateAccountAsync_UnauthorizedUserID_ThrowsUnauthorizedAccessException()
        {
            // Arrange - try to create account for another user
            var otherUserId = Guid.NewGuid();
            var account = new BankAccountModel(Guid.NewGuid(), otherUserId, "Checking", 100m);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(async () => await _service.CreateAccountAsync(account));
        }

        [TestMethod]
        public async Task CreateAccountAsync_RepositoryThrows_ThrowsInvalidOperationException()
        {
            // Arrange
            var serviceWithFailingRepo = new BankAccountService(new FailingCreateRepo(), _fakeCurrentUser);
            var account = new BankAccountModel(Guid.NewGuid(), _testUserId, "Checking", 100m);

            // Act & Assert
            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => await serviceWithFailingRepo.CreateAccountAsync(account));
            Assert.IsTrue(ex.Message.Contains("An error occurred while creating the bank account"));
        }

        #endregion

        #region Helper Classes for Testing Exception Handling

        private class FailingGetBankAccountsRepo : FakeBankAccountRepo
        {
            public override Task<List<BankAccountModel>> GetBankAccountsAsync(Guid id)
            {
                throw new Exception("Database connection failed");
            }
        }

        private class FailingGetByIdRepo : FakeBankAccountRepo
        {
            public override Task<BankAccountModel?> GetBankAccountByIdAsync(Guid id)
            {
                throw new Exception("Database connection failed");
            }
        }

        private class FailingUpdateRepo : FakeBankAccountRepo
        {
            public override Task UpdateBankAccountAsync(BankAccountModel account)
            {
                throw new Exception("Database connection failed");
            }
        }

        private class FailingDeleteRepo : FakeBankAccountRepo
        {
            public override Task DeleteBankAccountAsync(Guid id)
            {
                throw new Exception("Database connection failed");
            }
        }

        private class FailingCreateRepo : FakeBankAccountRepo
        {
            public override Task CreateBankAccountAsync(BankAccountModel account)
            {
                throw new Exception("Database connection failed");
            }
        }

        #endregion
    }
}
