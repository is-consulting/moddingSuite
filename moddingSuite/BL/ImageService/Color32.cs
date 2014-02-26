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
    public struct Color32
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

        public Color32(Color32 c) { b = 0; g = 0; r = 0; a = 0; u = c.u; }
        public Color32(byte R, byte G, byte B) { u = 0; r = R; g = G; b = B; a = 0xFF; }
        public Color32(byte R, byte G, byte B, byte A) { u = 0; r = R; g = G; b = B; a = A; }
        public Color32(uint U) { b = 0; g = 0; r = 0; a = 0; u = U; }

        public static bool operator ==(Color32 left, Color32 right)
        {
            return left.u == right.u;
        }

        public static bool operator !=(Color32 left, Color32 right)
        {
            return left.u != right.u;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
