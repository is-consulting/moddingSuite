using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.BL.ImageService
{
    /// <summary>
    /// .NET is little endian -> bgra
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 4, Pack = 1)]
    public class Color32
    {
        [FieldOffset(0)]
        public byte b;
        [FieldOffset(1)]
        public byte g;
        [FieldOffset(2)]
        public byte r;
        [FieldOffset(3)]
        public byte a;

        [FieldOffset(0)]
        public uint u;

        public Color32() { }
        public Color32(Color32 c) { u = c.u; }
        public Color32(byte R, byte G, byte B) { setRGBA(R, G, B, 0xFF); }
        public Color32(byte R, byte G, byte B, byte A) { setRGBA(R, G, B, A); }
        public Color32(uint U) { u = U; }

        public void setRGBA(byte R, byte G, byte B, byte A)
        {
            r = R;
            g = G;
            b = B;
            a = A;
        }

        public void setBGRA(byte B, byte G, byte R, byte A = 0xFF)
        {
            r = R;
            g = G;
            b = B;
            a = A;
        }
    }
}
