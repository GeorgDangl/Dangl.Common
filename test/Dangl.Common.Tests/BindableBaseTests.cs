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

                var instanceThatWillBeWatched = new BindableBaseMock();

                Assert.False(instance.EventCatcher);
                instance.ChangeableCollection = null;
                instance.ChangeableProperty = instanceThatWillBeWatched;
                Assert.False(instance.EventCatcher);
                instanceThatWillBeWatched.StringProperty = "New string";
                Assert.True(instance.EventCatcher);
                instanceThatWillBeWatched.StringProperty = "New string again";
                Assert.False(instance.EventCatcher);
                instance.ChangeableProperty = new BindableBaseMock();
                Assert.False(instance.EventCatcher);
                instanceThatWillBeWatched.StringProperty = "Noone listens anymore =(";
                Assert.False(instance.EventCatcher);
                Assert.NotNull(instance.ChangeableProperty);
            }

            [Fact]
            public void AttachesEventHandlerHook_PropertyChanged_DontThrowWhenSetToNull()
            {
                var instance = new MockClassWithEvent();
                var instanceThatWillBeWatched = new BindableBaseMock();
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

                var instanceThatWillBeWatched = new ObservableCollection<BindableBaseMock>();

                Assert.False(instance.EventCatcher);
                instance.ChangeableCollection = null;
                instance.ChangeableCollection = instanceThatWillBeWatched;
                Assert.False(instance.EventCatcher);
                instanceThatWillBeWatched.Add(new BindableBaseMock());
                Assert.True(instance.EventCatcher);
                instanceThatWillBeWatched.Add(new BindableBaseMock());
                Assert.False(instance.EventCatcher);
                Assert.NotNull(instance.ChangeableCollection);
            }

            [Fact]
            public void AttachesEventHandlerHook_CollectionChanged_DontThrowWhenSetToNull()
            {
                var instance = new MockClassWithEvent();

                var instanceThatWillBeWatched = new ObservableCollection<BindableBaseMock>();

                Assert.False(instance.EventCatcher);
                instance.ChangeableCollection = null;
                instance.ChangeableCollection = instanceThatWillBeWatched;
                Assert.False(instance.EventCatcher);
                instanceThatWillBeWatched.Add(new BindableBaseMock());
                Assert.True(instance.EventCatcher);
                instanceThatWillBeWatched.Add(new BindableBaseMock());
                Assert.False(instance.EventCatcher);
                instance.ChangeableCollection = null;
                instanceThatWillBeWatched.Add(new BindableBaseMock());
                Assert.False(instance.EventCatcher);
            }

            [Fact]
            public void IsCalled()
            {
                var mock = new BindableBaseMock();
                mock.PropertyChanged += Mock_PropertyChanged;
                _eventCatched = false;
                mock.StringProperty = "changed";
                Assert.True(_eventCatched);
            }

            [Fact]
            public void IsNotCalledForSameValue()
            {
                var mock = new BindableBaseMock();

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

                var complexProperty = new BindableBaseMock();
                mock.ChangeableProperty = complexProperty;
                mock.PropertyChanged += Mock_PropertyChanged;
                _eventCatched = false;
                mock.ChangeableProperty = complexProperty;
                Assert.False(_eventCatched);
            }

            [Fact]
            public void IsNotCalledForSameValue_ComplexProperty()
            {
                var mock = new BindableBaseMock();
                var complexProperty = new BindableBaseMock();
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
                var collectionProperty = new ObservableCollection<BindableBaseMock>();
                mock.ChangeableCollection = collectionProperty;
                mock.PropertyChanged += Mock_PropertyChanged;
                _eventCatched = false;
                mock.ChangeableCollection = collectionProperty;
                Assert.False(_eventCatched);
            }

            [Fact]
            public void ReturnsTrueForPrimitiveButDifferentValue()
            {
                var instance = new SetPropertyMockClass();
                instance.PrimitiveValue = "Hello World!";
                Assert.False(instance.PrimitiveValueUpdated);
                instance.PrimitiveValue = "Hello Universe!";
                Assert.True(instance.PrimitiveValueUpdated);
            }

            [Fact]
            public void ReturnsFalseForPrimitiveButSameValue()
            {
                var instance = new SetPropertyMockClass();
                instance.PrimitiveValue = "Hello World!";
                Assert.False(instance.PrimitiveValueUpdated);
                instance.PrimitiveValue = "Hello World!";
                Assert.False(instance.PrimitiveValueUpdated);
            }

            [Fact]
            public void ReturnsTrueForComplexButDifferentValue()
            {
                var instance = new SetPropertyMockClass();
                instance.ComplexValue = new object();
                Assert.False(instance.ComplexValueUpdated);
                instance.ComplexValue = new object();
                Assert.True(instance.ComplexValueUpdated);
            }

            [Fact]
            public void ReturnsFalseForComplexButSameValue()
            {
                var complexValue = new object();
                var instance = new SetPropertyMockClass();
                instance.ComplexValue = complexValue;
                Assert.False(instance.ComplexValueUpdated);
                instance.ComplexValue = complexValue;
                Assert.False(instance.ComplexValueUpdated);
            }

            private void Mock_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                Assert.False(_eventCatched); // Catch event only once
                _eventCatched = true;
            }

            private class MockClassWithEvent : BindableBase
            {
                private ObservableCollection<BindableBaseMock> _changeableCollection;
                private BindableBaseMock _changeableProperty;

                public BindableBaseMock ChangeableProperty
                {
                    get => _changeableProperty;
                    set => SetProperty(ref _changeableProperty, value, ChangeableProperty_PropertyChanged);
                }

                public ObservableCollection<BindableBaseMock> ChangeableCollection
                {
                    get => _changeableCollection;
                    set => SetProperty(ref _changeableCollection, value, _ChangeableCollection_CollectionChanged);
                }

                public bool EventCatcher { get; private set; }

                private void _ChangeableCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
                {
                    EventCatcher = !EventCatcher;
                }

                private void ChangeableProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
                {
                    EventCatcher = !EventCatcher;
                }
            }

            private class SetPropertyMockClass : BindableBase
            {
                private string _primitiveValue;
                public bool PrimitiveValueUpdated { get; private set; }
                public string PrimitiveValue
                {
                    set
                    {
                        if (_primitiveValue != null)
                        {
                            PrimitiveValueUpdated = SetProperty(ref _primitiveValue, value);
                        }
                        else
                        {
                            _primitiveValue = value;
                        }
                    }
                }
                private object _complexValue;
                public bool ComplexValueUpdated { get; private set; }
                public object ComplexValue
                {
                    set
                    {
                        if (_complexValue != null)
                        {
                            ComplexValueUpdated = SetProperty(ref _complexValue, value);
                        }
                        else
                        {
                            _complexValue = value;
                        }
                    }
                }
            }
        }

        public class OnPropertyChanged
        {
            private bool _eventCatched;

            [Fact]
            public void EventNotRaisedForNestedClass()
            {
                var mock = new BindableBaseMock();
                mock.PropertyChanged += Mock_PropertyChanged;
                _eventCatched = false;
                mock.ComplexProperty = new BindableBaseMock();
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

        public class DisposableImplementation
        {
            [Fact]
            public void DisposingDoesntThrow_NoEventHandlersAttached()
            {
                var mock = new BindableBaseMock();
                mock.Dispose();
            }

            [Fact]
            public void DisposingDoesntThrow_EventHandlerAttached()
            {
                var mock = new BindableBaseMock();
                mock.PropertyChanged += Mock_PropertyChanged;
                mock.Dispose();
            }

            [Fact]
            public void EventGetsHandled()
            {
                var mock = new BindableBaseMock();
                mock.PropertyChanged += Mock_PropertyChanged;
                mock.StringProperty = "New value";
                Assert.True(_eventCatched);
            }

            [Fact]
            public void DoesntFireEventWhenDisposed()
            {
                var mock = new BindableBaseMock();
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
    }
}
