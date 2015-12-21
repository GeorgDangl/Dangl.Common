using Xunit;
using System;
using System.Collections.Generic;

namespace Dangl.Test.Common
{
     
    public class TrulyObservableCollection_Test
    {
         
        public class Constructor
        {
            [Fact]
            public void InstantiationFromIEnumerable()
            {
                List<MockClass> TempList = new List<MockClass> { new MockClass() { StringProperty = "One" }, new MockClass() { StringProperty = "Two" }, new MockClass() { StringProperty = "Three" } };
                TrulyObservableCollection<MockClass> ObsCol = new TrulyObservableCollection<MockClass>(TempList);
                Assert.Equal(TempList[0].StringProperty, ObsCol[0].StringProperty);
                Assert.Equal(TempList[1].StringProperty, ObsCol[1].StringProperty);
                Assert.Equal(TempList[2].StringProperty, ObsCol[2].StringProperty);
            }

            [Fact]
            public void InstantiateAndGetEventHooks()
            {
                List<MockClass> TempList = new List<MockClass> { new MockClass() { StringProperty = "One" }, new MockClass() { StringProperty = "Two" }, new MockClass() { StringProperty = "Three" } };
                TrulyObservableCollection<MockClass> Origin = new TrulyObservableCollection<MockClass>();
                foreach (var CurrentItem in TempList)
                {
                    Origin.Add(CurrentItem);
                }
                TrulyObservableCollection<MockClass> ObsCol = new TrulyObservableCollection<MockClass>(Origin);
                Assert.True(Comparator.GetCompareLogic().Compare(Origin, ObsCol).AreEqual, Comparator.GetCompareLogic().Compare(Origin, ObsCol).DifferencesString);
            }
        }

        private bool EventCatched;

        private System.Collections.Specialized.NotifyCollectionChangedEventArgs CatchedEventArgs;

        private int EventCatchedCount;

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

        [Fact]
        public void CollectionChangedNotifiedWhenInClass()
        {
            MockClass_TrulyObservableCollection Instance = new MockClass_TrulyObservableCollection();
            Instance.MockCollection.CollectionChanged += TestCollection_CollectionChanged;
            Instance.MockCollection.Add(new MockClass());
            Assert.True(EventCatched);
            Assert.Equal(1, EventCatchedCount);
        }

        [Fact]
        public void CollectionChangedNotified()
        {
            TrulyObservableCollection<MockClass> TestCollection = new TrulyObservableCollection<MockClass>();
            TestCollection.CollectionChanged += TestCollection_CollectionChanged;
            EventCatched = false;
            TestCollection.Add(new MockClass());
            Assert.True(EventCatched);
            Assert.Equal(1, EventCatchedCount);
        }

        [Fact]
        public void CollectionItemChangedNotified()
        {
            TrulyObservableCollection<MockClass> TestCollection = new TrulyObservableCollection<MockClass>();
            TestCollection.Add(new MockClass());
            TestCollection.CollectionChanged += TestCollection_CollectionChanged;
            EventCatched = false;
            CatchedEventArgs = null;
            TestCollection[0].StringProperty = "Changed";
            Assert.True(EventCatched);
            Assert.NotNull(CatchedEventArgs);
            Assert.Equal(CatchedEventArgs.Action, System.Collections.Specialized.NotifyCollectionChangedAction.Replace);
            Assert.Same(CatchedEventArgs.NewItems[0], TestCollection[0]);
            Assert.Equal(1, EventCatchedCount);
        }

        [Fact]
        public void CollectionItemChangedNotified_OnInstanceFromIEnumerable_01()
        {
            TrulyObservableCollection<MockClass> TestCollection = new TrulyObservableCollection<MockClass>();
            TestCollection.Add(new MockClass());
            TestCollection.Add(new MockClass());
            TestCollection.Add(new MockClass());
            TestCollection.Add(new MockClass());
            TrulyObservableCollection<MockClass> CopiedInstance = new TrulyObservableCollection<MockClass>(TestCollection);

            CopiedInstance.CollectionChanged += TestCollection_CollectionChanged;
            EventCatched = false;
            CatchedEventArgs = null;
            CopiedInstance[0].StringProperty = "Changed";
            Assert.True(EventCatched);
            Assert.NotNull(CatchedEventArgs);
            Assert.Equal(CatchedEventArgs.Action, System.Collections.Specialized.NotifyCollectionChangedAction.Replace);
            Assert.Same(CatchedEventArgs.NewItems[0], CopiedInstance[0]);
            Assert.Equal(1, EventCatchedCount);
        }

        private void TestCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CatchedEventArgs = e;
            EventCatchedCount++;
            EventCatched = true;
        }
    }
}
