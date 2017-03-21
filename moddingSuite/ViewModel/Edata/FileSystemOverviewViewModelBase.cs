using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using moddingSuite.ViewModel.Base;

namespace moddingSuite.ViewModel.Edata
{
    public abstract class FileSystemOverviewViewModelBase : ViewModelBase
    {
        private string _rootPath;
        private readonly ObservableCollection<DirectoryViewModel> _root = new ObservableCollection<DirectoryViewModel>();

        public string RootPath
        {
            get { return _rootPath; }
            set
            {
                _rootPath = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<DirectoryViewModel> Root
        {
            get { return _root; }
        }

        protected DirectoryViewModel ParseRoot()
        {
            return ParseDirectory(new DirectoryInfo(RootPath));
        }

        protected DirectoryViewModel ParseDirectory(DirectoryInfo info)
        {
            var dirVm = new DirectoryViewModel(info);

            foreach (var directoryInfo in dirVm.Info.EnumerateDirectories())
            {
                dirVm.Items.Add(ParseDirectory(directoryInfo));
            }

            foreach (var fileInfo in dirVm.Info.EnumerateFiles())
            {
                dirVm.Items.Add(new FileViewModel(fileInfo));
            }

            return dirVm;
        }
    }
}
