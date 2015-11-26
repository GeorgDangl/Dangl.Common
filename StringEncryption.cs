using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dangl
{
    /// <summary>
    /// Taken from: http://stackoverflow.com/questions/165808/simple-two-way-encryption-for-c-sharp/26177005#26177005
    /// </summary>
    public class StringEncryption
    {
        private readonly Random Random;
        private readonly byte[] Key;
        private readonly RijndaelManaged RijndaelManagedInstance;
        private readonly UTF8Encoding Encoder;
        private const int VectorSize = 16;

        public StringEncryption(string Password)
        {
            if (Password.Length > 32 || Password.Length < 16)
            {
                throw new ArgumentOutOfRangeException(nameof(Password), nameof(Password) + " must be between 16 and 32 characters.");
            }
            var BytesFromPassword = Encoding.UTF8.GetBytes(Password);
            if (BytesFromPassword.Length > 32)
            {
                throw new ArgumentOutOfRangeException(nameof(Password), nameof(Password) + " length is valid, but UTF-8 bytes of password exceed allowed amount of 32 bytes. This may be due to high unicode characters needing as many as 4 bytes.");
            }
            BytesFromPassword = BytesFromPassword.Length == 32
                ? BytesFromPassword
                : BytesFromPassword.Concat(new byte[32 - BytesFromPassword.Length]).ToArray();
            Random = new Random();
            RijndaelManagedInstance = new RijndaelManaged();
            Encoder = new UTF8Encoding();
            Key= BytesFromPassword;
        }

        public string Encrypt(string PlainText)
        {
            var Vector = new byte[VectorSize];
            Random.NextBytes(Vector);
            var Cryptogram = Vector.Concat(Encrypt(Encoder.GetBytes(PlainText), Vector));
            return Convert.ToBase64String(Cryptogram.ToArray());
        }

        public string Decrypt(string EncryptedText)
        {
            var Cryptogram = Convert.FromBase64String(EncryptedText);
            if (Cryptogram.Length < VectorSize + 1)
            {
                throw new ArgumentException("Not a valid encrypted string", nameof(EncryptedText));
            }

            var Vector = Cryptogram.Take(VectorSize).ToArray();
            var Buffer = Cryptogram.Skip(VectorSize).ToArray();
            return Encoder.GetString(Decrypt(Buffer, Vector));
        }

        private byte[] Encrypt(byte[] Buffer, byte[] Vector)
        {
            var Encryptor = RijndaelManagedInstance.CreateEncryptor(Key, Vector);
            return Transform(Buffer, Encryptor);
        }

        private byte[] Decrypt(byte[] Buffer, byte[] Vector)
        {
            var Decryptor = RijndaelManagedInstance.CreateDecryptor(Key, Vector);
            return Transform(Buffer, Decryptor);
        }

        private byte[] Transform(byte[] Buffer, ICryptoTransform Transform)
        {
            var Stream = new MemoryStream();
            using (var CryptoStream = new CryptoStream(Stream, Transform, CryptoStreamMode.Write))
            {
                CryptoStream.Write(Buffer, 0, Buffer.Length);
            }
            return Stream.ToArray();
        }
    }
}
