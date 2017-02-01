using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.ViewModel.Edata
{
    public class FileViewModel : FileSystemItemViewModel
    {
        private FileInfo _fileInfo;

        public FileViewModel(FileInfo info)
        {
            _fileInfo = info;
        }

        public FileInfo Info
        {
            get { return _fileInfo; }
            set
            {
                _fileInfo = value;
                
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
