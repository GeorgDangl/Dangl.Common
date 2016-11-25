using System;
using System.Text;
using Xunit;

namespace Dangl.Common.Tests
{
    public class StringEncryptionTests
    {
        [Fact]
        public void Fail_PasswordTooShort()
        {
            Assert.Throws(typeof (ArgumentOutOfRangeException), () => { var instance = new StringEncryption("0123456789ABCDE"); });
        }

        [Fact]
        public void Fail_PasswordTooLong()
        {
            Assert.Throws(typeof (ArgumentOutOfRangeException), () => { var instance = new StringEncryption("0123456789ABCDEF0123456789ABCDEF_"); });
        }

        [Fact]
        public void Fail_PasswordTooLongDueToHighASCIICharacters()
        {
            var passwordInput = "0123456789012345678901234567891\u26A1";
            Assert.Equal(32, passwordInput.Length);
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => { var instance = new StringEncryption(passwordInput); });
        }

        [Fact]
        public void Initialization_32CharacterPassword()
        {
            var passwordInput = "01234567890123456789012345678912";
            var instance = new StringEncryption(passwordInput);
        }

        [Fact]
        public void EncryptDecrypt_RegularCharacters()
        {
            // Arrange
            var instance = new StringEncryption("1234ffddddddddf5666");
            var textToEncrypt = "Hello World!";

            // Act
            var encryptedString01 = instance.Encrypt(textToEncrypt);
            var encryptedString02 = instance.Encrypt(textToEncrypt);
            var decryptedString01 = instance.Decrypt(encryptedString01);
            var decryptedString02 = instance.Decrypt(encryptedString02);

            // Assert
            Assert.Equal(textToEncrypt, decryptedString01); // Decrypted string should match original string
            Assert.Equal(textToEncrypt, decryptedString02); // Decrypted string should match original string
            Assert.NotEqual(textToEncrypt, encryptedString01); // Encrypted string should not match original string
            Assert.NotEqual(encryptedString01, encryptedString02); // String should never be encrypted the same twice
        }

        [Fact]
        public void EncryptDecrypt_HighUnicodeCharacters()
        {
            // Arrange
            var instance = new StringEncryption("1234ffdddddddd\u26A1f5666");
            var textToEncrypt = "Hello\u26A1\u26A1\u26A1\u26A1\u26A1 World!";

            // Act
            var encryptedString01 = instance.Encrypt(textToEncrypt);
            var encryptedString02 = instance.Encrypt(textToEncrypt);
            var decryptedString01 = instance.Decrypt(encryptedString01);
            var decryptedString02 = instance.Decrypt(encryptedString02);

            // Assert
            Assert.Equal(textToEncrypt, decryptedString01); // Decrypted string should match original string
            Assert.Equal(textToEncrypt, decryptedString02); // Decrypted string should match original string
            Assert.NotEqual(textToEncrypt, encryptedString01); // Encrypted string should not match original string
            Assert.NotEqual(encryptedString01, encryptedString02); // String should never be encrypted the same twice
        }

        [Fact]
        public void EncryptDecrypt_HighUnicodeCharacters_ThrowsException()
        {
            // Arrange
            Assert.Throws(typeof (ArgumentOutOfRangeException), () => new StringEncryption("\u26A1\u26A1\u26A1\u26A1\u26A1\u26A1\u26A1\u26A1\u26A1"));
        }

        [Fact]
        public void Decrypt_ValidBase64StringButTooShort()
        {
            var input = Convert.ToBase64String(Encoding.UTF8.GetBytes("123456"));
            var encryptionInstance = new StringEncryption("1234567890123456");
            Assert.Throws(typeof (ArgumentException), () => encryptionInstance.Decrypt(input));
        }
    }
}