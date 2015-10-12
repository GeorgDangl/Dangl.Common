using System;
using Dangl;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dangl.Tests
{
    /// <summary>Diese Klasse enthält parametrisierte Komponententests für BindableBase.</summary>
    [TestClass]
    [PexClass(typeof(BindableBase))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class BindableBaseTest
    {

        /// <summary>Test-Stub für Dispose()</summary>
        [PexMethod]
        public void DisposeTest([PexAssumeNotNull]BindableBase target)
        {
            target.Dispose();
            // TODO: Assertionen zu Methode BindableBaseTest.DisposeTest(BindableBase) hinzufügen
        }
    }
}
