using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Dangl.Test.Common
{
    [TestClass]
    public class TrulyObservableCollection_Test
    {
        [TestClass]
        public class Constructor
        {
            [TestMethod]
            public void InstantiationFromIEnumerable()
            {
                List<MockClass> TempList = new List<MockClass> { new MockClass() { StringProperty = "One" }, new MockClass() { StringProperty = "Two" }, new MockClass() { StringProperty = "Three" } };
                TrulyObservableCollection<MockClass> ObsCol = new TrulyObservableCollection<MockClass>(TempList);
                Assert.AreEqual(TempList[0].StringProperty, ObsCol[0].StringProperty);
                Assert.AreEqual(TempList[1].StringProperty, ObsCol[1].StringProperty);
                Assert.AreEqual(TempList[2].StringProperty, ObsCol[2].StringProperty);
            }

            [TestMethod]
            public void InstantiateAndGetEventHooks()
            {
                List<MockClass> TempList = new List<MockClass> { new MockClass() { StringProperty = "One" }, new MockClass() { StringProperty = "Two" }, new MockClass() { StringProperty = "Three" } };
                TrulyObservableCollection<MockClass> Origin = new TrulyObservableCollection<MockClass>();
                foreach (var CurrentItem in TempList)
                {
                    Origin.Add(CurrentItem);
                }
                TrulyObservableCollection<MockClass> ObsCol = new TrulyObservableCollection<MockClass>(Origin);
                Assert.IsTrue(Comparator.GetCompareLogic().Compare(Origin, ObsCol).AreEqual, Comparator.GetCompareLogic().Compare(Origin, ObsCol).DifferencesString);
            }
        }

        private bool EventCatched;

        [TestMethod]
        public void CollectionChangedNotified()
        {
            TrulyObservableCollection<MockClass> TestCollection = new TrulyObservableCollection<MockClass>();
            TestCollection.CollectionChanged += TestCollection_CollectionChanged;
            EventCatched = false;
            TestCollection.Add(new MockClass());
            Assert.IsTrue(EventCatched);
        }

        [TestMethod]
        public void CollectionItemChangedNotified()
        {
            TrulyObservableCollection<MockClass> TestCollection = new TrulyObservableCollection<MockClass>();
            TestCollection.Add(new MockClass());
            TestCollection.CollectionChanged += TestCollection_CollectionChanged;
            EventCatched = false;
            CatchedEventArgs = null;
            TestCollection[0].StringProperty = "Changed";
            Assert.IsTrue(EventCatched);
            Assert.IsNotNull(CatchedEventArgs);
            Assert.AreEqual(CatchedEventArgs.Action, System.Collections.Specialized.NotifyCollectionChangedAction.Replace);
            Assert.AreSame(CatchedEventArgs.NewItems[0], TestCollection[0]);
        }

        private System.Collections.Specialized.NotifyCollectionChangedEventArgs CatchedEventArgs;

        private void TestCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CatchedEventArgs = e;
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
}
