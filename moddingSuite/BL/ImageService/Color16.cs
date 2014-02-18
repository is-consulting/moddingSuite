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
    public class Color16
    {
        // 5
        [FieldOffset(0)]
        public ushort b;
        // 6
        [FieldOffset(0)]
        public ushort g;
        // 5
        [FieldOffset(0)]
        public short r;

        [FieldOffset(0)]
        public ushort u;

        public Color16() { }
        public Color16(Color16 c) { u = c.u; }
        public Color16(ushort U) { u = U; }
    }
}
