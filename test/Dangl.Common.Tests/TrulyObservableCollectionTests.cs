using System.Collections.Generic;
using System.Collections.Specialized;
using Xunit;

namespace Dangl.Common.Tests
{
    public class TrulyObservableCollectionTests
    {
        private NotifyCollectionChangedEventArgs _catchedEventArgs;
        private bool _eventCatched;
        private int _eventCatchedCount;

        [Fact]
        public void CollectionChangedNotifiedWhenInClass()
        {
            var instance = new MockClass_TrulyObservableCollection();
            instance.MockCollection.CollectionChanged += TestCollection_CollectionChanged;
            instance.MockCollection.Add(new MockClass());
            Assert.True(_eventCatched);
            Assert.Equal(1, _eventCatchedCount);
        }

        [Fact]
        public void CollectionChangedNotified()
        {
            var testCollection = new TrulyObservableCollection<MockClass>();
            testCollection.CollectionChanged += TestCollection_CollectionChanged;
            _eventCatched = false;
            testCollection.Add(new MockClass());
            Assert.True(_eventCatched);
            Assert.Equal(1, _eventCatchedCount);
        }

        [Fact]
        public void CollectionItemChangedNotified()
        {
            var testCollection = new TrulyObservableCollection<MockClass>();
            testCollection.Add(new MockClass());
            testCollection.CollectionChanged += TestCollection_CollectionChanged;
            _eventCatched = false;
            _catchedEventArgs = null;
            testCollection[0].StringProperty = "Changed";
            Assert.True(_eventCatched);
            Assert.NotNull(_catchedEventArgs);
            Assert.Equal(NotifyCollectionChangedAction.Replace, _catchedEventArgs.Action);
            Assert.Same(_catchedEventArgs.NewItems[0], testCollection[0]);
            Assert.Equal(1, _eventCatchedCount);
        }

        [Fact]
        public void CollectionItemChangedNotified_OnInstanceFromIEnumerable_01()
        {
            var testCollection = new TrulyObservableCollection<MockClass>();
            testCollection.Add(new MockClass());
            testCollection.Add(new MockClass());
            testCollection.Add(new MockClass());
            testCollection.Add(new MockClass());
            var copiedInstance = new TrulyObservableCollection<MockClass>(testCollection);

            copiedInstance.CollectionChanged += TestCollection_CollectionChanged;
            _eventCatched = false;
            _catchedEventArgs = null;
            copiedInstance[0].StringProperty = "Changed";
            Assert.True(_eventCatched);
            Assert.NotNull(_catchedEventArgs);
            Assert.Equal(NotifyCollectionChangedAction.Replace, _catchedEventArgs.Action);
            Assert.Same(_catchedEventArgs.NewItems[0], copiedInstance[0]);
            Assert.Equal(1, _eventCatchedCount);
        }

        [Fact]
        public void CollectionItemChangedNotified_DetachEventWhenItemRemovedFromList()
        {
            var testCollection = new TrulyObservableCollection<MockClass>();
            var mockInstance = new MockClass();
            testCollection.Add(mockInstance);
            testCollection.CollectionChanged += TestCollection_CollectionChanged;
            Assert.False(_eventCatched);
            mockInstance.StringProperty = "First Change";
            Assert.True(_eventCatched);
            _eventCatched = false;
            Assert.False(_eventCatched);
            testCollection.Remove(mockInstance);
            Assert.True(_eventCatched);
            _eventCatched = false;
            Assert.False(_eventCatched);
            mockInstance.StringProperty = "Second Change";
            Assert.False(_eventCatched);
        }

        private void TestCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _catchedEventArgs = e;
            _eventCatchedCount++;
            _eventCatched = true;
        }

        public class Constructor
        {
            [Fact]
            public void InstantiationFromIEnumerable()
            {
                var tempList = new List<MockClass> {new MockClass {StringProperty = "One"}, new MockClass {StringProperty = "Two"}, new MockClass {StringProperty = "Three"}};
                var obsCol = new TrulyObservableCollection<MockClass>(tempList);
                Assert.Equal(tempList[0].StringProperty, obsCol[0].StringProperty);
                Assert.Equal(tempList[1].StringProperty, obsCol[1].StringProperty);
                Assert.Equal(tempList[2].StringProperty, obsCol[2].StringProperty);
            }

            [Fact]
            public void InstantiateAndGetEventHooks()
            {
                var tempList = new List<MockClass> {new MockClass {StringProperty = "One"}, new MockClass {StringProperty = "Two"}, new MockClass {StringProperty = "Three"}};
                var origin = new TrulyObservableCollection<MockClass>();
                foreach (var currentItem in tempList)
                {
                    origin.Add(currentItem);
                }
                var obsCol = new TrulyObservableCollection<MockClass>(origin);

                var changeDetected = false;
                obsCol.CollectionChanged += (x, y) =>
                {
                    changeDetected = true;
                };

                Assert.False(changeDetected);
                origin[0].StringProperty = "Another Value";
                Assert.True(changeDetected);
            }
        }

        private class MockClass_TrulyObservableCollection
        {
            private TrulyObservableCollection<MockClass> _mockCollection;

            public TrulyObservableCollection<MockClass> MockCollection
            {
                get
                {
                    return _mockCollection ?? (_mockCollection = new TrulyObservableCollection<MockClass>());
                }
            }
        }
    }
}