using System.Security.Cryptography;

namespace PMS.Lib {
    internal class Hash
    {
        internal static string GetHash(string password)
        {
            using SHA256 sha256Hash = SHA256.Create();
            var passwordHash = sha256Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(passwordHash).Replace("-", "").ToLower();
        }
    }
}