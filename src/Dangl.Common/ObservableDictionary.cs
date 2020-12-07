using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Dangl
{
    /// <summary>
    /// This implementation of <see cref="IDictionary{TKey, TValue}"/> implements
    /// <see cref="INotifyCollectionChanged"/> to emit events when the collection is changed.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged
    {
        private readonly Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

        /// <summary>
        /// Delegates to the underlying <see cref="Dictionary{TKey, TValue}"/> and raises events.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set
            {
                if (_dictionary.TryGetValue(key, out var existing))
                {
                    _dictionary[key] = value;
                    var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, existing);
                    CollectionChanged?.Invoke(this, eventArgs);
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        /// <summary>
        /// Delegates to the underlying <see cref="Dictionary{TKey, TValue}"/> and raises events.
        /// </summary>
        public ICollection<TKey> Keys => _dictionary.Keys;

        /// <summary>
        /// Delegates to the underlying <see cref="Dictionary{TKey, TValue}"/> and raises events.
        /// </summary>
        public ICollection<TValue> Values => _dictionary.Values;

        /// <summary>
        /// Delegates to the underlying <see cref="Dictionary{TKey, TValue}"/> and raises events.
        /// </summary>
        public int Count => _dictionary.Count;

        /// <summary>
        /// Delegates to the underlying <see cref="Dictionary{TKey, TValue}"/> and raises events.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// This event is raised when items in the underlying <see cref="Dictionary{TKey, TValue}"/> change.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Delegates to the underlying <see cref="Dictionary{TKey, TValue}"/> and raises events.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value);
            CollectionChanged?.Invoke(this, eventArgs);
        }

        /// <summary>
        /// Delegates to the underlying <see cref="Dictionary{TKey, TValue}"/> and raises events.
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        /// Delegates to the underlying <see cref="Dictionary{TKey, TValue}"/> and raises events.
        /// </summary>
        public void Clear()
        {
            _dictionary.Clear();
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            CollectionChanged?.Invoke(this, eventArgs);
        }

        /// <summary>
        /// Delegates to the underlying <see cref="Dictionary{TKey, TValue}"/> and raises events.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.ContainsKey(item.Key)
                && (_dictionary[item.Key]?.Equals(item.Value) ?? false);
        }

        /// <summary>
        /// Delegates to the underlying <see cref="Dictionary{TKey, TValue}"/> and raises events.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        /// <summary>
        /// Delegates to the underlying <see cref="Dictionary{TKey, TValue}"/> and raises events.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            int currentIndex = arrayIndex;
            foreach (var pair in _dictionary)
            {
                array[currentIndex++] = pair;
            }
        }

        /// <summary>
        /// Delegates to the underlying <see cref="Dictionary{TKey, TValue}"/> and raises events.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        /// <summary>
        /// Delegates to the underlying <see cref="Dictionary{TKey, TValue}"/> and raises events.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(TKey key)
        {
            if (_dictionary.TryGetValue(key, out var oldValue))
            {
                var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldValue);
                var hasRemoved = _dictionary.Remove(key);
                if (hasRemoved)
                {
                    CollectionChanged?.Invoke(this, eventArgs);
                }

                return hasRemoved;
            }

            return false;
        }

        /// <summary>
        /// Delegates to the underlying <see cref="Dictionary{TKey, TValue}"/> and raises events.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        /// <summary>
        /// Delegates to the underlying <see cref="Dictionary{TKey, TValue}"/> and raises events.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        /// <summary>
        /// Delegates to the underlying <see cref="Dictionary{TKey, TValue}"/> and raises events.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }
    }
}
