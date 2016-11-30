using System;
using System.Text;
using Xunit;

namespace Dangl.Common.Tests
{
    public class StringEncryptionTests
    {
        [Fact]
        public void EncryptDecrypt_RegularCharacters()
        {
            // Arrange
            var password = "1234ffddddddddf5666";
            var text = "Hello World!";

            // Act
            var encryptedString01 = StringEncryption.EncryptString(text, password);
            var encryptedString02 = StringEncryption.EncryptString(text, password);
            var decryptedString01 = StringEncryption.DecryptString(encryptedString01, password);
            var decryptedString02 = StringEncryption.DecryptString(encryptedString02, password);

            // Assert
            Assert.Equal(text, decryptedString01); // Decrypted string should match original string
            Assert.Equal(text, decryptedString02); // Decrypted string should match original string
            Assert.NotEqual(text, encryptedString01); // Encrypted string should not match original string
            Assert.NotEqual(encryptedString01, encryptedString02); // String should never be encrypted the same twice
        }

        [Fact]
        public void EncryptDecrypt_EmptyPassword()
        {
            // Arrange
            var text = "Hello World!";

            // Act
            var encryptedString01 = StringEncryption.EncryptString(text, string.Empty);
            var encryptedString02 = StringEncryption.EncryptString(text, string.Empty);
            var decryptedString01 = StringEncryption.DecryptString(encryptedString01, string.Empty);
            var decryptedString02 = StringEncryption.DecryptString(encryptedString02, string.Empty);

            // Assert
            Assert.Equal(text, decryptedString01); // Decrypted string should match original string
            Assert.Equal(text, decryptedString02); // Decrypted string should match original string
            Assert.NotEqual(text, encryptedString01); // Encrypted string should not match original string
            Assert.NotEqual(encryptedString01, encryptedString02); // String should never be encrypted the same twice
        }

        [Fact]
        public void EncryptDecrypt_EmptyMessage()
        {
            // Arrange
            var password = "1234ffddddddddf5666";
            var text = string.Empty;

            // Act
            var encryptedString01 = StringEncryption.EncryptString(text, password);
            var encryptedString02 = StringEncryption.EncryptString(text, password);
            var decryptedString01 = StringEncryption.DecryptString(encryptedString01, password);
            var decryptedString02 = StringEncryption.DecryptString(encryptedString02, password);

            // Assert
            Assert.Equal(text, decryptedString01); // Decrypted string should match original string
            Assert.Equal(text, decryptedString02); // Decrypted string should match original string
            Assert.NotEqual(text, encryptedString01); // Encrypted string should not match original string
            Assert.NotEqual(encryptedString01, encryptedString02); // String should never be encrypted the same twice
        }

        [Fact]
        public void EncryptTwiceWithSamePassword_DifferentRepresentation()
        {
            var password = "SomePassword";
            var text = "Hello World!";
            var encryptedString01 = StringEncryption.EncryptString(text, password);
            var encryptedString02 = StringEncryption.EncryptString(text, password);
            Assert.NotEqual(encryptedString01, encryptedString02);
        }

        [Fact]
        public void EncryptTwiceWithEmptyPassword_DifferentRepresentation()
        {
            var text = "Hello World!";
            var encryptedString01 = StringEncryption.EncryptString(text, string.Empty);
            var encryptedString02 = StringEncryption.EncryptString(text, string.Empty);
            Assert.NotEqual(encryptedString01, encryptedString02);
        }

        [Fact]
        public void CanNotDecryptWithPasswordThatStartsWithSameCharactersButIsDifferent()
        {
            var password_01 = "fahjdsfadsf987hdsafbd6fbasd90fvb6ads0fvb6das98fbsad0fvbads09fvbß9vbf6absdf6";
            var password_02 = "fahjdsfadsf987hdsafbd6fbasd90fvb6ads0fvb6das98fbsad0fvbads09fvbß9vbf6absdf";
            var text = "Hello World!";
            var encryptedString = StringEncryption.EncryptString(text, password_01);
            var decryptedString = StringEncryption.DecryptString(encryptedString, password_02);
            Assert.NotEqual(text, decryptedString);
        }

        [Fact]
        public void CanNotDecryptWithInvalidPassword()
        {
            var password = "some pass";
            var text = "Hello World!";
            var encryptedString = StringEncryption.EncryptString(text, password);
            var invalidDecryptedString = StringEncryption.DecryptString(encryptedString, Guid.NewGuid().ToString());
            Assert.NotEqual(text, invalidDecryptedString);
        }

        [Fact]
        public void InvalidKeyDoesNotThrowExceptionDuringDeserialization()
        {
            var password = "some pass";
            var text = "Hello World!";
            var encryptedString = StringEncryption.EncryptString(text, password);
            for (int i = 0; i < 50; i++)
            {
                var invalidDecryptedString = StringEncryption.DecryptString(encryptedString, Guid.NewGuid().ToString());
                Assert.NotEqual(text, invalidDecryptedString);
            }
        }


        [Fact]
        public void EncryptDecrypt_HighUnicodeCharacters()
        {
            // Arrange
            var password = "1234ffdddddddd\u26A1f5666";
            var textToEncrypt = "Hello\u26A1\u26A1\u26A1\u26A1\u26A1 World!";

            // Act
            var encryptedString01 = StringEncryption.EncryptString(textToEncrypt, password);
            var encryptedString02 = StringEncryption.EncryptString(textToEncrypt, password);
            var decryptedString01 = StringEncryption.DecryptString(encryptedString01, password);
            var decryptedString02 = StringEncryption.DecryptString(encryptedString02, password);

            // Assert
            Assert.Equal(textToEncrypt, decryptedString01); // Decrypted string should match original string
            Assert.Equal(textToEncrypt, decryptedString02); // Decrypted string should match original string
            Assert.NotEqual(textToEncrypt, encryptedString01); // Encrypted string should not match original string
            Assert.NotEqual(encryptedString01, encryptedString02); // String should never be encrypted the same twice
        }

        [Fact]
        public void ArgumentNullExceptionOnNullText_Encrypt()
        {
            string inputText = null;
            string password = "Hello Password!";
            Assert.Throws(typeof(ArgumentNullException), () => StringEncryption.EncryptString(inputText, password));
        }

        [Fact]
        public void NoArgumentNullExceptionOnEmptyText_Encrypt()
        {
            string inputText = string.Empty;
            string password = "Hello Password!";
            var result = StringEncryption.EncryptString(inputText, password);
            Assert.False(string.IsNullOrWhiteSpace(result));
            Assert.NotEqual(inputText, result);
        }

        [Fact]
        public void ArgumentNullExceptionOnNullPassword_Encrypt()
        {
            string inputText = "Hello World";
            string password = null;
            Assert.Throws(typeof(ArgumentNullException), () => StringEncryption.EncryptString(inputText, password));
        }

        [Fact]
        public void NoArgumentNullExceptionOnEmptyPassword_Encrypt()
        {
            string inputText = "Hello World";
            string password = string.Empty;
            var result =  StringEncryption.EncryptString(inputText, password);
            Assert.False(string.IsNullOrWhiteSpace(result));
            Assert.NotEqual(inputText, result);
        }

        [Fact]
        public void ArgumentNullExceptionOnNullTextAndPassword_Encrypt()
        {
            string inputText = null;
            string password = null;
            Assert.Throws(typeof(ArgumentNullException), () => StringEncryption.EncryptString(inputText, password));
        }

        [Fact]
        public void NoArgumentNullExceptionOnEmptyTextAndPassword_Encrypt()
        {
            string inputText = string.Empty;
            string password = string.Empty;
            var result = StringEncryption.EncryptString(inputText, password);
            Assert.False(string.IsNullOrWhiteSpace(result));
            Assert.NotEqual(inputText, result);
        }

        [Fact]
        public void ArgumentNullExceptionOnNullText_Decrypt()
        {
            string inputText = null;
            string password = "Hello Password!";
            Assert.Throws(typeof(ArgumentNullException), () => StringEncryption.DecryptString(inputText, password));
        }

        [Fact]
        public void ArgumentNullExceptionOnEmptyText_Decrypt()
        {
            string inputText = string.Empty;
            string password = "Hello Password!";
            Assert.Throws(typeof(ArgumentNullException), () => StringEncryption.DecryptString(inputText, password));
        }

        [Fact]
        public void ArgumentNullExceptionOnNullPassword_Decrypt()
        {
            string inputText = "Hello World!";
            string password = null;
            Assert.Throws(typeof(ArgumentNullException), () => StringEncryption.DecryptString(inputText, password));
        }

        [Fact]
        public void NoArgumentNullExceptionOnEmptyPassword_Decrypt()
        {
            string password = string.Empty;
            string inputText = StringEncryption.EncryptString("Hello World!", password);
            var result =  StringEncryption.DecryptString(inputText, password);
            Assert.False(string.IsNullOrWhiteSpace(result));
            Assert.NotEqual(inputText, result);
            Assert.Equal("Hello World!", result);
        }

        [Fact]
        public void ArgumentNullExceptionOnNullTextAndPassword_Decrypt()
        {
            string inputText = null;
            string password = null;
            Assert.Throws(typeof(ArgumentNullException), () => StringEncryption.DecryptString(inputText, password));
        }

        [Fact]
        public void ArgumentNullExceptionOnEmptyTextAndPassword_Decrypt()
        {
            string inputText = string.Empty;
            string password = string.Empty;
            Assert.Throws(typeof(ArgumentNullException), () => StringEncryption.DecryptString(inputText, password));
        }

        [Fact]
        public void FormatException_IVTooShort()
        {
            var fabricatedDecryptedText = "2DEC9C6991943FCC4CEDE77173AF229E149ACF1877F1F2DD0BE332CD43429418:0DD41EE10F3FE2EA9328C898F03F231:H8LR+VN7diSBZeylsYVVGw==";
            var password = "Hello Password!";
            Assert.Throws(typeof(FormatException), () => StringEncryption.DecryptString(fabricatedDecryptedText, password));
        }

        [Fact]
        public void FormatException_IVTooLong()
        {
            var fabricatedDecryptedText = "2DEC9C6991943FCC4CEDE77173AF229E149ACF1877F1F2DD0BE332CD43429418:760DD41EE10F3FE2EA9328C898F03F231:H8LR+VN7diSBZeylsYVVGw==";
            var password = "Hello Password!";
            Assert.Throws(typeof(FormatException), () => StringEncryption.DecryptString(fabricatedDecryptedText, password));
        }

        [Fact]
        public void FormatException_SaltTooShort()
        {
            var fabricatedDecryptedText = "DEC9C6991943FCC4CEDE77173AF229E149ACF1877F1F2DD0BE332CD43429418:60DD41EE10F3FE2EA9328C898F03F231:H8LR+VN7diSBZeylsYVVGw==";
            var password = "Hello Password!";
            Assert.Throws(typeof(FormatException), () => StringEncryption.DecryptString(fabricatedDecryptedText, password));
        }

        [Fact]
        public void FormatException_SaltTooLong()
        {
            var fabricatedDecryptedText = "22DEC9C6991943FCC4CEDE77173AF229E149ACF1877F1F2DD0BE332CD43429418:60DD41EE10F3FE2EA9328C898F03F231:H8LR+VN7diSBZeylsYVVGw==";
            var password = "Hello Password!";
            Assert.Throws(typeof(FormatException), () => StringEncryption.DecryptString(fabricatedDecryptedText, password));
        }

        [Fact]
        public void FormatException_EncryptedPartNotBase64()
        {
            var fabricatedDecryptedText = "2DEC9C6991943FCC4CEDE77173AF229E149ACF1877F1F2DD0BE332CD43429418:60DD41EE10F3FE2EA93ü8C898F03F231:H8LR+VN7diSBZeylsYVVGw==";
            var password = "Hello Password!";
            Assert.Throws(typeof(FormatException), () => StringEncryption.DecryptString(fabricatedDecryptedText, password));
        }

        [Fact]
        public void FormatException_TooFewSegments()
        {
            var fabricatedDecryptedText = "2DEC9C6991943FCC4CEDE77173AF229E149ACF1877F1F2DD0BE332CD43429418:H8LR+VN7diSBZeylsYVVGw==";
            var password = "Hello Password!";
            Assert.Throws(typeof(FormatException), () => StringEncryption.DecryptString(fabricatedDecryptedText, password));
        }

        [Fact]
        public void FormatException_TooManySegments()
        {
            var fabricatedDecryptedText = "2DEC9C6991943FCC4CEDE77173AF229E149ACF1877F1F2DD0BE332CD43429418:60DD41EE10F3FE2EA9328C898F03F231:43:H8LR+VN7diSBZeylsYVVGw==";
            var password = "Hello Password!";
            Assert.Throws(typeof(FormatException), () => StringEncryption.DecryptString(fabricatedDecryptedText, password));
        }
    }
}