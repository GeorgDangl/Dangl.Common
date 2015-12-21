using System;
using Xunit;

namespace Dangl.Common.Tests
{
     
    public class StringEncryptionTest
    {
        [Fact]
        public void Fail_PasswordTooShort()
        {
            Assert.Throws(typeof (ArgumentOutOfRangeException), () =>
            {
                var Instance = new StringEncryption("0123456789ABCDE");
            });
        }
        [Fact]
        public void Fail_PasswordTooLong()
        {
            Assert.Throws(typeof(ArgumentOutOfRangeException), () =>
            {
                var Instance = new StringEncryption("0123456789ABCDEF0123456789ABCDEF_");
            });
        }
        [Fact]
        public void EncryptDecrypt_RegularCharacters()
        {
            // Arrange
            var Instance = new StringEncryption("1234ffddddddddf5666");
            var TextToEncrypt = "Hello World!";

            // Act
            var EncryptedString_01 = Instance.Encrypt(TextToEncrypt);
            var EncryptedString_02 = Instance.Encrypt(TextToEncrypt);
            var DecryptedString_01 = Instance.Decrypt(EncryptedString_01);
            var DecryptedString_02 = Instance.Decrypt(EncryptedString_02);

            // Assert
            Assert.Equal(TextToEncrypt, DecryptedString_01); // Decrypted string should match original string
            Assert.Equal(TextToEncrypt, DecryptedString_02); // Decrypted string should match original string
            Assert.NotEqual(TextToEncrypt, EncryptedString_01); // Encrypted string should not match original string
            Assert.NotEqual(EncryptedString_01, EncryptedString_02); // String should never be encrypted the same twice
        }
        [Fact]
        public void EncryptDecrypt_HighUnicodeCharacters()
        {
            // Arrange
            var Instance = new StringEncryption("1234ffdddddddd\u26A1f5666");
            var TextToEncrypt = "Hello\u26A1\u26A1\u26A1\u26A1\u26A1 World!";

            // Act
            var EncryptedString_01 = Instance.Encrypt(TextToEncrypt);
            var EncryptedString_02 = Instance.Encrypt(TextToEncrypt);
            var DecryptedString_01 = Instance.Decrypt(EncryptedString_01);
            var DecryptedString_02 = Instance.Decrypt(EncryptedString_02);

            // Assert
            Assert.Equal(TextToEncrypt, DecryptedString_01); // Decrypted string should match original string
            Assert.Equal(TextToEncrypt, DecryptedString_02); // Decrypted string should match original string
            Assert.NotEqual(TextToEncrypt, EncryptedString_01); // Encrypted string should not match original string
            Assert.NotEqual(EncryptedString_01, EncryptedString_02); // String should never be encrypted the same twice
        }
        [Fact]
        public void EncryptDecrypt_HighUnicodeCharacters_ThrowsException()
        {
            // Arrange
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new StringEncryption("\u26A1\u26A1\u26A1\u26A1\u26A1\u26A1\u26A1\u26A1\u26A1"));
        }
    }
}
