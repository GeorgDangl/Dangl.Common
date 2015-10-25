using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dangl.Common.Tests
{
    [TestClass]
    public class StringExtensionMethodsTest
    {

        [TestClass]
        public class FromBase64
        {
            [TestMethod]
            [ExpectedException(typeof(FormatException))]
            public void InvalidInput()
            {
                var Input = "<html>Body</html>";
                var Actual = Input.FromBase64();
            }

            [TestMethod]
            public void Convert_01()
            {
                var Input = "SGVsbG8gV29ybGQh";
                var Expected = "Hello World!";
                var Actual = Input.FromBase64();
                Assert.AreEqual(Expected, Actual);
            }

            [TestMethod]
            public void Convert_02()
            {
                var Input = "PGh0bWw+Qm9keTwvaHRtbD4=";
                var Expected = "<html>Body</html>";
                var Actual = Input.FromBase64();
                Assert.AreEqual(Expected, Actual);
            }
        }
        [TestClass]
        public class ToBase64
        {
            [TestMethod]
            public void Convert_01()
            {
                var Input = "<html>Body</html>";
                var Expected = "PGh0bWw+Qm9keTwvaHRtbD4=";
                var Actual = Input.ToBase64();
                Assert.AreEqual(Expected, Actual);
            }

            [TestMethod]
            public void Convert_02()
            {
                var Input = "<html>Body</html>";
                var Expected = "PGh0bWw+Qm9keTwvaHRtbD4=";
                var Actual = Input.ToBase64();
                Assert.AreEqual(Expected, Actual);
            }
        }
        [TestClass]
        public class Sanitize
        {
            [TestMethod]
            public void SanitizeNullString()
            {
                string Input = null;
                string Expected = null;
                var Actual = Input.Sanitize();
                Assert.AreEqual(Expected, Actual);
            }
            [TestMethod]
            public void Sanitize_01()
            {
                var Input = string.Empty;
                var Expected = string.Empty;
                var Actual = Input.Sanitize();
                Assert.AreEqual(Expected, Actual);
            }
            [TestMethod]
            public void Sanitize_02()
            {
                var Input = "Hello world!";
                var Expected = "Hello world!";
                var Actual = Input.Sanitize();
                Assert.AreEqual(Expected, Actual);
            }
            [TestMethod]
            public void Sanitize_03()
            {
                var Input = "Hello world! " + " ";
                var Expected = "Hello world!";
                var Actual = Input.Sanitize();
                Assert.AreEqual(Expected, Actual);
            }
            [TestMethod]
            public void Sanitize_04()
            {
                var Input = "Hello world!" + Environment.NewLine;
                var Expected = "Hello world!";
                var Actual = Input.Sanitize();
                Assert.AreEqual(Expected, Actual);
            }
            [TestMethod]
            public void Sanitize_05()
            {
                var Input = "Hello world! " + " " + Environment.NewLine;
                var Expected = "Hello world!";
                var Actual = Input.Sanitize();
                Assert.AreEqual(Expected, Actual);
            }
            [TestMethod]
            public void Sanitize_06()
            {
                var Input = "Hello world!" + " " + Environment.NewLine + " ";
                var Expected = "Hello world!";
                var Actual = Input.Sanitize();
                Assert.AreEqual(Expected, Actual);
            }
            [TestMethod]
            public void Sanitize_07()
            {
                var Input = Environment.NewLine + "Hello world!" + " " + Environment.NewLine + " ";
                var Expected = Environment.NewLine + "Hello world!";
                var Actual = Input.Sanitize();
                Assert.AreEqual(Expected, Actual);
            }
        }


    }
}
