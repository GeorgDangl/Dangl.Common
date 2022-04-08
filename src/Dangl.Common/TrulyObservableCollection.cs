/*
 * Taken from simon's answer at:
 * http://stackoverflow.com/questions/1427471/observablecollection-not-noticing-when-item-in-it-changes-even-with-inotifyprop
 *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Dangl
{
    /// <summary>
    /// This class extends the <see cref="System.Collections.ObjectModel.ObservableCollection{T}"/> for generic content types that
    /// implement <see cref="INotifyPropertyChanged"/> and does send the <see cref="System.Collections.ObjectModel.ObservableCollection{T}.CollectionChanged"/>
    /// event also when an element of the collection changes.
    /// This is the intended behaviour of the <see cref="System.Collections.ObjectModel.ObservableCollection{T}"/> and documented as such in
    /// the MSDN, however, it does not work as intended.
    /// </summary>
    /// <typeparam name="T">Collection type that implements <see cref="INotifyPropertyChanged"/>.</typeparam>
    public sealed class TrulyObservableCollection<T> : ObservableCollection<T>
    where T : INotifyPropertyChanged
    {
        /// <summary>
        /// Will initialize with a hook to the <see cref="System.Collections.ObjectModel.ObservableCollection{T}.CollectionChanged"/> event and add or remove event listeners
        /// to the items in the collection.
        /// </summary>
        public TrulyObservableCollection()
        {
            CollectionChanged += FullObservableCollectionCollectionChanged;
        }

        /// <summary>
        /// Will initialize with a hook to the <see cref="System.Collections.ObjectModel.ObservableCollection{T}.CollectionChanged"/> event and add or remove event listeners
        /// to the items in the collection.
        /// </summary>
        /// <param name="collection">The <see cref="IEnumerable{T}"/> from which to seed.</param>
        public TrulyObservableCollection(IEnumerable<T> collection)
            : base(collection)
        {
            CollectionChanged += FullObservableCollectionCollectionChanged;
            foreach (var instantiatedItem in this)
            {
                if (instantiatedItem != null)
                {
                    instantiatedItem.PropertyChanged += ItemPropertyChanged;
                }
            }
        }

        /// <summary>
        /// This first removes all event subscriptions to 'PropertyChanged' on the
        /// 'Items' and then calls the base method.
        /// </summary>
        protected override void ClearItems()
        {
            if (Items != null)
            {
                foreach (var item in Items.Where(i => i != null))
                {
                    item.PropertyChanged -= ItemPropertyChanged;
                }
            }

            base.ClearItems();
        }

        private void FullObservableCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    if (item != null)
                    {
                        ((INotifyPropertyChanged) item).PropertyChanged += ItemPropertyChanged;
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (item != null)
                    {
                        ((INotifyPropertyChanged) item).PropertyChanged -= ItemPropertyChanged;
                    }
                }
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, sender, sender, IndexOf((T)sender));
            OnCollectionChanged(args);
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="ObservableCollection{T}"/>.
        /// </summary>
        /// <param name="collection">
        /// The collection whose elements should be added to the end of the <see cref="ObservableCollection{T}"/>.
        /// The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public void AddRange(IEnumerable<T> collection)
        {
            InsertRange(Count, collection);
        }

        /// <summary>
        /// Inserts the elements of a collection into the <see cref="ObservableCollection{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="collection">The collection whose elements should be inserted into the List&lt;T&gt;.
        /// The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>            
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not in the collection range.</exception>
        public void InsertRange(int index, IEnumerable<T> collection)
        {
            // This code was taken from GitHub originally and slightly modified:
            // https://gist.github.com/weitzhandler/65ac9113e31d12e697cb58cd92601091
            // It's original from this StackOverflow post:
            // https://stackoverflow.com/questions/670577/observablecollection-doesnt-support-addrange-method-so-i-get-notified-for-each/45364074#45364074

            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (index > Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (collection is ICollection<T> countable)
            {
                if (countable.Count == 0)
                {
                    return;
                }
            }
            else if (!collection.Any())
            {
                return;
            }

            CheckReentrancy();

            //expand the following couple of lines when adding more constructors.
            var target = (List<T>)Items;
            target.InsertRange(index, collection);

            if (!(collection is IList list))
            {
                list = new List<T>(collection);
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list, index));
        }
    }
}
