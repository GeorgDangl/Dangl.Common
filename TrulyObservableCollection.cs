// **************************************
// *									*
// *  Last change: 2015-04-23			*
// *  © 2015 Georg Dangl				*
// *  info@georgdangl.de				*
// *									*
// **************************************

/*
 * Taken from simon's answer at:
 * http://stackoverflow.com/questions/1427471/observablecollection-not-noticing-when-item-in-it-changes-even-with-inotifyprop
 *
 */

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

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

        private void FullObservableCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Object item in e.NewItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged += ItemPropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (Object item in e.OldItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged -= ItemPropertyChanged;
                }
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, sender, sender, IndexOf((T)sender));
            OnCollectionChanged(args);
        }
    }
}
