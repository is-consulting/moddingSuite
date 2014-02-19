using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.BL.ImageService
{
    /// <summary>
    /// .NET is little endian -> bgr
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 2, Pack = 1)]
    public struct Color16
    {
        // Btifield: 5
        [FieldOffset(0)]
        private ushort b_i;

        public ushort b
        {
            get { return (ushort)((b_i >> 11) & 0x1F); }
            set { b_i = (ushort)((b_i & ~(0x1F << 11)) | (value & 0x3F) << 11); }
        }

        // Bitfield: 6
        [FieldOffset(0)]
        private ushort g_i;

        public ushort g
        {
            get { return (ushort)((g_i >> 5) & 0x7F); }
            set { g_i = (ushort)((g_i & ~(0x7F << 5)) | (value & 0x7F) << 5); }
        }

        // Bitfield: 5
        [FieldOffset(0)]
        private ushort r_i;

        public ushort r
        {
            get { return (ushort) (r_i & 0x1F); }
            set { r_i = (ushort) ((r_i & ~0x1F) | (value & 0x1F)); }
        }

        [FieldOffset(0)]
        public ushort u;

        //public Color16() { }
        public Color16(Color16 c) { r_i = 0; g_i = 0; b_i = 0; u = c.u; }
        public Color16(ushort U) { r_i = 0; g_i = 0; b_i = 0; u = U; }


    }
}
