using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Dangl
{
    /// <summary>
    /// Taken from: http://stackoverflow.com/questions/165808/simple-two-way-encryption-for-c-sharp/26177005#26177005
    /// </summary>
    public class StringEncryption
    {
        private const int KEY_SIZE_BITS = 256;
        private const int SALT_SIZE_BYTES = 32;
        private const int PBKDF2_ITERATIONS = 1000;

        /// <summary>
        /// Returns the encrypted string.
        /// </summary>
        /// <param name="plainText">May not be null, empty or only whitespace</param>
        /// <param name="password">May not be null, empty or only whitespace</param>
        /// <returns></returns>
        public static string EncryptString(string plainText, string password)
        {
            if (string.IsNullOrWhiteSpace(plainText))
            {
                throw new ArgumentNullException(nameof(plainText));
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            var saltBytes = new byte[SALT_SIZE_BYTES];
            var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(saltBytes);
            var passwordBytes = new Rfc2898DeriveBytes(password, saltBytes, PBKDF2_ITERATIONS).GetBytes(KEY_SIZE_BITS / 8);
            var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Key = passwordBytes;
            aes.GenerateIV();
            var initVectorBytes = aes.IV;
            var aesEncryptor = aes.CreateEncryptor();
            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    var cipherTextBytes = memoryStream.ToArray();
                    return $"{BitConverter.ToString(saltBytes).Replace("-", string.Empty)}:{BitConverter.ToString(initVectorBytes).Replace("-", string.Empty)}:{Convert.ToBase64String(cipherTextBytes)}";
                }
            }
        }

        /// <summary>
        /// Will decrypt a string given the encrypted text and a password.
        /// </summary>
        /// <param name="encryptedText">May not be null, empty or only whitespace</param>
        /// <param name="password">May not be null, empty or only whitespace</param>
        /// <returns></returns>
        public static string DecryptString(string encryptedText, string password)
        {
            if (string.IsNullOrWhiteSpace(encryptedText))
            {
                throw new ArgumentNullException(nameof(encryptedText));
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password));
            }
            return DecryptString(encryptedText, password, false);
        }

        private static string DecryptString(string encryptedText, string password, bool useZeroPadding)
        {
            var splitText = encryptedText.Split(':');
            if (splitText.Length != 3)
            {
                throw new FormatException("Expecting the decrypted text to be in the form of \"salt_hex:iv_hex:text_base64\"");
            }
            if (splitText[0].Length != SALT_SIZE_BYTES * 2)
            {
                throw new FormatException("Expecting the salt to be " + SALT_SIZE_BYTES + " bytes");
            }
            if (splitText[1].Length != KEY_SIZE_BITS / 8)
            {
                throw new FormatException("Expecting the IV to be " + (KEY_SIZE_BITS / (8 * 2)) + " bytes");
            }

            var encryptedTextBytes = Convert.FromBase64String(splitText[2]);
            using (var memoryStream = new MemoryStream(encryptedTextBytes))
            {
                memoryStream.Position = 0;
                var saltBytes = StringToByteArray(splitText[0]);
                var initVectorBytes = StringToByteArray(splitText[1]);
                var passwordBytes = new Rfc2898DeriveBytes(password, saltBytes, PBKDF2_ITERATIONS).GetBytes(KEY_SIZE_BITS / 8);
                var aes = Aes.Create();
                aes.Mode = CipherMode.CBC;
                if (useZeroPadding)
                {
                    aes.Padding = PaddingMode.Zeros;
                }
                var aesDecryptor = aes.CreateDecryptor(passwordBytes, initVectorBytes);
                try
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Read))
                    {
                        byte[] plainTextBytes = new byte[encryptedTextBytes.Length];

                        int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                        return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

                    }
                }
                catch (CryptographicException)
                {
                    return DecryptString(encryptedText, password, true);
                }
            }
        }


        private static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }
    }
}
