using moddingSuite.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.BL.ImageService
{
    public class RawImage
    {
        private Color32[] _data;

        public enum Format
        {
            Format_RGB,
            Format_ARGB,
        }

        public uint Width
        {
            get;
            set;
        }

        public uint Height
        {
            get;
            set;
        }

        public Color32[] Data
        {
            get { return _data; }
        }

        public Format ColFormat
        {
            get;
            set;
        }

        public RawImage(Color32[] data, uint width, uint height)
        {
            if (data == null)
                _data = new Color32[width * height * 4]; // sizeof(Color32) == 4
            else
                _data = data;

            Width = width;
            Height = height;
        }

        public RawImage(uint width, uint height)
            : this(null, width, height)
        {
        }

        public Color32[] Scanline(uint line)
        {
            Color32[] tmp = new Color32[Width];
            Array.Copy(Data, line * Width, tmp, 0, Width);
            return tmp;
        }

        public Color32 Pixel(uint px)
        {
            return Data[px];
        }

        public Color32 Pixel(uint x, uint y)
        {
            return Pixel(y * Width + x);
        }

        public byte[] GetRawData()
        {
            var ret = new List<byte>();

            foreach (var col in Data)
                ret.AddRange(Utils.StructToBytes(col));

            return ret.ToArray();
        }
    }
}
