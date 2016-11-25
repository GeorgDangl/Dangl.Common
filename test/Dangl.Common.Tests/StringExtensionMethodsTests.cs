using System;
using System.Text;
using Xunit;

namespace Dangl.Common.Tests
{
    public class StringExtensionMethodsTests
    {
        public class FromBase64
        {
            [Fact]
            public void InvalidInput_HtmlWithNonBase64Chars()
            {
                var input = "<html>Body</html>";
                Assert.Throws(typeof (FormatException), () => { input.FromBase64(); });
            }

            [Fact]
            public void InvalidInput_RegularStringNonBase64()
            {
                var input = "SomeString";
                Assert.Throws(typeof (FormatException), () => { input.FromBase64(); });
            }

            [Fact]
            public void Convert_01()
            {
                var input = "SGVsbG8gV29ybGQh";
                var expected = "Hello World!";
                var actual = input.FromBase64();
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void Convert_02()
            {
                var input = "PGh0bWw+Qm9keTwvaHRtbD4=";
                var expected = "<html>Body</html>";
                var actual = input.FromBase64();
                Assert.Equal(expected, actual);
            }
        }

        public class ToBase64
        {
            [Fact]
            public void Convert_01()
            {
                var input = "<html>Body</html>";
                var expected = "PGh0bWw+Qm9keTwvaHRtbD4=";
                var actual = input.ToBase64();
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void Convert_02()
            {
                var input = "<html>Body</html>";
                var expected = "PGh0bWw+Qm9keTwvaHRtbD4=";
                var actual = input.ToBase64();
                Assert.Equal(expected, actual);
            }
        }

        public class Sanitize
        {
            [Fact]
            public void SanitizeNullString()
            {
                string input = null;
                string expected = null;
                var actual = input.Sanitize();
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void Sanitize_01()
            {
                var input = string.Empty;
                var expected = string.Empty;
                var actual = input.Sanitize();
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void Sanitize_02()
            {
                var input = "Hello world!";
                var expected = "Hello world!";
                var actual = input.Sanitize();
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void Sanitize_03()
            {
                var input = "Hello world! " + " ";
                var expected = "Hello world!";
                var actual = input.Sanitize();
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void Sanitize_04()
            {
                var input = "Hello world!" + Environment.NewLine;
                var expected = "Hello world!";
                var actual = input.Sanitize();
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void Sanitize_05()
            {
                var input = "Hello world! " + " " + Environment.NewLine;
                var expected = "Hello world!";
                var actual = input.Sanitize();
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void Sanitize_06()
            {
                var input = "Hello world!" + " " + Environment.NewLine + " ";
                var expected = "Hello world!";
                var actual = input.Sanitize();
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void Sanitize_07()
            {
                var input = Environment.NewLine + "Hello world!" + " " + Environment.NewLine + " ";
                var expected = Environment.NewLine + "Hello world!";
                var actual = input.Sanitize();
                Assert.Equal(expected, actual);
            }
        }

        public class CompressDecompress
        {
            [Fact]
            public void CompressDecompress_01()
            {
                var input = "Hello World!";
                var compressed = input.Compress();
                Assert.NotEqual(input, compressed);
                var decompressed = compressed.Decompress();
                Assert.Equal(input, decompressed);
            }

            [Fact]
            public void CompressDecompress_02()
            {
                var builder = new StringBuilder();
                for (var i = 0; i < 100; i++)
                {
                    builder.Append("Hello World!");
                }
                var input = builder.ToString();
                var compressed = input.Compress();
                Assert.True(compressed.Length < input.Length);
            }
        }
    }
}