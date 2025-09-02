using Microsoft.AspNetCore.Identity;

namespace Feedback_System.Services
{
    public class PasswordServices
    {

        private readonly PasswordHasher<string> _passwordHasher;

        public PasswordServices()
        {
            _passwordHasher = new PasswordHasher<string>();
        }

        // Hash password for storage
        public string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(null, password);
        }

        // Verify password for login
        public bool VerifyPassword(string hashedPassword, string enteredPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(null, hashedPassword, enteredPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}

