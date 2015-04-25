using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Dangl.Test.Common
{
    [TestClass]
    public class TestBindableBase
    {
        [TestClass]
        public class SetProperty
        {
            [TestMethod]
            public void IsCalled()
            {
                MockClass Mock = new MockClass();
                Mock.PropertyChanged += Mock_PropertyChanged;
                EventCatched = false;
                Mock.StringProperty = "changed";
                Assert.IsTrue(EventCatched);
            }

            [TestMethod]
            public void IsNotCalledForSameValue()
            {
                MockClass Mock = new MockClass();

                Mock.StringProperty = "SomeValue";
                Mock.PropertyChanged += Mock_PropertyChanged;
                EventCatched = false;
                Mock.StringProperty = "SomeValue";
                Assert.IsFalse(EventCatched);
            }

            private bool EventCatched;

            private void Mock_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                if (!EventCatched)
                {
                    EventCatched = true;
                }
                else
                {
                    Assert.Fail("Event catched multiple times.");
                }
            }

            // TODO SHOULD NOT RAISE EVENT WHEN COMPLEX OBJECT INSIDE RAISES
        }

        [TestClass]
        public class OnPropertyChanged
        {
            [TestMethod]
            public void EventNotRaisedForNestedClass()
            {
                MockClass Mock = new MockClass();
                Mock.PropertyChanged += Mock_PropertyChanged;
                EventCatched = false;
                Mock.ComplexProperty = new MockClass();
                Assert.IsTrue(EventCatched);
                EventCatched = false;
                Mock.ComplexProperty.StringProperty = "Changed";
                Assert.IsFalse(EventCatched);
            }

            private bool EventCatched;

            private void Mock_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                if (!EventCatched)
                {
                    EventCatched = true;
                }
                else
                {
                    Assert.Fail("Event catched multiple times.");
                }
            }
        }

        [TestClass]
        public class GetPropertyName
        {
            [TestMethod]
            public void ReportsCorrectName()
            {
                MockClass Mock = new MockClass();
                Assert.AreEqual("StringProperty", Mock.GetPropertyName(() => Mock.StringProperty));
            }
        }
    }
}
