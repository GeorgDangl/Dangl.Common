using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.ObjectModel;

namespace Dangl.Test.Common
{
    [TestClass]
    public class BindableBase_Test
    {
        [TestClass]
        public class SetProperty
        {
            public class MockClassWithEvent : BindableBase
            {
                private MockClass _ChangeableProperty;

                public MockClass ChangeableProperty
                {
                    get
                    {
                        return _ChangeableProperty;
                    }
                    set
                    {
                        SetProperty(ref _ChangeableProperty, value, ChangeableProperty_PropertyChanged);
                    }
                }

                public ObservableCollection<MockClass> _ChangeableCollection;

                public ObservableCollection<MockClass> ChangeableCollection
                {
                    get
                    {
                        return _ChangeableCollection;
                    }
                    set
                    {
                        SetProperty(ref _ChangeableCollection, value, _ChangeableCollection_CollectionChanged);
                    }
                }

                private void _ChangeableCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
                {
                    EventCatcher = !EventCatcher;
                }

                public bool EventCatcher { get; set; }

                private void ChangeableProperty_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                    EventCatcher = !EventCatcher;
                }
            }

            [TestMethod]
            public void AttachesEventHandlerHook_PropertyChanged()
            {
                MockClassWithEvent Instance = new MockClassWithEvent();

                var InstanceThatWillBeWatched = new MockClass();

                Assert.IsFalse(Instance.EventCatcher);
                Instance.ChangeableCollection = null;
                Instance.ChangeableProperty = InstanceThatWillBeWatched;
                Assert.IsFalse(Instance.EventCatcher);
                InstanceThatWillBeWatched.StringProperty = "New string";
                Assert.IsTrue(Instance.EventCatcher);
                InstanceThatWillBeWatched.StringProperty = "New string again";
                Assert.IsFalse(Instance.EventCatcher);
                Instance.ChangeableProperty = new MockClass();
                Assert.IsFalse(Instance.EventCatcher);
                InstanceThatWillBeWatched.StringProperty = "Noone listens anymore =(";
                Assert.IsFalse(Instance.EventCatcher);
            }

            [TestMethod]
            public void AttachesEventHandlerHook_CollectionChanged()
            {
                MockClassWithEvent Instance = new MockClassWithEvent();

                var InstanceThatWillBeWatched = new ObservableCollection<MockClass>();

                Assert.IsFalse(Instance.EventCatcher);
                Instance.ChangeableCollection = null;
                Instance.ChangeableCollection = InstanceThatWillBeWatched;
                Assert.IsFalse(Instance.EventCatcher);
                InstanceThatWillBeWatched.Add(new MockClass());
                Assert.IsTrue(Instance.EventCatcher);
                InstanceThatWillBeWatched.Add(new MockClass());
                Assert.IsFalse(Instance.EventCatcher);
            }

            [TestMethod]
            public void AttachesEventHandlerHook_CollectionChanged_DontThrowWhenSetToNull()
            {
                MockClassWithEvent Instance = new MockClassWithEvent();

                var InstanceThatWillBeWatched = new ObservableCollection<MockClass>();

                Assert.IsFalse(Instance.EventCatcher);
                Instance.ChangeableCollection = null;
                Instance.ChangeableCollection = InstanceThatWillBeWatched;
                Assert.IsFalse(Instance.EventCatcher);
                InstanceThatWillBeWatched.Add(new MockClass());
                Assert.IsTrue(Instance.EventCatcher);
                InstanceThatWillBeWatched.Add(new MockClass());
                Assert.IsFalse(Instance.EventCatcher);
                Instance.ChangeableCollection = null;
                InstanceThatWillBeWatched.Add(new MockClass());
                Assert.IsFalse(Instance.EventCatcher);
            }

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

            [TestMethod]
            public void ReportNestedArrayLengthName()
            {
                SomeClassWithArrays TestInstance = new SomeClassWithArrays();

                string RetrievedPropertyName = TestInstance.GetPropertyName(() => TestInstance.Array) + ".Length";
                Assert.AreEqual("Array.Length", RetrievedPropertyName);
            }

            public class SomeClassWithArrays : BindableBase
            {
                public string[] Array { get; set; }
            }
        }
    }
}