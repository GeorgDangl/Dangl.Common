using System.Collections.Generic;
using System.Collections.Specialized;
using Xunit;

namespace Dangl.Common.Tests
{
    public class TrulyObservableCollectionTests
    {
        private NotifyCollectionChangedEventArgs CatchedEventArgs;
        private bool EventCatched;
        private int EventCatchedCount;

        [Fact]
        public void CollectionChangedNotifiedWhenInClass()
        {
            var Instance = new MockClass_TrulyObservableCollection();
            Instance.MockCollection.CollectionChanged += TestCollection_CollectionChanged;
            Instance.MockCollection.Add(new MockClass());
            Assert.True(EventCatched);
            Assert.Equal(1, EventCatchedCount);
        }

        [Fact]
        public void CollectionChangedNotified()
        {
            var TestCollection = new TrulyObservableCollection<MockClass>();
            TestCollection.CollectionChanged += TestCollection_CollectionChanged;
            EventCatched = false;
            TestCollection.Add(new MockClass());
            Assert.True(EventCatched);
            Assert.Equal(1, EventCatchedCount);
        }

        [Fact]
        public void CollectionItemChangedNotified()
        {
            var TestCollection = new TrulyObservableCollection<MockClass>();
            TestCollection.Add(new MockClass());
            TestCollection.CollectionChanged += TestCollection_CollectionChanged;
            EventCatched = false;
            CatchedEventArgs = null;
            TestCollection[0].StringProperty = "Changed";
            Assert.True(EventCatched);
            Assert.NotNull(CatchedEventArgs);
            Assert.Equal(CatchedEventArgs.Action, NotifyCollectionChangedAction.Replace);
            Assert.Same(CatchedEventArgs.NewItems[0], TestCollection[0]);
            Assert.Equal(1, EventCatchedCount);
        }

        [Fact]
        public void CollectionItemChangedNotified_OnInstanceFromIEnumerable_01()
        {
            var TestCollection = new TrulyObservableCollection<MockClass>();
            TestCollection.Add(new MockClass());
            TestCollection.Add(new MockClass());
            TestCollection.Add(new MockClass());
            TestCollection.Add(new MockClass());
            var CopiedInstance = new TrulyObservableCollection<MockClass>(TestCollection);

            CopiedInstance.CollectionChanged += TestCollection_CollectionChanged;
            EventCatched = false;
            CatchedEventArgs = null;
            CopiedInstance[0].StringProperty = "Changed";
            Assert.True(EventCatched);
            Assert.NotNull(CatchedEventArgs);
            Assert.Equal(CatchedEventArgs.Action, NotifyCollectionChangedAction.Replace);
            Assert.Same(CatchedEventArgs.NewItems[0], CopiedInstance[0]);
            Assert.Equal(1, EventCatchedCount);
        }

        [Fact]
        public void CollectionItemChangedNotified_DetachEventWhenItemRemovedFromList()
        {
            var TestCollection = new TrulyObservableCollection<MockClass>();
            var mockInstance = new MockClass();
            TestCollection.Add(mockInstance);
            TestCollection.CollectionChanged += TestCollection_CollectionChanged;
            Assert.False(EventCatched);
            mockInstance.StringProperty = "First Change";
            Assert.True(EventCatched);
            EventCatched = false;
            Assert.False(EventCatched);
            TestCollection.Remove(mockInstance);
            Assert.True(EventCatched);
            EventCatched = false;
            Assert.False(EventCatched);
            mockInstance.StringProperty = "Second Change";
            Assert.False(EventCatched);
        }

        private void TestCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CatchedEventArgs = e;
            EventCatchedCount++;
            EventCatched = true;
        }

        public class Constructor
        {
            [Fact]
            public void InstantiationFromIEnumerable()
            {
                var TempList = new List<MockClass> {new MockClass {StringProperty = "One"}, new MockClass {StringProperty = "Two"}, new MockClass {StringProperty = "Three"}};
                var ObsCol = new TrulyObservableCollection<MockClass>(TempList);
                Assert.Equal(TempList[0].StringProperty, ObsCol[0].StringProperty);
                Assert.Equal(TempList[1].StringProperty, ObsCol[1].StringProperty);
                Assert.Equal(TempList[2].StringProperty, ObsCol[2].StringProperty);
            }

            [Fact]
            public void InstantiateAndGetEventHooks()
            {
                var TempList = new List<MockClass> {new MockClass {StringProperty = "One"}, new MockClass {StringProperty = "Two"}, new MockClass {StringProperty = "Three"}};
                var Origin = new TrulyObservableCollection<MockClass>();
                foreach (var CurrentItem in TempList)
                {
                    Origin.Add(CurrentItem);
                }
                var ObsCol = new TrulyObservableCollection<MockClass>(Origin);
                Assert.True(Comparator.GetCompareLogic().Compare(Origin, ObsCol).AreEqual, Comparator.GetCompareLogic().Compare(Origin, ObsCol).DifferencesString);
            }
        }

        private class MockClass_TrulyObservableCollection
        {
            private TrulyObservableCollection<MockClass> _MockCollection;

            public TrulyObservableCollection<MockClass> MockCollection
            {
                get
                {
                    return _MockCollection ?? (_MockCollection = new TrulyObservableCollection<MockClass>());
                }
            }
        }
    }
}