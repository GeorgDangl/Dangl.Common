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
    public static class StringEncryptionExtensions
    {
        private const int KEY_SIZE_BITS = 256;
        private const int SALT_SIZE_BYTES = 32;
        private const int PBKDF2_ITERATIONS = 1000;

        /// <summary>
        /// Returns the encrypted string.
        /// </summary>
        /// <param name="plainText">May not be null, empty or only whitespace</param>
        /// <param name="password">May not be null, empty or only whitespace</param>
        /// <param name="pbkdf2Iterations">Iterations to use in the PBKDF2 password bytes derivation algorithm</param>
        /// <returns></returns>
        public static string Encrypt(this string plainText, string password, int pbkdf2Iterations = PBKDF2_ITERATIONS)
        {
            if (plainText == null)
            {
                throw new ArgumentNullException(nameof(plainText));
            }
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }
            if (pbkdf2Iterations <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pbkdf2Iterations), "The count of PBKDF2 iterations must be bigger than zero");
            }
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            var saltBytes = new byte[SALT_SIZE_BYTES];
            var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(saltBytes);
            var passwordBytes = new Rfc2898DeriveBytes(password, saltBytes, pbkdf2Iterations).GetBytes(KEY_SIZE_BITS / 8);
            var aes = Aes.Create();
            System.Diagnostics.Debug.Assert(aes != null);
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
                    var salt = BitConverter.ToString(saltBytes).Replace("-", string.Empty);
                    var iv = BitConverter.ToString(initVectorBytes).Replace("-", string.Empty);
                    var cipherText = Convert.ToBase64String(cipherTextBytes);
                    return $"{pbkdf2Iterations}:{salt}:{iv}:{cipherText}";
                }
            }
        }

        /// <summary>
        /// Will decrypt a string given the encrypted text and a password.
        /// </summary>
        /// <param name="encryptedText">May not be null, empty or only whitespace</param>
        /// <param name="password">May not be null, empty or only whitespace</param>
        /// <returns></returns>
        public static string DecryptString(this string encryptedText, string password)
        {
            if (string.IsNullOrWhiteSpace(encryptedText))
            {
                throw new ArgumentNullException(nameof(encryptedText));
            }
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }
            return DecryptString(encryptedText, password, false);
        }

        private static string DecryptString(string encryptedText, string password, bool useZeroPadding)
        {
            var splitText = encryptedText.Split(':');
            if (splitText.Length == 3)
            {
                try
                {
                    return DecryptOldVersionWithoutIterationCount(encryptedText, password);
                }
                catch { /* Do nothing to propagate failure */ }
            }
            if (splitText.Length != 4)
            {
                throw new FormatException("Expecting the decrypted text to be in the form of \"pbkdf2_iterations:salt_hex:iv_hex:text_base64\"");
            }
            if (!int.TryParse(splitText[0], out var pbkdf2Iterations) || pbkdf2Iterations <= 0)
            {
                throw new InvalidDataException($"The first segment is expected to be a positive integer, was: {splitText[0]}");
            }
            var saltRaw = splitText[1];
            if (saltRaw.Length != SALT_SIZE_BYTES * 2)
            {
                throw new FormatException("Expecting the salt to be " + SALT_SIZE_BYTES + " bytes");
            }
            var keyRaw = splitText[2];
            if (keyRaw.Length != KEY_SIZE_BITS / 8)
            {
                throw new FormatException("Expecting the IV to be " + (KEY_SIZE_BITS / (8 * 2)) + " bytes");
            }
            var encryptedRaw = splitText[3];
            var encryptedTextBytes = Convert.FromBase64String(encryptedRaw);
            using (var memoryStream = new MemoryStream(encryptedTextBytes))
            {
                memoryStream.Position = 0;
                var saltBytes = StringToByteArray(saltRaw);
                var initVectorBytes = StringToByteArray(keyRaw);
                var passwordBytes = new Rfc2898DeriveBytes(password, saltBytes, Convert.ToInt32(pbkdf2Iterations)).GetBytes(KEY_SIZE_BITS / 8);
                var aes = Aes.Create();
                System.Diagnostics.Debug.Assert(aes != null);
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
                        var plainTextBytes = new byte[encryptedTextBytes.Length];
                        // For the actual reading, please see below:
                        // https://stackoverflow.com/questions/70933327/net-6-failing-at-decompress-large-gzip-text
                        // There was an issue discovered with a Dangl.AVA project in JSON format, which
                        // surfaced when tests that worked fine in .NET 5 failed with .NET 6. The errorenous
                        // decompression occured during an embedded image in base64 format.
                        int totalRead = 0;
                        while (totalRead < plainTextBytes.Length)
                        {
                            int bytesRead = cryptoStream.Read(plainTextBytes, totalRead, plainTextBytes.Length - totalRead);
                            if (bytesRead == 0)
                            {
                                break;
                            }
                            totalRead += bytesRead;
                        }

                        return Encoding.UTF8.GetString(plainTextBytes, 0, totalRead);
                    }
                }
                catch (CryptographicException)
                {
                    return DecryptString(encryptedText, password, true);
                }
            }
        }

        private static string DecryptOldVersionWithoutIterationCount(string encryptedText, string password)
        {
            // Old versions do use a PBDKF2 iteration count of 1000 but don't append this to the output
            var encryptedTextOldVersion = $"1000:{encryptedText}";
            return DecryptString(encryptedTextOldVersion, password, false);
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
