namespace TMS.Infrastructure.Helpers
{
    public class PasswordHelper
    {
        public static string HashPassword(string plainText)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainText);
        }

        public static bool VerifyPassword(string plainText, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(plainText, hash);
        }
    }
}
