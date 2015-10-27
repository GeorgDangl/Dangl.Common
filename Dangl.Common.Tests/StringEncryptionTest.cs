using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dangl.Common.Tests
{
    [TestClass]
    public class StringEncryptionTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Fail_PasswordTooShort()
        {
            var Instance = new StringEncryption("0123456789ABCDE");
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Fail_PasswordTooLong()
        {
            var Instance = new StringEncryption("0123456789ABCDEF0123456789ABCDEF_");
        }
        [TestMethod]
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
            Assert.AreEqual(TextToEncrypt, DecryptedString_01, "Decrypted string should match original string");
            Assert.AreEqual(TextToEncrypt, DecryptedString_02, "Decrypted string should match original string");
            Assert.AreNotEqual(TextToEncrypt, EncryptedString_01, "Encrypted string should not match original string");
            Assert.AreNotEqual(EncryptedString_01, EncryptedString_02, "String should never be encrypted the same twice");
        }
        [TestMethod]
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
            Assert.AreEqual(TextToEncrypt, DecryptedString_01, "Decrypted string should match original string");
            Assert.AreEqual(TextToEncrypt, DecryptedString_02, "Decrypted string should match original string");
            Assert.AreNotEqual(TextToEncrypt, EncryptedString_01, "Encrypted string should not match original string");
            Assert.AreNotEqual(EncryptedString_01, EncryptedString_02, "String should never be encrypted the same twice");
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void EncryptDecrypt_HighUnicodeCharacters_ThrowsException()
        {
            // Arrange
            var Instance = new StringEncryption("\u26A1\u26A1\u26A1\u26A1\u26A1\u26A1\u26A1\u26A1\u26A1");
            var TextToEncrypt = "Hello\u26A1\u26A1\u26A1\u26A1\u26A1 World!";

            // Act
            var EncryptedString_01 = Instance.Encrypt(TextToEncrypt);
            var EncryptedString_02 = Instance.Encrypt(TextToEncrypt);
            var DecryptedString_01 = Instance.Decrypt(EncryptedString_01);
            var DecryptedString_02 = Instance.Decrypt(EncryptedString_02);

            // Assert
            Assert.AreEqual(TextToEncrypt, DecryptedString_01, "Decrypted string should match original string");
            Assert.AreEqual(TextToEncrypt, DecryptedString_02, "Decrypted string should match original string");
            Assert.AreNotEqual(TextToEncrypt, EncryptedString_01, "Encrypted string should not match original string");
            Assert.AreNotEqual(EncryptedString_01, EncryptedString_02, "String should never be encrypted the same twice");
        }
    }
}
