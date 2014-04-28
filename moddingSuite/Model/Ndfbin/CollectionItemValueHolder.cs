using moddingSuite.Model.Ndfbin.Types.AllTypes;
using moddingSuite.ViewModel.Base;

namespace moddingSuite.Model.Ndfbin
{
    public class CollectionItemValueHolder : ViewModelBase, IValueHolder
    {
        private NdfValueWrapper _value;

        public CollectionItemValueHolder(NdfValueWrapper value, NdfBinary manager)
        {
            Value = value;
            Manager = manager;
        }

        public virtual NdfBinary Manager
        {
            get;
            private set;
        }

        public virtual NdfValueWrapper Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }
    }
}