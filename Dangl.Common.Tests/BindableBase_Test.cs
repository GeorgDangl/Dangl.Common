using Xunit;
using System;
using System.Collections.ObjectModel;

namespace Dangl.Test.Common
{
     
    public class BindableBase_Test
    {
        [Fact]
        public void WillIBeDiscovered()
        {
            Assert.True(true);
        }


         
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

            [Fact]
            public void AttachesEventHandlerHook_PropertyChanged()
            {
                MockClassWithEvent Instance = new MockClassWithEvent();

                var InstanceThatWillBeWatched = new MockClass();

                Assert.False(Instance.EventCatcher);
                Instance.ChangeableCollection = null;
                Instance.ChangeableProperty = InstanceThatWillBeWatched;
                Assert.False(Instance.EventCatcher);
                InstanceThatWillBeWatched.StringProperty = "New string";
                Assert.True(Instance.EventCatcher);
                InstanceThatWillBeWatched.StringProperty = "New string again";
                Assert.False(Instance.EventCatcher);
                Instance.ChangeableProperty = new MockClass();
                Assert.False(Instance.EventCatcher);
                InstanceThatWillBeWatched.StringProperty = "Noone listens anymore =(";
                Assert.False(Instance.EventCatcher);
            }

            [Fact]
            public void AttachesEventHandlerHook_CollectionChanged()
            {
                MockClassWithEvent Instance = new MockClassWithEvent();

                var InstanceThatWillBeWatched = new ObservableCollection<MockClass>();

                Assert.False(Instance.EventCatcher);
                Instance.ChangeableCollection = null;
                Instance.ChangeableCollection = InstanceThatWillBeWatched;
                Assert.False(Instance.EventCatcher);
                InstanceThatWillBeWatched.Add(new MockClass());
                Assert.True(Instance.EventCatcher);
                InstanceThatWillBeWatched.Add(new MockClass());
                Assert.False(Instance.EventCatcher);
            }

            [Fact]
            public void AttachesEventHandlerHook_CollectionChanged_DontThrowWhenSetToNull()
            {
                MockClassWithEvent Instance = new MockClassWithEvent();

                var InstanceThatWillBeWatched = new ObservableCollection<MockClass>();

                Assert.False(Instance.EventCatcher);
                Instance.ChangeableCollection = null;
                Instance.ChangeableCollection = InstanceThatWillBeWatched;
                Assert.False(Instance.EventCatcher);
                InstanceThatWillBeWatched.Add(new MockClass());
                Assert.True(Instance.EventCatcher);
                InstanceThatWillBeWatched.Add(new MockClass());
                Assert.False(Instance.EventCatcher);
                Instance.ChangeableCollection = null;
                InstanceThatWillBeWatched.Add(new MockClass());
                Assert.False(Instance.EventCatcher);
            }

            [Fact]
            public void IsCalled()
            {
                MockClass Mock = new MockClass();
                Mock.PropertyChanged += Mock_PropertyChanged;
                EventCatched = false;
                Mock.StringProperty = "changed";
                Assert.True(EventCatched);
            }

            [Fact]
            public void IsNotCalledForSameValue()
            {
                MockClass Mock = new MockClass();

                Mock.StringProperty = "SomeValue";
                Mock.PropertyChanged += Mock_PropertyChanged;
                EventCatched = false;
                Mock.StringProperty = "SomeValue";
                Assert.False(EventCatched);
            }

            private bool EventCatched;

            private void Mock_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                Assert.False(EventCatched); // Catch event only once
                EventCatched = true;
            }
        }


        public class OnPropertyChanged
        {
            [Fact]
            public void EventNotRaisedForNestedClass()
            {
                MockClass Mock = new MockClass();
                Mock.PropertyChanged += Mock_PropertyChanged;
                EventCatched = false;
                Mock.ComplexProperty = new MockClass();
                Assert.True(EventCatched);
                EventCatched = false;
                Mock.ComplexProperty.StringProperty = "Changed";
                Assert.False(EventCatched);
            }

            private bool EventCatched;

            private void Mock_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                Assert.False(EventCatched); // Catch event only once
                EventCatched = true;
            }
        }


        public class GetPropertyName
        {
            [Fact]
            public void ReportsCorrectName()
            {
                MockClass Mock = new MockClass();
                Assert.Equal("StringProperty", Mock.GetPropertyName(() => Mock.StringProperty));
            }

            [Fact]
            public void ReportNestedArrayLengthName()
            {
                SomeClassWithArrays TestInstance = new SomeClassWithArrays();

                string RetrievedPropertyName = TestInstance.GetPropertyName(() => TestInstance.Array) + ".Length";
                Assert.Equal("Array.Length", RetrievedPropertyName);
            }

            public class SomeClassWithArrays : BindableBase
            {
                public string[] Array { get; set; }
            }
        }
    }
}
