using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using moddingSuite.ViewModel.Base;

namespace moddingSuite.ViewModel.Edata
{
    public abstract class FileSystemItemViewModel : ViewModelBase
    {
        public abstract string Name { get; }

        public void Invalidate()
        {
            OnPropertyChanged(nameof(Name));
        }
    }
}
