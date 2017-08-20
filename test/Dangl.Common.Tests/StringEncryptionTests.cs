using System;
using System.IO;
using Xunit;

namespace Dangl.Common.Tests
{
    public class StringEncryptionTests
    {
        [Fact]
        public void DecryptKnownStringOfPreviousVersion()
        {
            var encryptedPassword = "98499D68A1DBB303EBD77F814CC178E95DAA323BD71DC3D942627A149DEB9A51:189BB9A021F5040C9B8572C7C2933248:etyJjoHnziStmEVedWM0iQ==";
            var password = "P4$$w0|2|)";
            var expectedPlaintext = "Hello World!";
            var actualPlaintext = StringEncryption.DecryptString(encryptedPassword, password);
            Assert.Equal(expectedPlaintext, actualPlaintext);
        }

        [Fact]
        public void DecryptKnownString()
        {
            var encryptedPassword = "1000:98499D68A1DBB303EBD77F814CC178E95DAA323BD71DC3D942627A149DEB9A51:189BB9A021F5040C9B8572C7C2933248:etyJjoHnziStmEVedWM0iQ==";
            var password = "P4$$w0|2|)";
            var expectedPlaintext = "Hello World!";
            var actualPlaintext = StringEncryption.DecryptString(encryptedPassword, password);
            Assert.Equal(expectedPlaintext, actualPlaintext);
        }

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
            var password = "P4$$w0|2|)";
            var text = "Hello World!";
            var encryptedString01 = StringEncryption.EncryptString(text, password);
            var encryptedString02 = StringEncryption.EncryptString(text, password);
            Assert.NotEqual(encryptedString01, encryptedString02);
        }

        [Fact]
        public void EncryptTwiceWithSamePassword_DifferentRepresentationWithCustomPbkdf2IterationCount()
        {
            var password = "P4$$w0|2|)";
            var text = "Hello World!";
            var encryptedString01 = StringEncryption.EncryptString(text, password, 50);
            var encryptedString02 = StringEncryption.EncryptString(text, password, 50);
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
        public void EncryptTwiceWithEmptyPassword_DifferentRepresentationWithCustomPbkdf2IterationCount()
        {
            var text = "Hello World!";
            var encryptedString01 = StringEncryption.EncryptString(text, string.Empty, 50);
            var encryptedString02 = StringEncryption.EncryptString(text, string.Empty, 50);
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
            var fabricatedDecryptedText = "1000:2DEC9C6991943FCC4CEDE77173AF229E149ACF1877F1F2DD0BE332CD43429418:0DD41EE10F3FE2EA9328C898F03F231:H8LR+VN7diSBZeylsYVVGw==";
            var password = "Hello Password!";
            Assert.Throws(typeof(FormatException), () => StringEncryption.DecryptString(fabricatedDecryptedText, password));
        }

        [Fact]
        public void FormatException_IVTooLong()
        {
            var fabricatedDecryptedText = "1000:2DEC9C6991943FCC4CEDE77173AF229E149ACF1877F1F2DD0BE332CD43429418:760DD41EE10F3FE2EA9328C898F03F231:H8LR+VN7diSBZeylsYVVGw==";
            var password = "Hello Password!";
            Assert.Throws(typeof(FormatException), () => StringEncryption.DecryptString(fabricatedDecryptedText, password));
        }

        [Fact]
        public void FormatException_SaltTooShort()
        {
            var fabricatedDecryptedText = "1000:DEC9C6991943FCC4CEDE77173AF229E149ACF1877F1F2DD0BE332CD43429418:60DD41EE10F3FE2EA9328C898F03F231:H8LR+VN7diSBZeylsYVVGw==";
            var password = "Hello Password!";
            Assert.Throws(typeof(FormatException), () => StringEncryption.DecryptString(fabricatedDecryptedText, password));
        }

        [Fact]
        public void FormatException_SaltTooLong()
        {
            var fabricatedDecryptedText = "1000:22DEC9C6991943FCC4CEDE77173AF229E149ACF1877F1F2DD0BE332CD43429418:60DD41EE10F3FE2EA9328C898F03F231:H8LR+VN7diSBZeylsYVVGw==";
            var password = "Hello Password!";
            Assert.Throws(typeof(FormatException), () => StringEncryption.DecryptString(fabricatedDecryptedText, password));
        }

        [Fact]
        public void FormatException_EncryptedPartNotBase64_1()
        {
            var fabricatedDecryptedText = "1000:2DEC9C6991943FCC4CEDE77173AF229E149ACF1877F1F2DD0BE332CD43429418:189BB9A021F5040C9B8572C7C2933248:H8LR+VN7diSBZeylsYVVGw===";
            var password = "Hello Password!";
            Assert.Throws(typeof(FormatException), () => StringEncryption.DecryptString(fabricatedDecryptedText, password));
        }

        [Fact]
        public void FormatException_EncryptedPartNotBase64_2()
        {
            var fabricatedDecryptedText = "1000:2DEC9C6991943FCC4CEDE77173AF229E149ACF1877F1F2DD0BE332CD43429418:189BB9A021F5040C9B8572C7C2933248:bob@example.com";
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
            var fabricatedDecryptedText = "1000:2DEC9C6991943FCC4CEDE77173AF229E149ACF1877F1F2DD0BE332CD43429418:60DD41EE10F3FE2EA9328C898F03F231:43:H8LR+VN7diSBZeylsYVVGw==";
            var password = "Hello Password!";
            Assert.Throws(typeof(FormatException), () => StringEncryption.DecryptString(fabricatedDecryptedText, password));
        }

        [Fact]
        public void ArgumentOutOfRangeExceptionOnNegativePbkdf2Iterations()
        {
            Assert.Throws<ArgumentOutOfRangeException>("pbkdf2Iterations", () => StringEncryption.EncryptString("Hello World!", "Password", -2));
        }

        [Fact]
        public void ArgumentOutOfRangeExceptionOnZeroPbkdf2Iterations()
        {
            Assert.Throws<ArgumentOutOfRangeException>("pbkdf2Iterations", () => StringEncryption.EncryptString("Hello World!", "Password", 0));
        }

        [Fact]
        public void CanEncryptAndDecryptWithCustomPbkdf2Iterations()
        {
            var plainText = "Hello world!";
            var password = "Password";
            var encryptedText = StringEncryption.EncryptString(plainText, password, 39);
            var decryptedText = StringEncryption.DecryptString(encryptedText, password);
            Assert.Equal(plainText, decryptedText);
        }

        [Fact]
        public void EncodesPbkdf2IterationsInOutput()
        {
            var encryptedText = StringEncryption.EncryptString("Hello world!", "Password");
            Assert.StartsWith("1000:", encryptedText);
        }

        [Fact]
        public void EncodesPbkdf2IterationsInOutputWithCustomIterationCount()
        {
            var encryptedText = StringEncryption.EncryptString("Hello world!", "Password", 50);
            Assert.StartsWith("50:", encryptedText);
        }

        [Fact]
        public void InvalidDataExceptionWhenGivenPbkdf2IterationsNoIntegerButReal()
        {
            var encryptedPassword = "10.2:98499D68A1DBB303EBD77F814CC178E95DAA323BD71DC3D942627A149DEB9A51:189BB9A021F5040C9B8572C7C2933248:etyJjoHnziStmEVedWM0iQ==";
            var password = "P4$$w0|2|)";
            Assert.Throws<InvalidDataException>(() => StringEncryption.DecryptString(encryptedPassword, password));
        }

        [Fact]
        public void InvalidDataExceptionWhenGivenPbkdf2IterationsNoIntegerButString()
        {
            var encryptedPassword = "Hello:98499D68A1DBB303EBD77F814CC178E95DAA323BD71DC3D942627A149DEB9A51:189BB9A021F5040C9B8572C7C2933248:etyJjoHnziStmEVedWM0iQ==";
            var password = "P4$$w0|2|)";
            Assert.Throws<InvalidDataException>(() => StringEncryption.DecryptString(encryptedPassword, password));
        }

        [Fact]
        public void InvalidDataExceptionWhenGivenPbkdf2IterationsNegative()
        {
            var encryptedPassword = "-200:98499D68A1DBB303EBD77F814CC178E95DAA323BD71DC3D942627A149DEB9A51:189BB9A021F5040C9B8572C7C2933248:etyJjoHnziStmEVedWM0iQ==";
            var password = "P4$$w0|2|)";
            Assert.Throws<InvalidDataException>(() => StringEncryption.DecryptString(encryptedPassword, password));
        }

        [Fact]
        public void InvalidDataExceptionWhenGivenPbkdf2IterationsZero()
        {
            var encryptedPassword = "0:98499D68A1DBB303EBD77F814CC178E95DAA323BD71DC3D942627A149DEB9A51:189BB9A021F5040C9B8572C7C2933248:etyJjoHnziStmEVedWM0iQ==";
            var password = "P4$$w0|2|)";
            Assert.Throws<InvalidDataException>(() => StringEncryption.DecryptString(encryptedPassword, password));
        }
    }
}
