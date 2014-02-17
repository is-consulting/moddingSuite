using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.BL.ImageService
{
    // Because of .NET's Little Endian we use BGRA
    [StructLayout(LayoutKind.Explicit, Size = 4, Pack = 1)]
    public struct Color32
    {
        [FieldOffset(0)]
        public byte B;
        [FieldOffset(1)]
        public byte G;
        [FieldOffset(2)]
        public byte R;
        [FieldOffset(3)]
        public byte A;

        [FieldOffset(0)]
        public uint Color;
    }
}
