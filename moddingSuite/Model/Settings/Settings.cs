using System.Collections.Generic;
using moddingSuite.ViewModel.Base;

namespace moddingSuite.Model.Settings
{
    public class Settings : ViewModelBase
    {
        private string _lastOpenFolder = @"C:\";
        private List<string> _lastOpenedFile = new List<string>();
        private string _savePath = @"C:\";
        private int _lastHighlightedFileIndex;
        private string _wargamePath;
        private string _pythonPath;

        public string SavePath
        {
            get { return _savePath; }
            set
            {
                _savePath = value;
                OnPropertyChanged(() => SavePath);
            }
        }

        public List<string> LastOpenedFiles
        {
            get { return _lastOpenedFile; }
            set
            {
                _lastOpenedFile = value;
                OnPropertyChanged(() => LastOpenedFiles);
            }
        }

        public string LastOpenFolder
        {
            get { return _lastOpenFolder; }
            set { _lastOpenFolder = value; OnPropertyChanged(() => LastOpenedFiles); }
        }

        public int LastHighlightedFileIndex
        {
            get { return _lastHighlightedFileIndex; }
            set { _lastHighlightedFileIndex = value; OnPropertyChanged(() => LastOpenedFiles); }
        }

        public string WargamePath
        {
            get { return _wargamePath; }
            set { _wargamePath = value; OnPropertyChanged(() => WargamePath); }
        }

        public string PythonPath
        {
            get { return _pythonPath; }
            set { _pythonPath = value; OnPropertyChanged(() => PythonPath); }
        }
    }
}