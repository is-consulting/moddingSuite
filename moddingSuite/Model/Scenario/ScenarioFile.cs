using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using moddingSuite.Model.Ndfbin;
using moddingSuite.ViewModel.Base;

namespace moddingSuite.Model.Scenario
{
    public class ScenarioFile : ViewModelBase
    {
        private int _version;
        private byte[] _checksum;
        private NdfBinary _ndfBinary;
        private List<byte[]> _contentFiles = new List<byte[]>();
        private AreaFile _zoneData;
        public long lastPartStartByte;
        public byte[] Checksum
        {
            get { return _checksum; }
            set { _checksum = value; OnPropertyChanged("Checksum"); }
        }

        public NdfBinary NdfBinary
        {
            get { return _ndfBinary; }
            set { _ndfBinary = value; OnPropertyChanged("Checksum"); }
        }

        public int Version
        {
            get { return _version; }
            set { _version = value; OnPropertyChanged("Version"); }
        }

        public List<byte[]> ContentFiles
        {
            get { return _contentFiles; }
            set { _contentFiles = value; }
        }

        public AreaFile ZoneData
        {
            get { return _zoneData; }
            set { _zoneData = value; OnPropertyChanged("ZoneData"); }
        }
    }
}
