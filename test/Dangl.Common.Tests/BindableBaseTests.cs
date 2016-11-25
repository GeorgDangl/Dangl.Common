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
            private bool _eventCatched;

            [Fact]
            public void AttachesEventHandlerHook_PropertyChanged()
            {
                var instance = new MockClassWithEvent();

                var instanceThatWillBeWatched = new MockClass();

                Assert.False(instance.EventCatcher);
                instance.ChangeableCollection = null;
                instance.ChangeableProperty = instanceThatWillBeWatched;
                Assert.False(instance.EventCatcher);
                instanceThatWillBeWatched.StringProperty = "New string";
                Assert.True(instance.EventCatcher);
                instanceThatWillBeWatched.StringProperty = "New string again";
                Assert.False(instance.EventCatcher);
                instance.ChangeableProperty = new MockClass();
                Assert.False(instance.EventCatcher);
                instanceThatWillBeWatched.StringProperty = "Noone listens anymore =(";
                Assert.False(instance.EventCatcher);
                Assert.NotNull(instance.ChangeableProperty);
            }

            [Fact]
            public void AttachesEventHandlerHook_PropertyChanged_DontThrowWhenSetToNull()
            {
                var instance = new MockClassWithEvent();
                var instanceThatWillBeWatched = new MockClass();
                Assert.Null(instance.ChangeableProperty);
                Assert.False(instance.EventCatcher);
                instance.ChangeableProperty = instanceThatWillBeWatched;
                Assert.NotNull(instance.ChangeableProperty);
                instance.ChangeableProperty = null;
                Assert.Null(instance.ChangeableProperty);
                Assert.False(instance.EventCatcher);
            }

            [Fact]
            public void AttachesEventHandlerHook_CollectionChanged()
            {
                var instance = new MockClassWithEvent();

                var instanceThatWillBeWatched = new ObservableCollection<MockClass>();

                Assert.False(instance.EventCatcher);
                instance.ChangeableCollection = null;
                instance.ChangeableCollection = instanceThatWillBeWatched;
                Assert.False(instance.EventCatcher);
                instanceThatWillBeWatched.Add(new MockClass());
                Assert.True(instance.EventCatcher);
                instanceThatWillBeWatched.Add(new MockClass());
                Assert.False(instance.EventCatcher);
                Assert.NotNull(instance.ChangeableCollection);
            }

            [Fact]
            public void AttachesEventHandlerHook_CollectionChanged_DontThrowWhenSetToNull()
            {
                var instance = new MockClassWithEvent();

                var instanceThatWillBeWatched = new ObservableCollection<MockClass>();

                Assert.False(instance.EventCatcher);
                instance.ChangeableCollection = null;
                instance.ChangeableCollection = instanceThatWillBeWatched;
                Assert.False(instance.EventCatcher);
                instanceThatWillBeWatched.Add(new MockClass());
                Assert.True(instance.EventCatcher);
                instanceThatWillBeWatched.Add(new MockClass());
                Assert.False(instance.EventCatcher);
                instance.ChangeableCollection = null;
                instanceThatWillBeWatched.Add(new MockClass());
                Assert.False(instance.EventCatcher);
            }

            [Fact]
            public void IsCalled()
            {
                var mock = new MockClass();
                mock.PropertyChanged += Mock_PropertyChanged;
                _eventCatched = false;
                mock.StringProperty = "changed";
                Assert.True(_eventCatched);
            }

            [Fact]
            public void IsNotCalledForSameValue()
            {
                var mock = new MockClass();

                mock.StringProperty = "SomeValue";
                mock.PropertyChanged += Mock_PropertyChanged;
                _eventCatched = false;
                mock.StringProperty = "SomeValue";
                Assert.False(_eventCatched);
            }

            [Fact]
            public void IsNotCalledForSameValue_MockClassWithEvent()
            {
                var mock = new MockClassWithEvent();

                var complexProperty = new MockClass();
                mock.ChangeableProperty = complexProperty;
                mock.PropertyChanged += Mock_PropertyChanged;
                _eventCatched = false;
                mock.ChangeableProperty = complexProperty;
                Assert.False(_eventCatched);
            }

            [Fact]
            public void IsNotCalledForSameValue_ComplexProperty()
            {
                var mock = new MockClass();
                var complexProperty = new MockClass();
                mock.ComplexProperty = complexProperty;
                mock.PropertyChanged += Mock_PropertyChanged;
                _eventCatched = false;
                mock.ComplexProperty = complexProperty;
                Assert.False(_eventCatched);
            }

            [Fact]
            public void IsNotCalledForSameValue_CollectionProperty()
            {
                var mock = new MockClassWithEvent();
                var collectionProperty = new ObservableCollection<MockClass>();
                mock.ChangeableCollection = collectionProperty;
                mock.PropertyChanged += Mock_PropertyChanged;
                _eventCatched = false;
                mock.ChangeableCollection = collectionProperty;
                Assert.False(_eventCatched);
            }

            private void Mock_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                Assert.False(_eventCatched); // Catch event only once
                _eventCatched = true;
            }

            public class MockClassWithEvent : BindableBase
            {
                private ObservableCollection<MockClass> _changeableCollection;
                private MockClass _changeableProperty;

                public MockClass ChangeableProperty
                {
                    get
                    {
                        return _changeableProperty;
                    }
                    set
                    {
                        SetProperty(ref _changeableProperty, value, ChangeableProperty_PropertyChanged);
                    }
                }

                public ObservableCollection<MockClass> ChangeableCollection
                {
                    get
                    {
                        return _changeableCollection;
                    }
                    set
                    {
                        SetProperty(ref _changeableCollection, value, _ChangeableCollection_CollectionChanged);
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
            private bool _eventCatched;

            [Fact]
            public void EventNotRaisedForNestedClass()
            {
                var mock = new MockClass();
                mock.PropertyChanged += Mock_PropertyChanged;
                _eventCatched = false;
                mock.ComplexProperty = new MockClass();
                Assert.True(_eventCatched);
                _eventCatched = false;
                mock.ComplexProperty.StringProperty = "Changed";
                Assert.False(_eventCatched);
            }

            private void Mock_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                Assert.False(_eventCatched); // Catch event only once
                _eventCatched = true;
            }
        }

        public class IDisposableImplementation
        {
            [Fact]
            public void DisposingDoesntThrow_NoEventHandlersAttached()
            {
                var mock = new MockClass();
                mock.Dispose();
            }

            [Fact]
            public void DisposingDoesntThrow_EventHandlerAttached()
            {
                var mock = new MockClass();
                mock.PropertyChanged += Mock_PropertyChanged;
                mock.Dispose();
            }

            [Fact]
            public void EventGetsHandled()
            {
                var mock = new MockClass();
                mock.PropertyChanged += Mock_PropertyChanged;
                mock.StringProperty = "New value";
                Assert.True(_eventCatched);
            }

            [Fact]
            public void DoesntFireEventWhenDisposed()
            {
                var mock = new MockClass();
                mock.PropertyChanged += Mock_PropertyChanged;
                mock.Dispose();
                mock.StringProperty = "New value";
                Assert.False(_eventCatched);
            }

            private bool _eventCatched;

            private void Mock_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (!_eventCatched)
                {
                    _eventCatched = true;
                }
            }
        }

        public class GetPropertyName
        {
            [Fact]
            public void ReportsCorrectName()
            {
                var mock = new MockClass();
                Assert.Equal("StringProperty", mock.GetPropertyName(() => mock.StringProperty));
            }

            [Fact]
            public void ReportNestedArrayLengthName()
            {
                var testInstance = new SomeClassWithArrays();

                var retrievedPropertyName = testInstance.GetPropertyName(() => testInstance.Array) + ".Length";
                Assert.Equal("Array.Length", retrievedPropertyName);
                Assert.Null(testInstance.Array);
                testInstance.Array = new string[2];
                Assert.NotNull(testInstance.Array);
                retrievedPropertyName = testInstance.GetPropertyName(() => testInstance.Array) + ".Length";
                Assert.Equal("Array.Length", retrievedPropertyName);
            }

            public class SomeClassWithArrays : BindableBase
            {
                public string[] Array { get; set; }
            }
        }
    }
}