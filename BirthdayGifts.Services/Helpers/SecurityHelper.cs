using System.Security.Cryptography;
using System.Text;

namespace BirthdayGifts.Services.Helpers
{
    public static class SecurityHelper
    {
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public static bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            if (string.IsNullOrEmpty(plainPassword) || string.IsNullOrEmpty(hashedPassword))
            {
                return false;
            }

            var hashedInput = HashPassword(plainPassword);
            return hashedInput.Equals(hashedPassword, StringComparison.OrdinalIgnoreCase);
        }
    }
}