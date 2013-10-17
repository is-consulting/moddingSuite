using moddingSuite.Model.Ndfbin.Types;
using moddingSuite.ViewModel.Base;

namespace moddingSuite.Model.Ndfbin.ChangeManager
{
    public class ChangeEntry : ViewModelBase
    {
        private IValueHolder _changedValue;
        private byte[] _newValue;
        private byte[] _oldValue;
        private bool _triggersOffsetreordering;

        public bool TriggersOffsetreordering
        {
            get { return _triggersOffsetreordering; }
            set
            {
                _triggersOffsetreordering = value;
                OnPropertyChanged(() => TriggersOffsetreordering);
            }
        }

        public object OldValueDisplay
        {
            get
            {
                if (ChangedValue == null)
                    return null;

                return NdfTypeManager.GetValue(OldValue, ChangedValue.Value.Type, ChangedValue.Manager, 0);
            }
        }

        public byte[] OldValue
        {
            get { return _oldValue; }
            set
            {
                _oldValue = value;
                OnPropertyChanged(() => OldValue);
            }
        }

        public byte[] NewValue
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