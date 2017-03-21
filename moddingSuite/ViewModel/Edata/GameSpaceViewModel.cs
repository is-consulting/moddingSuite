using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using moddingSuite.BL;
using moddingSuite.Model.Settings;
using moddingSuite.ViewModel.Base;

namespace moddingSuite.ViewModel.Edata
{
    public class GameSpaceViewModel : FileSystemOverviewViewModelBase
    {
        public GameSpaceViewModel(Settings settings)
        {
            RootPath = settings.WargamePath;

            if (Directory.Exists(RootPath))
                Root.Add(ParseRoot());
        }
    }
}
