using System.Globalization;
using moddingSuite.Util;
using moddingSuite.ViewModel.Base;

namespace moddingSuite.Model.Ndfbin
{
    public class NdfProperty : ViewModelBase
    {
        private NdfClass _class;
        private int _id;
        private string _name;
        private long _offset;

        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged(() => Id);
            }
        }

        public string BinId
        {
            get { return Utils.Int32ToBigEndianHexByteString(Id); }
        }

        public NdfClass Class
        {
            get { return _class; }
            set
            {
                _class = value;
                OnPropertyChanged(() => Class);
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

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(() => Name);
            }
        }

        public override string ToString()
        {
            return Name.ToString(CultureInfo.InvariantCulture);
        }
    }
}