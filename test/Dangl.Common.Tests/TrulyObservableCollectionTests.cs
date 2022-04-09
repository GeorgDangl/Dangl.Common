using System;
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
        public void CanAddNull()
        {
            var instance = new TrulyObservableCollectionMock();
            instance.MockCollection.Add(null);
            Assert.True(true);
        }

        [Fact]
        public void CanRemoveNull()
        {
            var instance = new TrulyObservableCollectionMock();
            instance.MockCollection.Add(null);
            Assert.True(true);
            instance.MockCollection.RemoveAt(0);
            Assert.True(true);
        }

        [Fact]
        public void CanInstantiateWithNullValues_01()
        {
            var collection = new TrulyObservableCollection<BindableBaseMock> {new BindableBaseMock(), null};
            Assert.Equal(2, collection.Count);
        }

        [Fact]
        public void CanInstantiateWithNullValues_02()
        {
            var collection = new TrulyObservableCollection<BindableBaseMock>(new[] {new BindableBaseMock(), null});
            Assert.Equal(2, collection.Count);
        }

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
        public void CollectionItemChangedNotified_DetachEventWhenItemRemovedFromList_ViaRemove()
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

        [Fact]
        public void CollectionItemChangedNotified_DetachEventWhenItemRemovedFromList_ViaClear()
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
            testCollection.Clear();

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
                obsCol.CollectionChanged += (x, y) => { changeDetected = true; };

                Assert.False(changeDetected);
                origin[0].StringProperty = "Another Value";
                Assert.True(changeDetected);
            }
        }

        public class AddRange
        {
            [Fact]
            public void ArgumentNullExceptionForNullItems()
            {
                var collection = new TrulyObservableCollection<BindableBaseMock>();
                Assert.Throws<ArgumentNullException>("collection", () => collection.AddRange(null));
            }

            [Fact]
            public void NoEventAndNoChangesWhenAddingEmptyCollection()
            {
                var collection = new TrulyObservableCollection<BindableBaseMock>();
                var hasRaised = false;
                collection.CollectionChanged += (e, s) => hasRaised = true;
                collection.AddRange(new List<BindableBaseMock>());
                Assert.False(hasRaised);

            }

            [Fact]
            public void RaisesEventWhenAddingNewItems()
            {
                var collection = new TrulyObservableCollection<BindableBaseMock>();
                var hasRaised = false;
                collection.CollectionChanged += (e, s) => hasRaised = true;
                collection.AddRange(new[] { new BindableBaseMock() });
                Assert.True(hasRaised);
            }

            [Fact]
            public void AddsItems()
            {
                var collection = new TrulyObservableCollection<BindableBaseMock>();
                Assert.Empty(collection);
                collection.AddRange(new[] { new BindableBaseMock() });
                Assert.Single(collection);
            }
        }

        public class InsertRange
        {
            [Fact]
            public void ArgumentNullExceptionForNullItems()
            {
                var collection = new TrulyObservableCollection<BindableBaseMock>();
                Assert.Throws<ArgumentNullException>("collection", () => collection.InsertRange(0, null));
            }

            [Fact]
            public void ArgumentOutOfRangeExceptionForNegativeIndex()
            {
                var collection = new TrulyObservableCollection<BindableBaseMock>();
                var items = new List<BindableBaseMock> { new BindableBaseMock() };
                Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.InsertRange(-1, items));
            }

            [Fact]
            public void ArgumentOutOfRangeExceptionForTooHighIndex()
            {
                var collection = new TrulyObservableCollection<BindableBaseMock>();
                var items = new List<BindableBaseMock> { new BindableBaseMock() };
                Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.InsertRange(1, items));
            }

            [Fact]
            public void RaisesEventWhenAddingNewItems_SingleNewItem()
            {
                var collection = new TrulyObservableCollection<BindableBaseMock>();
                var eventCount = 0;
                collection.CollectionChanged += (e, s) => eventCount++;
                collection.InsertRange(0, new[] { new BindableBaseMock() });
                Assert.Equal(1, eventCount);
            }

            [Fact]
            public void RaisesEventWhenAddingNewItems_MultipleNewItems()
            {
                var collection = new TrulyObservableCollection<BindableBaseMock>();
                var eventCount = 0;
                collection.CollectionChanged += (e, s) => eventCount++;
                collection.InsertRange(0, new[] { new BindableBaseMock(), new BindableBaseMock() });
                Assert.Equal(1, eventCount);
            }

            [Fact]
            public void AddsItems_AtEnd()
            {
                var collection = new TrulyObservableCollection<BindableBaseMock>();
                collection.Add(new BindableBaseMock { StringProperty = "Prev" });
                collection.Add(new BindableBaseMock { StringProperty = "Prev" });
                collection.Add(new BindableBaseMock { StringProperty = "Prev" });
                collection.InsertRange(3, new List<BindableBaseMock> { new BindableBaseMock { StringProperty = "New" } });
                Assert.Equal(4, collection.Count);
                Assert.Equal("New", collection[3].StringProperty);
            }

            [Fact]
            public void AddsItems_AtStart()
            {
                var collection = new TrulyObservableCollection<BindableBaseMock>();
                collection.Add(new BindableBaseMock { StringProperty = "Prev" });
                collection.Add(new BindableBaseMock { StringProperty = "Prev" });
                collection.Add(new BindableBaseMock { StringProperty = "Prev" });
                collection.InsertRange(0, new List<BindableBaseMock> { new BindableBaseMock { StringProperty = "New" } });
                Assert.Equal(4, collection.Count);
                Assert.Equal("New", collection[0].StringProperty);
            }

            [Fact]
            public void AddsItems_Inside()
            {
                var collection = new TrulyObservableCollection<BindableBaseMock>();
                collection.Add(new BindableBaseMock { StringProperty = "Prev" });
                collection.Add(new BindableBaseMock { StringProperty = "Prev" });
                collection.Add(new BindableBaseMock { StringProperty = "Prev" });
                collection.InsertRange(2, new List<BindableBaseMock> { new BindableBaseMock { StringProperty = "New" } });
                Assert.Equal(4, collection.Count);
                Assert.Equal("New", collection[2].StringProperty);
            }

            [Fact]
            public void SubscribesToEventsInNewItemsAddedAndCorrectlyManagesEventLifetime()
            {
                var collection = new TrulyObservableCollection<BindableBaseMock>();
                var changedCount = 0;
                collection.CollectionChanged += (s, e) => changedCount++;
                var newItem = new BindableBaseMock();
                collection.InsertRange(0, new [] { newItem });

                changedCount = 0;
                newItem.StringProperty = "New";
                Assert.Equal(1, changedCount);

                collection.Clear();
                Assert.Equal(2, changedCount);

                newItem.StringProperty = "Another";
                Assert.Equal(2, changedCount);
            }
        }

        private class TrulyObservableCollectionMock
        {
            private TrulyObservableCollection<BindableBaseMock> _mockCollection;

            public TrulyObservableCollection<BindableBaseMock> MockCollection => _mockCollection ?? (_mockCollection = new TrulyObservableCollection<BindableBaseMock>());
        }
    }
}
