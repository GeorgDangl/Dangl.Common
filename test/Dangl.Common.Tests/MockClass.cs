using System.Collections.ObjectModel;

namespace Dangl.Common.Tests
{
    public class MockClass : BindableBase
    {
        #region Fields

        private string _StringProperty;

        private MockClass _ComplexProperty;

        #endregion Fields

        #region Properties

        public string StringProperty
        {
            get
            {
                return _StringProperty;
            }
            set
            {
                SetProperty(ref _StringProperty, value);
            }
        }

        public MockClass ComplexProperty
        {
            get
            {
                return _ComplexProperty;
            }
            set
            {
                SetProperty(ref _ComplexProperty, value);
            }
        }

        #endregion Properties
    }
}