using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dangl.Common.Tests
{
    [TestClass]
    public class StringExtensionMethodsTest
    {

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
