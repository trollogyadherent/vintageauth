using System;
using System.Security.Cryptography;
using System.Text;

namespace VintageAuth {
    public class PasswordUtil {
        public static string getSalt() {
            byte[] salt = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }
        public static string getHash(string password, string salt) {
            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashedBytes = new byte[saltBytes.Length + passwordBytes.Length];

            Buffer.BlockCopy(saltBytes, 0, hashedBytes, 0, saltBytes.Length);
            Buffer.BlockCopy(passwordBytes, 0, hashedBytes, saltBytes.Length, passwordBytes.Length);

            using (var sha256 = SHA256.Create()) {
                byte[] hash = sha256.ComputeHash(hashedBytes);
                return Convert.ToBase64String(hash);
            }
        }

        public static string GenerateSecureHash(int length) {
        // Generate a cryptographically secure random salt
        byte[] salt = new byte[length];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        // Convert the salt to a hex string
        string saltString = BitConverter.ToString(salt).Replace("-", "").ToLower();
        // Return the salt
        return saltString;
    }
    }
}