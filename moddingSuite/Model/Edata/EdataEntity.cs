using System.Globalization;
using moddingSuite.BL;
using moddingSuite.ViewModel.Base;

namespace moddingSuite.Model.Edata
{
    public class EdataEntity : ViewModelBase
    {
        private int _fileEntrySize;
        private int _groupId;
        private string _name;

        public EdataEntity(EdataManager mgr)
        {
            Manager = mgr;
        }

        public EdataManager Manager { get; protected set; }

        public int GroupId
        {
            get { return _groupId; }
            set
            {
                _groupId = value;
                OnPropertyChanged("GroupId");
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public int FileEntrySize
        {
            get { return _fileEntrySize; }
            set
            {
                _fileEntrySize = value;
                OnPropertyChanged("FileEntrySize");
            }
        }

        public override string ToString()
        {
            return Name.ToString(CultureInfo.CurrentCulture);
        }
    }
}