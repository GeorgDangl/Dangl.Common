using System;
using Xunit;

namespace Dangl.Common.Tests
{
    public static class ObjectExtensionsTests
    {
        public class DeepCopy
        {
            [Fact]
            public void ReturnNullForNullInput()
            {
                var actual = ObjectExtensions.DeepCopy<object>(null);
                Assert.Null(actual);
            }

            [Fact]
            public void ReturnsSameForPrimitiveType_Integer()
            {
                var source = 1;
                var actual = source.DeepCopy();
                Assert.Equal(source, actual);
            }

            [Fact]
            public void ReturnsSameForPrimitiveType_Boolean()
            {
                var source = true;
                var actual = source.DeepCopy();
                Assert.Equal(source, actual);
            }

            [Fact]
            public void ReturnsSameForPrimitiveType_Decimal()
            {
                var source = 1.3m;
                var actual = source.DeepCopy();
                Assert.Equal(source, actual);
            }

            [Fact]
            public void ReturnsSameForPrimitiveType_Byte()
            {
                var source = (byte)13;
                var actual = source.DeepCopy();
                Assert.Equal(source, actual);
            }

            [Fact]
            public void ReturnsSameForPrimitiveType_String()
            {
                var source = "Hello World!";
                var actual = source.DeepCopy();
                Assert.Equal(source, actual);
            }

            [Fact]
            public void ReturnsSameForPrimitiveType_DateTime()
            {
                var source = new DateTime(2019, 1, 1);
                var actual = source.DeepCopy();
                Assert.Equal(source, actual);
            }

            [Fact]
            public void ReturnsSameForPrimitiveType_DateTimeOffset()
            {
                var source = new DateTimeOffset(2019, 1, 1, 0, 0, 0, TimeSpan.Zero);
                var actual = source.DeepCopy();
                Assert.Equal(source, actual);
            }

            [Fact]
            public void ReturnsSameForPrimitiveType_Guid()
            {
                var source = Guid.NewGuid();
                var actual = source.DeepCopy();
                Assert.Equal(source, actual);
            }

            [Fact]
            public void ReturnsSameForPrimitiveType_TimeSpan()
            {
                var source = TimeSpan.FromMinutes(1);
                var actual = source.DeepCopy();
                Assert.Equal(source, actual);
            }

            [Fact]
            public void ReturnsSameForPrimitiveType_Char()
            {
                var source = 'a';
                var actual = source.DeepCopy();
                Assert.Equal(source, actual);
            }

            [Fact]
            public void ReturnsSameForPrimitiveType_Double()
            {
                var source = 1.3;
                var actual = source.DeepCopy();
                Assert.Equal(source, actual);
            }

            [Fact]
            public void CreatesCopy_AnonymousType()
            {
                var source = new { Name = "Test", Value = 1 };
                var actual = source.DeepCopy();
                Assert.NotSame(source, actual);
                Assert.Equal(source.Name, actual.Name);
                Assert.Equal(source.Value, actual.Value);
            }

            [Fact]
            public void CreatesCopy_EmptyObject()
            {
                var source = new object();
                var actual = source.DeepCopy();
                Assert.False(ReferenceEquals(source, actual));
            }

            [Fact]
            public void CreatesCopy_ObjectWithProperties()
            {
                var source = new TestObject();
                source.Name = "Hello World!";
                source.Value = 42;
                var actual = source.DeepCopy();
                Assert.False(ReferenceEquals(source, actual));
                Assert.Equal(source.Name, actual.Name);
                Assert.Equal(source.Value, actual.Value);
            }

            [Fact]
            public void CreatesCopy_ObjectWithNestedObjects()
            {
                var source = new TestObject();
                source.Name = "Hello World!";
                source.Value = 42;
                source.Nested = new TestObject();
                source.Nested.Name = "Nested";
                source.Nested.Value = 1337;
                var actual = source.DeepCopy();
                Assert.False(ReferenceEquals(source, actual));
                Assert.Equal(source.Name, actual.Name);
                Assert.Equal(source.Value, actual.Value);
                Assert.False(ReferenceEquals(source.Nested, actual.Nested));
                Assert.Equal(source.Nested.Name, actual.Nested.Name);
                Assert.Equal(source.Nested.Value, actual.Nested.Value);
            }

            [Fact]
            public void CreatesCopy_ObjectWithNestedObjects_WithCircularReferences()
            {
                var source = new TestObject();
                source.Name = "Hello World!";
                source.Value = 42;
                source.Nested = new TestObject();
                source.Nested.Name = "Nested";
                source.Nested.Value = 1337;
                source.Nested.Nested = source;
                var actual = source.DeepCopy();
                Assert.True(ReferenceEquals(source, source.Nested.Nested));
                Assert.True(ReferenceEquals(actual, actual.Nested.Nested));
                Assert.False(ReferenceEquals(source, actual));
                Assert.Equal(source.Name, actual.Name);
                Assert.Equal(source.Value, actual.Value);
                Assert.False(ReferenceEquals(source.Nested, actual.Nested));
                Assert.Equal(source.Nested.Name, actual.Nested.Name);
                Assert.Equal(source.Nested.Value, actual.Nested.Value);
                Assert.False(ReferenceEquals(source.Nested.Nested, actual.Nested.Nested));
                Assert.Equal(source.Nested.Nested.Name, actual.Nested.Nested.Name);
                Assert.Equal(source.Nested.Nested.Value, actual.Nested.Nested.Value);
            }

            [Fact]
            public void CreatesCopy_ObjectWithAction()
            {
                var integerValue = 0;
                var source = new TestObject();
                source.Name = "Hello World!";
                source.Value = 42;
                source.Action = () => { integerValue = 1337; };
                var actual = source.DeepCopy();
                Assert.False(ReferenceEquals(source, actual));
                Assert.Equal(source.Name, actual.Name);
                Assert.Equal(source.Value, actual.Value);
                Assert.False(ReferenceEquals(source.Action, actual.Action));

                Assert.Equal(0, integerValue);
                source.Action();
                Assert.Equal(1337, integerValue);

                // Delegate types are actually not cloned
                Assert.Null(actual.Action);
            }

            [Fact]
            public void CanCopyComplexObjectWithNullProperty()
            {
                var source = new TestObject();
                source.Name = "Test";
                source.Nested = null;

                var actual = source.DeepCopy();

                Assert.False(ReferenceEquals(source, actual));
                Assert.Equal("Test", source.Name);
                Assert.Equal("Test", actual.Name);
                Assert.Null(source.Nested);
                Assert.Null(actual.Nested);
            }

            private class TestObject
            {
                public string Name { get; set; }
                public int Value { get; set; }
                public TestObject Nested { get; set; }
                public Action Action { get; set; }
            }
        }
    }
}
