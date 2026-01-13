using Service.Interface;

namespace OkaneFlow_UnitTests.FakeRepos
{
    public class FakePasswordManager : IPasswordManager
    {
        public bool WasHashPasswordCalled { get; private set; }
        public bool WasVerifyPasswordCalled { get; private set; }
        public string? LastPasswordHashed { get; private set; }
        public string? LastPasswordVerified { get; private set; }

        public string HashPassword(string password)
        {
            WasHashPasswordCalled = true;
            LastPasswordHashed = password;
            // Return a fake hash - just prefix with "HASHED_"
            return $"HASHED_{password}";
        }

        public bool VerifyPassword(string password, string storedHash)
        {
            WasVerifyPasswordCalled = true;
            LastPasswordVerified = password;
            // Simple verification: check if stored hash matches our fake pattern
            return storedHash == $"HASHED_{password}";
        }
    }
}