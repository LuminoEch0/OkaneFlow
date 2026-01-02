using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace OkaneFlow.Helpers
{
    /// <summary>
    /// Custom validation attribute for strong passwords.
    /// Requires: 8+ characters, uppercase, lowercase, digit, and special character.
    /// </summary>
    public class StrongPasswordAttribute : ValidationAttribute
    {
        public StrongPasswordAttribute()
        {
            ErrorMessage = "Password must be at least 8 characters and contain uppercase, lowercase, a digit, and a special character.";
        }

        public override bool IsValid(object? value)
        {
            if (value == null)
                return false;

            var password = value.ToString() ?? string.Empty;

            if (password.Length < 8)
                return false;

            // Check for uppercase
            if (!Regex.IsMatch(password, @"[A-Z]"))
                return false;

            // Check for lowercase
            if (!Regex.IsMatch(password, @"[a-z]"))
                return false;

            // Check for digit
            if (!Regex.IsMatch(password, @"\d"))
                return false;

            // Check for special character
            if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?""':{}|<>_\-+=\[\]\\\/`~]"))
                return false;

            return true;
        }
    }
}
