
namespace Dangl.Common.Tests
{
    public class BindableBaseMock : BindableBase
    {
        private string _stringProperty;

        private BindableBaseMock _complexProperty;

        public string StringProperty
        {
            get => _stringProperty;
            set => SetProperty(ref _stringProperty, value);
        }

        public BindableBaseMock ComplexProperty
        {
            get => _complexProperty;
            set => SetProperty(ref _complexProperty, value);
        }
    }
}
