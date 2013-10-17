using System.Globalization;
using moddingSuite.ViewModel.Base;

namespace moddingSuite.Model.Ndfbin
{
    public class NdfStringReference : ViewModelBase
    {
        private int _id;
        private long _offset;
        private string _value;

        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged(() => Id);
            }
        }

        public long Offset
        {
            get { return _offset; }
            set
            {
                _offset = value;
                OnPropertyChanged(() => Offset);
            }
        }

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged(() => Value);
            }
        }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}