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
            var instance = new TrulyObservableCollectionMock();
            instance.MockCollection.CollectionChanged += TestCollection_CollectionChanged;
            instance.MockCollection.Add(new BindableBaseMock());
            Assert.True(_eventCatched);
            Assert.Equal(1, _eventCatchedCount);
        }

        [Fact]
        public void CollectionChangedNotified()
        {
            var testCollection = new TrulyObservableCollection<BindableBaseMock>();
            testCollection.CollectionChanged += TestCollection_CollectionChanged;
            _eventCatched = false;
            testCollection.Add(new BindableBaseMock());
            Assert.True(_eventCatched);
            Assert.Equal(1, _eventCatchedCount);
        }

        [Fact]
        public void CollectionItemChangedNotified()
        {
            var testCollection = new TrulyObservableCollection<BindableBaseMock>();
            testCollection.Add(new BindableBaseMock());
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
            var testCollection = new TrulyObservableCollection<BindableBaseMock>();
            testCollection.Add(new BindableBaseMock());
            testCollection.Add(new BindableBaseMock());
            testCollection.Add(new BindableBaseMock());
            testCollection.Add(new BindableBaseMock());
            var copiedInstance = new TrulyObservableCollection<BindableBaseMock>(testCollection);

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
            var testCollection = new TrulyObservableCollection<BindableBaseMock>();
            var mockInstance = new BindableBaseMock();
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
                var tempList = new List<BindableBaseMock> {new BindableBaseMock {StringProperty = "One"}, new BindableBaseMock {StringProperty = "Two"}, new BindableBaseMock {StringProperty = "Three"}};
                var obsCol = new TrulyObservableCollection<BindableBaseMock>(tempList);
                Assert.Equal(tempList[0].StringProperty, obsCol[0].StringProperty);
                Assert.Equal(tempList[1].StringProperty, obsCol[1].StringProperty);
                Assert.Equal(tempList[2].StringProperty, obsCol[2].StringProperty);
            }

            [Fact]
            public void InstantiateAndGetEventHooks()
            {
                var tempList = new List<BindableBaseMock> {new BindableBaseMock {StringProperty = "One"}, new BindableBaseMock {StringProperty = "Two"}, new BindableBaseMock {StringProperty = "Three"}};
                var origin = new TrulyObservableCollection<BindableBaseMock>();
                foreach (var currentItem in tempList)
                {
                    origin.Add(currentItem);
                }
                var obsCol = new TrulyObservableCollection<BindableBaseMock>(origin);

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

        private class TrulyObservableCollectionMock
        {
            private TrulyObservableCollection<BindableBaseMock> _mockCollection;

            public TrulyObservableCollection<BindableBaseMock> MockCollection => _mockCollection ?? (_mockCollection = new TrulyObservableCollection<BindableBaseMock>());
        }
    }
}
