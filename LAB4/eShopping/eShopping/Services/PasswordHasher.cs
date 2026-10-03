using System.Security.Cryptography;
using System.Text;

namespace eShopping.Services
{
    public static class PasswordHasher
    {
        public static string Hash(string plain)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(plain ?? ""));
                var sb = new StringBuilder();
                foreach (var b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}