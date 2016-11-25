using System.Collections.ObjectModel;

namespace Dangl.Common.Tests
{
    public class MockClass : BindableBase
    {
        #region Fields

        private string _stringProperty;

        private MockClass _complexProperty;

        #endregion Fields

        #region Properties

        public string StringProperty
        {
            get
            {
                return _stringProperty;
            }
            set
            {
                SetProperty(ref _stringProperty, value);
            }
        }

        public MockClass ComplexProperty
        {
            get
            {
                return _complexProperty;
            }
            set
            {
                SetProperty(ref _complexProperty, value);
            }
        }

        #endregion Properties
    }
}