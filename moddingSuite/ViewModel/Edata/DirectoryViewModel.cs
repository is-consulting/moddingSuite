using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.ViewModel.Edata
{
    public class DirectoryViewModel : FileSystemItemViewModel
    {
        private ObservableCollection<FileSystemItemViewModel> _items = new ObservableCollection<FileSystemItemViewModel>();
        private  DirectoryInfo _directoryInfo;

        public DirectoryViewModel(DirectoryInfo info)
        {
            _directoryInfo = info;
        }

        public ObservableCollection<FileSystemItemViewModel> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                OnPropertyChanged();
            }
        }

        public DirectoryInfo Info
        {
            get
            {
                return _directoryInfo;
            }
            set
            {
                _directoryInfo = value;
            }
        }

        public override string Name
        {
            get
            {
                return Info.Name;
            }
        }
    }
}
