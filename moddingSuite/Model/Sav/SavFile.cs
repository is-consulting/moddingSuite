using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.Model.Sav
{
    public class SavFile
    {
        private byte[] _checksum = new byte[16];

        public byte[] Checksum
        {
            get { return _checksum; }
            set { _checksum = value; }
        }

        public uint FileSize { get; set; }

        public uint ContentSize1 { get; set; }
        public uint ContentSize2 { get; set; }
        
        public ulong SaveDate {get; set; }
    }
}
