using System.Globalization;
using moddingSuite.ViewModel.Base;

namespace moddingSuite.Model.Edata
{
    public abstract class EdataEntity : ViewModelBase
    {
        private int _fileEntrySize;
        private string _name;

        public EdataEntity(string name)
        {
            Name = name;
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