using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.BL.DDS
{
    /// <summary>
    ///  http://msdn.microsoft.com/en-us/library/windows/desktop/bb943982(v=vs.85).aspx       
    /// </summary>
    public class DdsHeader
    {
        public const uint DdsMagic = 0x20534444; // "DDS "
        public const uint Size = 124;

        public DdsHeader(uint heght, uint width, uint mipMapCnt, DdsPixelFormat format)
        {

        }

        public DdsFlags Flags
        {
            get { return MipMapCount > 0 ? DdsFlags.DDS_HEADER_FLAGS_MIPMAP : DdsFlags.DDS_HEADER_FLAGS_TEXTURE; }
        }

        public uint Height { get; set; }

        public uint Width { get; set; }

        public int Pitch { get; set; }

        public int Depth { get; set; }

        public uint MipMapCount { get; set; }

        public DdsPixelFormat PixelFormat { get; set; }

        public DdsCaps Caps { get; set; }

        public int Caps2 { get; set; }

        public int Caps3 { get; set; }

        public int Caps4 { get; set; }

        public int Reserved2 { get; set; }
    }
}
