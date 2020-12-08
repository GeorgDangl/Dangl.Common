using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Xunit;

namespace Dangl.Common.Tests
{
    public class ObservableDictionaryTests
    {
        private NotifyCollectionChangedEventArgs _catchedEventArgs;
        private bool _eventCatched;
        private int _eventCatchedCount;
        private NotifyCollectionChangedAction _action;
        private List<KeyValuePair<string, string>> _newItems = new List<KeyValuePair<string, string>>();
        private List<KeyValuePair<string, string>> _oldItems = new List<KeyValuePair<string, string>>();

        [Fact]
        public void CanNotAddNull()
        {
            var instance = new ObservableDictionary<string, string>();
            Assert.Throws<ArgumentNullException>(() => instance.Add(null, null));
        }

        [Fact]
        public void CanNotInstantiateWithNullValues()
        {
            Assert.Throws<ArgumentNullException>(() => new ObservableDictionary<string, string>
            {
                {null, null }
            });
        }

        [Fact]
        public void CollectionChangedNotified_OnAdd()
        {
            var instance = new ObservableDictionary<string, string>();
            instance.CollectionChanged += TestCollection_CollectionChanged;
            _eventCatched = false;
            instance.Add("hello", "world");
            Assert.True(_eventCatched);
            Assert.Equal(1, _eventCatchedCount);
            Assert.Single(_newItems);
            Assert.Empty(_oldItems);
            Assert.Equal(NotifyCollectionChangedAction.Add, _action);
        }

        [Fact]
        public void CollectionChangedNotified_OnAddViaIndex()
        {
            var instance = new ObservableDictionary<string, string>();
            instance.CollectionChanged += TestCollection_CollectionChanged;
            _eventCatched = false;
            instance["hello"] = "world";
            Assert.True(_eventCatched);
            Assert.Equal(1, _eventCatchedCount);
            Assert.Single(_newItems);
            Assert.Empty(_oldItems);
            Assert.Equal(NotifyCollectionChangedAction.Add, _action);
        }

        [Fact]
        public void CollectionChangedNotified_OnChangeViaIndex()
        {
            var instance = new ObservableDictionary<string, string>();
            instance.Add("hello", "world");
            instance.CollectionChanged += TestCollection_CollectionChanged;
            _eventCatched = false;
            instance["hello"] = "world";
            Assert.True(_eventCatched);
            Assert.Equal(1, _eventCatchedCount);
            Assert.Single(_newItems);
            Assert.Single(_oldItems);
            Assert.Equal(NotifyCollectionChangedAction.Replace, _action);
        }

        [Fact]
        public void CollectionChangedNotified_OnRemove()
        {
            var instance = new ObservableDictionary<string, string>();
            instance.Add("hello", "world");
            instance.CollectionChanged += TestCollection_CollectionChanged;
            _eventCatched = false;
            instance.Remove("hello");
            Assert.True(_eventCatched);
            Assert.Equal(1, _eventCatchedCount);
            Assert.Empty(_newItems);
            Assert.Single(_oldItems);
            Assert.Equal(NotifyCollectionChangedAction.Remove, _action);
        }

        [Fact]
        public void CollectionChangedNotified_OnClear()
        {
            var instance = new ObservableDictionary<string, string>();
            instance.Add("hello", "world");
            instance.CollectionChanged += TestCollection_CollectionChanged;
            _eventCatched = false;
            instance.Clear();
            Assert.True(_eventCatched);
            Assert.Equal(1, _eventCatchedCount);
            Assert.Empty(_newItems);
            Assert.Empty(_oldItems);
            Assert.Equal(NotifyCollectionChangedAction.Reset, _action);
        }

        private void TestCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _catchedEventArgs = e;
            _eventCatchedCount++;
            _eventCatched = true;
            if (e.NewItems != null)
            {
                _newItems.AddRange(e.NewItems.Cast<KeyValuePair<string, string>>());
            }
            if (e.OldItems != null)
            {
                _oldItems.AddRange(e.OldItems.Cast<KeyValuePair<string, string>>());
            }
            _action = e.Action;
        }
    }
}
