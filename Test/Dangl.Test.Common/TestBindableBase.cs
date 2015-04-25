﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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

    public class MockClass : BindableBase
    {
        #region Fields

        private string _StringProperty;

        #endregion Fields

        #region Properties

        public string StringProperty
        {
            get
            {
                return _StringProperty;
            }
            set
            {
                SetProperty(ref _StringProperty, value);
            }
        }

        #endregion Properties
    }
}
