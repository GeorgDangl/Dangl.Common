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
        private readonly Random _random;
        private readonly byte[] _key;
        private readonly Aes _aes = Aes.Create();
        private readonly UTF8Encoding _encoder;
        private const int VectorSize = 16;

        /// <summary>
        /// Basic constructor, requires the password.
        /// </summary>
        /// <param name="password">Must be between 16 and 32 characters. The characters must map to 16 to 32 bytes, so high unicode characters might count as up to 4.</param>
        public StringEncryption(string password)
        {
            if (password.Length > 32 || password.Length < 16)
            {
                throw new ArgumentOutOfRangeException(nameof(password), nameof(password) + " must be between 16 and 32 characters.");
            }
            var bytesFromPassword = Encoding.UTF8.GetBytes(password);
            if (bytesFromPassword.Length > 32)
            {
                throw new ArgumentOutOfRangeException(nameof(password), nameof(password) + " length is valid, but UTF-8 bytes of password exceed allowed amount of 32 bytes. This may be due to high unicode characters needing as many as 4 bytes.");
            }
            bytesFromPassword = bytesFromPassword.Length == 32
                ? bytesFromPassword
                : bytesFromPassword.Concat(new byte[32 - bytesFromPassword.Length]).ToArray();
            _random = new Random();
            //System.Security.Cryptography.Aes.Create();
            //RijndaelManagedInstance = new RijndaelManaged();
            _encoder = new UTF8Encoding();
            _key= bytesFromPassword;
        }

        /// <summary>
        /// Returns the encrypted string.
        /// </summary>
        /// <param name="plainText">The string to encrypt.</param>
        /// <returns></returns>
        public string Encrypt(string plainText)
        {
            var vector = new byte[VectorSize];
            _random.NextBytes(vector);
            var cryptogram = vector.Concat(Encrypt(_encoder.GetBytes(plainText), vector));
            return Convert.ToBase64String(cryptogram.ToArray());
        }

        /// <summary>
        /// Returns a decrypted string.
        /// </summary>
        /// <param name="encryptedText">The encrypted string.</param>
        /// <returns></returns>
        public string Decrypt(string encryptedText)
        {
            var cryptogram = Convert.FromBase64String(encryptedText);
            if (cryptogram.Length < VectorSize + 1)
            {
                throw new ArgumentException("Not a valid encrypted string", nameof(encryptedText));
            }

            var vector = cryptogram.Take(VectorSize).ToArray();
            var buffer = cryptogram.Skip(VectorSize).ToArray();
            return _encoder.GetString(Decrypt(buffer, vector));
        }

        private byte[] Encrypt(byte[] buffer, byte[] vector)
        {
            var encryptor = _aes.CreateEncryptor(_key, vector);
            return Transform(buffer, encryptor);
        }

        private byte[] Decrypt(byte[] buffer, byte[] vector)
        {
            var decryptor = _aes.CreateDecryptor(_key, vector);
            return Transform(buffer, decryptor);
        }

        private byte[] Transform(byte[] buffer, ICryptoTransform transform)
        {
            var stream = new MemoryStream();
            using (var cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Write))
            {
                cryptoStream.Write(buffer, 0, buffer.Length);
            }
            return stream.ToArray();
        }
    }
}
