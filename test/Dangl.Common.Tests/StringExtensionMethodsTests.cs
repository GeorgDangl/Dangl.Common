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
                var Input = "<html>Body</html>";
                Assert.Throws(typeof (FormatException), () => { Input.FromBase64(); });
            }

            [Fact]
            public void InvalidInput_RegularStringNonBase64()
            {
                var Input = "SomeString";
                Assert.Throws(typeof (FormatException), () => { Input.FromBase64(); });
            }

            [Fact]
            public void Convert_01()
            {
                var Input = "SGVsbG8gV29ybGQh";
                var Expected = "Hello World!";
                var Actual = Input.FromBase64();
                Assert.Equal(Expected, Actual);
            }

            [Fact]
            public void Convert_02()
            {
                var Input = "PGh0bWw+Qm9keTwvaHRtbD4=";
                var Expected = "<html>Body</html>";
                var Actual = Input.FromBase64();
                Assert.Equal(Expected, Actual);
            }
        }

        public class ToBase64
        {
            [Fact]
            public void Convert_01()
            {
                var Input = "<html>Body</html>";
                var Expected = "PGh0bWw+Qm9keTwvaHRtbD4=";
                var Actual = Input.ToBase64();
                Assert.Equal(Expected, Actual);
            }

            [Fact]
            public void Convert_02()
            {
                var Input = "<html>Body</html>";
                var Expected = "PGh0bWw+Qm9keTwvaHRtbD4=";
                var Actual = Input.ToBase64();
                Assert.Equal(Expected, Actual);
            }
        }

        public class Sanitize
        {
            [Fact]
            public void SanitizeNullString()
            {
                string Input = null;
                string Expected = null;
                var Actual = Input.Sanitize();
                Assert.Equal(Expected, Actual);
            }

            [Fact]
            public void Sanitize_01()
            {
                var Input = string.Empty;
                var Expected = string.Empty;
                var Actual = Input.Sanitize();
                Assert.Equal(Expected, Actual);
            }

            [Fact]
            public void Sanitize_02()
            {
                var Input = "Hello world!";
                var Expected = "Hello world!";
                var Actual = Input.Sanitize();
                Assert.Equal(Expected, Actual);
            }

            [Fact]
            public void Sanitize_03()
            {
                var Input = "Hello world! " + " ";
                var Expected = "Hello world!";
                var Actual = Input.Sanitize();
                Assert.Equal(Expected, Actual);
            }

            [Fact]
            public void Sanitize_04()
            {
                var Input = "Hello world!" + Environment.NewLine;
                var Expected = "Hello world!";
                var Actual = Input.Sanitize();
                Assert.Equal(Expected, Actual);
            }

            [Fact]
            public void Sanitize_05()
            {
                var Input = "Hello world! " + " " + Environment.NewLine;
                var Expected = "Hello world!";
                var Actual = Input.Sanitize();
                Assert.Equal(Expected, Actual);
            }

            [Fact]
            public void Sanitize_06()
            {
                var Input = "Hello world!" + " " + Environment.NewLine + " ";
                var Expected = "Hello world!";
                var Actual = Input.Sanitize();
                Assert.Equal(Expected, Actual);
            }

            [Fact]
            public void Sanitize_07()
            {
                var Input = Environment.NewLine + "Hello world!" + " " + Environment.NewLine + " ";
                var Expected = Environment.NewLine + "Hello world!";
                var Actual = Input.Sanitize();
                Assert.Equal(Expected, Actual);
            }
        }

        public class CompressDecompress
        {
            [Fact]
            public void CompressDecompress_01()
            {
                var Input = "Hello World!";
                var Compressed = Input.Compress();
                Assert.NotEqual(Input, Compressed);
                var Decompressed = Compressed.Decompress();
                Assert.Equal(Input, Decompressed);
            }

            [Fact]
            public void CompressDecompress_02()
            {
                var Builder = new StringBuilder();
                for (var i = 0; i < 100; i++)
                {
                    Builder.Append("Hello World!");
                }
                var Input = Builder.ToString();
                var Compressed = Input.Compress();
                Assert.True(Compressed.Length < Input.Length);
            }
        }
    }
}