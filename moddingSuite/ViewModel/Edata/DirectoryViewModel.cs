using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using moddingSuite.ViewModel.Base;

namespace moddingSuite.ViewModel.Edata
{
    public class DirectoryViewModel : FileSystemItemViewModel
    {
        private ObservableCollection<FileSystemItemViewModel> _items = new ObservableCollection<FileSystemItemViewModel>();
        private  DirectoryInfo _directoryInfo;

        public DirectoryViewModel(DirectoryInfo info)
        {
            _directoryInfo = info;
            OpenInFileExplorerCommand = new ActionCommand(OpenInFileExplorerExecute);
        }

        private void OpenInFileExplorerExecute(object obj)
        {
            Process.Start(this.Info.FullName);
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

        public ICommand OpenInFileExplorerCommand
        {
            get; set;
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
