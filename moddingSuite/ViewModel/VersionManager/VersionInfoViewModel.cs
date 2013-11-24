using System.Collections.ObjectModel;
using System.IO;
using moddingSuite.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.ViewModel.VersionManager
{
    public class VersionInfoViewModel : ViewModelBase
    {
        private int _version;
        private string _directory;
        private DirectoryInfo _directoryInfo;
        private VersionManagerViewModel _manager;
        private readonly ObservableCollection<VersionFileViewModel> _versionFiles = new ObservableCollection<VersionFileViewModel>();

        public string DirectoryPath
        {
            get { return _directory; }
            set
            {
                _directory = value;
                OnPropertyChanged(() => DirectoryPath);
            }
        }

        public int Version
        {
            get { return _version; }
            set { _version = value; OnPropertyChanged(() => Version); }
        }

        public DirectoryInfo DirectoryInfo
        {
            get { return _directoryInfo; }
            set { _directoryInfo = value; OnPropertyChanged(() => DirectoryInfo); }
        }

        public ObservableCollection<VersionFileViewModel> VersionFiles
        {
            get { return _versionFiles; }
        }

        public VersionManagerViewModel Manager
        {
            get { return _manager; }
            set { _manager = value;  OnPropertyChanged(() => Manager);}
        }

        public VersionInfoViewModel(string path, VersionManagerViewModel manager)
            : this(new DirectoryInfo(path), manager)
        {
        }

        public VersionInfoViewModel(DirectoryInfo directoryInfo, VersionManagerViewModel manager)
        {
            Manager = manager;

            DirectoryInfo = directoryInfo;
            DirectoryPath = directoryInfo.FullName;
            Version = Convert.ToInt32(directoryInfo.Name);

            foreach (var file in directoryInfo.EnumerateFiles())
                VersionFiles.Add(new VersionFileViewModel(file));


        }
    }
}
