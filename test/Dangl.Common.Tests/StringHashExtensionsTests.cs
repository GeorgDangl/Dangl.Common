using Xunit;

namespace Dangl.Common.Tests
{
    public class StringHashExtensionsTests
    {
        public class ToMd5
        {
            [Fact]
            public void ReturnsNullForNullInput()
            {
                var actual = StringHashExtensions.ToMd5(null);
                Assert.Null(actual);
            }

            [Fact]
            public void ReturnsNullForEmptyInput()
            {
                var actual = string.Empty.ToMd5();
                Assert.Equal(string.Empty, actual);
            }

            [Fact]
            public void ReturnsHashForAllWhitespaceInput()
            {
                var actual = " ".ToMd5();
                Assert.Equal("7215EE9C7D9DC229D2921A40E899EC5F", actual);
            }

            [Fact]
            public void ReturnsHash()
            {
                var actual = "George".ToMd5();
                Assert.Equal("578AD8E10DC4EDB52FF2BD4EC9BC93A3", actual);
            }

            [Fact]
            public void ConsecutivelyReturnsSameHash()
            {
                var first = "George".ToMd5();
                var second = "George".ToMd5();
                Assert.Equal(first, second);
            }
        }

        public class ToSha256
        {
            [Fact]
            public void ReturnsNullForNullInput()
            {
                var actual = StringHashExtensions.ToSha256(null);
                Assert.Null(actual);
            }

            [Fact]
            public void ReturnsNullForEmptyInput()
            {
                var actual = string.Empty.ToSha256();
                Assert.Equal(string.Empty, actual);
            }

            [Fact]
            public void ReturnsHashForAllWhitespaceInput()
            {
                var actual = " ".ToSha256();
                Assert.Equal("36A9E7F1C95B82FFB99743E0C5C4CE95D83C9A430AAC59F84EF3CBFAB6145068", actual);
            }

            [Fact]
            public void ReturnsHash()
            {
                var actual = "George".ToSha256();
                Assert.Equal("3D28271EC52E3D07FE14F5F16D01F2C09CBCAC1949F9904B305136D0EDBEE12D", actual);
            }

            [Fact]
            public void ConsecutivelyReturnsSameHash()
            {
                var first = "George".ToSha256();
                var second = "George".ToSha256();
                Assert.Equal(first, second);
            }
        }
    }
}
