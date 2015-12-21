using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Xunit;

namespace Dangl.Common.Tests
{
    public class BindableBaseTests
    {
        [Fact]
        public void WillIBeDiscovered()
        {
            Assert.True(true);
        }

        public class SetProperty
        {
            private bool EventCatched;

            [Fact]
            public void AttachesEventHandlerHook_PropertyChanged()
            {
                var Instance = new MockClassWithEvent();

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
                var Instance = new MockClassWithEvent();

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
                var Instance = new MockClassWithEvent();

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
                var Mock = new MockClass();
                Mock.PropertyChanged += Mock_PropertyChanged;
                EventCatched = false;
                Mock.StringProperty = "changed";
                Assert.True(EventCatched);
            }

            [Fact]
            public void IsNotCalledForSameValue()
            {
                var Mock = new MockClass();

                Mock.StringProperty = "SomeValue";
                Mock.PropertyChanged += Mock_PropertyChanged;
                EventCatched = false;
                Mock.StringProperty = "SomeValue";
                Assert.False(EventCatched);
            }

            private void Mock_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                Assert.False(EventCatched); // Catch event only once
                EventCatched = true;
            }

            public class MockClassWithEvent : BindableBase
            {
                public ObservableCollection<MockClass> _ChangeableCollection;
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

                public bool EventCatcher { get; set; }

                private void _ChangeableCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
                {
                    EventCatcher = !EventCatcher;
                }

                private void ChangeableProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
                {
                    EventCatcher = !EventCatcher;
                }
            }
        }

        public class OnPropertyChanged
        {
            private bool EventCatched;

            [Fact]
            public void EventNotRaisedForNestedClass()
            {
                var Mock = new MockClass();
                Mock.PropertyChanged += Mock_PropertyChanged;
                EventCatched = false;
                Mock.ComplexProperty = new MockClass();
                Assert.True(EventCatched);
                EventCatched = false;
                Mock.ComplexProperty.StringProperty = "Changed";
                Assert.False(EventCatched);
            }

            private void Mock_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
                var Mock = new MockClass();
                Assert.Equal("StringProperty", Mock.GetPropertyName(() => Mock.StringProperty));
            }

            [Fact]
            public void ReportNestedArrayLengthName()
            {
                var TestInstance = new SomeClassWithArrays();

                var RetrievedPropertyName = TestInstance.GetPropertyName(() => TestInstance.Array) + ".Length";
                Assert.Equal("Array.Length", RetrievedPropertyName);
            }

            public class SomeClassWithArrays : BindableBase
            {
                public string[] Array { get; set; }
            }
        }
    }
}