﻿// **************************************
// *									*
// *  Last change: 2015-04-19			*
// *  © 2015 Georg Dangl				*
// *  info@georgdangl.de				*
// *									*
// **************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Dangl
{
    public abstract class BindableBase : INotifyPropertyChanged, IDisposable
    {
        protected BindableBase()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Checks if a property already matches a desired value.  Sets the property and
        ///     notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="Storage">Reference to a property with both getter and setter.</param>
        /// <param name="Value">Desired value for the property.</param>
        /// <param name="PropertyName">
        ///     Name of the property used to notify listeners.  This
        ///     value is optional and can be provided automatically when invoked from compilers that
        ///     support CallerMemberName.
        /// </param>
        /// <returns>
        ///     True if the value was changed, false if the existing value matched the
        ///     desired value.
        /// </returns>
        protected bool SetProperty<T>(ref T Storage, T Value, [CallerMemberName] String PropertyName = null)
        {
            if (Equals(Storage, Value))
            {
                return false;
            }

            Storage = Value;
            this.OnPropertyChanged(PropertyName);
            return true;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChangedEventHandler Handler = this.PropertyChanged;
            if (Handler != null)
            {
                Handler(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        public void Dispose()
        {
            this.OnDispose();
        }

        protected virtual void OnDispose()
        {
        }
    }
}
