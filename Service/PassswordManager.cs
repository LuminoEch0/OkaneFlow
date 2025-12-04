using System;
using System.Collections.Generic;
using System.Text;
using BCrypt.Net;

namespace Service
{
    public static class PassswordManager
    {
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);

            // DEMO ONLY: Return a mock hash
            //return $"BCrypt_Secure_Hash_For_{password}";
        }

        // Simulates verifying a password against a stored hash (used during login)
        public static bool VerifyPassword(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);

            // DEMO ONLY: Simple string comparison based on mock hashing logic
            //return HashPassword(password).Equals(storedHash);
        }
    }
}
