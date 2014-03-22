using moddingSuite.Model.Ndfbin.Types;
using moddingSuite.Model.Ndfbin.Types.AllTypes;
using moddingSuite.ViewModel.Base;

namespace moddingSuite.Model.Ndfbin.ChangeManager
{
    public class ChangeEntry : ViewModelBase
    {
        private IValueHolder _changedValue;
        private NdfValueWrapper _newValue;
        
        public NdfValueWrapper NewValue
        {
            get { return _newValue; }
            set
            {
                _newValue = value;
                OnPropertyChanged(() => NewValue);
            }
        }

        public IValueHolder ChangedValue
        {
            get { return _changedValue; }
            set
            {
                _changedValue = value;
                OnPropertyChanged(() => ChangedValue);
            }
        }
    }
}