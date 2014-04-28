using moddingSuite.Model.Ndfbin.Types;
using moddingSuite.Model.Ndfbin.Types.AllTypes;
using moddingSuite.ViewModel.Base;

namespace moddingSuite.Model.Ndfbin.ChangeManager
{
    public class FlatChangeEntry : ChangeEntryBase
    {
        private NdfValueWrapper _newValue;

        public FlatChangeEntry(NdfPropertyValue affectedPropertyValue, NdfValueWrapper newValue)
            : base(affectedPropertyValue)
        {
            NewValue = newValue;
        }

        public NdfValueWrapper NewValue
        {
            get { return _newValue; }
            set
            {
                _newValue = value;
                OnPropertyChanged(() => NewValue);
            }
        }

    }
}