using System;
using Xunit;

namespace Dangl.Common.Tests
{
    public class DecimalExtensionsTests
    {
        [Fact]
        public void ArgumentOutOfRangeExceptionForZeroValue()
        {
            Assert.Throws<ArgumentOutOfRangeException>("maxValueAbsolute", () => 5m.WithMaxAbsoluteValue(0m));
        }

        [Fact]
        public void ArgumentOutOfRangeExceptionForNegativeValue()
        {
            Assert.Throws<ArgumentOutOfRangeException>("maxValueAbsolute", () => 5m.WithMaxAbsoluteValue(-10m));
        }

        [Theory]
        [InlineData(0, 1, 0)]
        [InlineData(1, 1, 1)]
        [InlineData(2, 1, 1)]
        [InlineData(-1, 1, -1)]
        [InlineData(-2, 1, -1)]
        [InlineData(9.999, 10, 9.999)]
        [InlineData(99.999, 10, 10)]
        [InlineData(99.999, 100, 99.999)]
        [InlineData(101, 100, 100)]
        [InlineData(-9.999, 10, -9.999)]
        [InlineData(-99.999, 10, -10)]
        [InlineData(-99.999, 100, -99.999)]
        [InlineData(-101, 100, -100)]
        public void TransformCorrectly(decimal inputValue, decimal maxValue, decimal expectedValue)
        {
            var actualValue = inputValue.WithMaxAbsoluteValue(maxValue);
            Assert.Equal(expectedValue, actualValue);
        }
    }
}
