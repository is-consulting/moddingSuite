using moddingSuite.ViewModel.Base;

namespace moddingSuite.ViewModel.Filter
{
    public class PropertyFilterExpression : ViewModelBase
    {
        private string _propertyName;
        private string _value;

        private FilterDiscriminator _discriminator = FilterDiscriminator.Equals;

        public string PropertyName
        {
            get { return _propertyName; }
            set { _propertyName = value; OnPropertyChanged(() => PropertyName); }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; OnPropertyChanged(() => Value); }
        }

        public FilterDiscriminator Discriminator
        {
            get { return _discriminator; }
            set { _discriminator = value; OnPropertyChanged(() => Discriminator); }
        }
    }
}
