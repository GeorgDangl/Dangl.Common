using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Dangl
{
    /// <summary>
    /// Implements <see cref="INotifyPropertyChanged"/> and <see cref="IDisposable"/>.
    /// Extension of Prisms BindableBase, see https://github.com/PrismLibrary/Prism/blob/master/Source/Prism/Mvvm/BindableBase.cs
    /// Adds overloads for SetProperty to automatically bind to INotifiyPropertyChanged and INotifyCollectionChanged events
    /// See the Prism Library license at https://github.com/PrismLibrary/Prism/blob/master/LICENSE
    /// </summary>
    public abstract class BindableBase : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// <see cref="INotifyPropertyChanged"/> implementation.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Checks if a property already matches a desired value.  Sets the property and
        ///     notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">
        ///     Name of the property used to notify listeners.  This
        ///     value is optional and can be provided automatically when invoked from compilers that
        ///     support CallerMemberName.
        /// </param>
        /// <returns>
        ///     True if the value was changed, false if the existing value matched the
        ///     desired value.
        /// </returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        ///     Checks if a property already matches a desired value.  Sets the property and
        ///     notifies listeners only when necessary.
        ///     Will automatically attach the <see cref="PropertyChangedEventHandler"/> to the new value
        ///     and detach it from the old value.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="changeEventHook"></param>
        /// <param name="propertyName">
        ///     Name of the property used to notify listeners.  This
        ///     value is optional and can be provided automatically when invoked from compilers that
        ///     support CallerMemberName.
        /// </param>
        /// <returns>
        ///     True if the value was changed, false if the existing value matched the
        ///     desired value.
        /// </returns>
        protected bool SetProperty<T>(ref T storage, T value, PropertyChangedEventHandler changeEventHook, [CallerMemberName] string propertyName = null) where T : INotifyPropertyChanged
        {
            if (Equals(storage, value))
            {
                return false;
            }
            if (storage != null)
            {
                storage.PropertyChanged -= changeEventHook;
            }
            storage = value;
            if (storage != null)
            {
                storage.PropertyChanged += changeEventHook;
            }
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        ///     Checks if a property already matches a desired value.  Sets the property and
        ///     notifies listeners only when necessary.
        ///     Will automatically attach the <see cref="NotifyCollectionChangedEventHandler"/> to the new value
        ///     and detach it from the old value.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="changeEventHook"></param>
        /// <param name="propertyName">
        ///     Name of the property used to notify listeners.  This
        ///     value is optional and can be provided automatically when invoked from compilers that
        ///     support CallerMemberName.
        /// </param>
        /// <returns>
        ///     True if the value was changed, false if the existing value matched the
        ///     desired value.
        /// </returns>
        protected bool SetProperty<T>(ref T storage, T value, NotifyCollectionChangedEventHandler changeEventHook, [CallerMemberName] string propertyName = null) where T : INotifyCollectionChanged
        {
            if (Equals(storage, value))
            {
                return false;
            }
            if (storage != null)
            {
                storage.CollectionChanged -= changeEventHook;
            }
            storage = value;
            if (storage != null)
            {
                storage.CollectionChanged += changeEventHook;
            }
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Event to be raised for <see cref="INotifyPropertyChanged"/>.
        /// </summary>
        /// <param name="propertyName">Optional, when not given the <see cref="System.Runtime.CompilerServices.CallerMemberNameAttribute"/> is used to determine
        /// the calling function.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Implementation of <see cref="IDisposable"/>. Will always call the <see cref="OnDispose"/> method that
        /// may be used in derived classes to implement behaviour upon being disposed, such as releasing event
        /// handler listeners.
        /// </summary>
        public void Dispose()
        {
            OnDispose();
        }

        /// <summary>
        /// This method is called by the <see cref="Dispose"/> method upon disposing of
        /// this class via the <see cref="IDisposable"/> interface.
        /// </summary>
        protected virtual void OnDispose()
        {
            PropertyChanged = null;
        }
    }
}
