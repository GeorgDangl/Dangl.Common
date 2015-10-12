using System.ComponentModel;
// <copyright file="TrulyObservableCollectionTTest.cs">Copyright ©  2015</copyright>

using System;
using Dangl;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dangl.Tests
{
    [TestClass]
    [PexClass(typeof(TrulyObservableCollection<>))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class TrulyObservableCollectionTTest
    {

        [PexGenericArguments(typeof(INotifyPropertyChanged))]
        [PexMethod]
        public TrulyObservableCollection<T> Constructor<T>()
            where T : INotifyPropertyChanged
        {
            TrulyObservableCollection<T> target = new TrulyObservableCollection<T>();
            return target;
            // TODO: Assertionen zu Methode TrulyObservableCollectionTTest.Constructor() hinzufügen
        }
    }
}
