using moddingSuite.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.BL.ImageService.BlockDXT
{
    [StructLayout(LayoutKind.Explicit, Size = 8, Pack = 1)]
    public struct BlockDXT1
    {
        [FieldOffset(0)]
        public Color16 col0;
        [FieldOffset(2)]
        public Color16 col1;

        [FieldOffset(4)]
        public byte row_1;
        [FieldOffset(5)]
        public byte row_2;
        [FieldOffset(6)]
        public byte row_3;
        [FieldOffset(7)]
        public byte row_4;
        [FieldOffset(4)]
        public uint indices;

        public byte row(uint r)
        {
            if (r > 3)
                throw new ArgumentException("r");

            switch (r)
            {
                case 0:
                    return row_1;
                case 1:
                    return row_2;
                case 2:
                    return row_3;
                case 3:
                    return row_4;
            }

            throw new InvalidOperationException("r ist not 0-3");
        }

        public bool isFourColorMode()
        {
            return col0.u > col1.u;
        }

        public uint evaluatePalette(ref Color32[] color_array)
        {
            if (color_array.Length > 4)
                throw new ArgumentOutOfRangeException("color_array out of bounds");

            // Does bit expansion before interpolation.
            color_array[0].b = (byte)((col0.b << 3) | (col0.b >> 2));
            color_array[0].g = (byte)((col0.g << 2) | (col0.g >> 4));
            color_array[0].r = (byte)((col0.r << 3) | (col0.r >> 2));
            color_array[0].a = 0xFF;

            // @@ Same as above, but faster?
            //	Color32 c;
            //	c.u = ((col0.u << 3) & 0xf8) | ((col0.u << 5) & 0xfc00) | ((col0.u << 8) & 0xf80000);
            //	c.u |= (c.u >> 5) & 0x070007;
            //	c.u |= (c.u >> 6) & 0x000300;
            //	color_array[0].u = c.u;

            color_array[1].r = (byte)((col1.r << 3) | (col1.r >> 2));
            color_array[1].g = (byte)((col1.g << 2) | (col1.g >> 4));
            color_array[1].b = (byte)((col1.b << 3) | (col1.b >> 2));
            color_array[1].a = 0xFF;

            // @@ Same as above, but faster?
            //	c.u = ((col1.u << 3) & 0xf8) | ((col1.u << 5) & 0xfc00) | ((col1.u << 8) & 0xf80000);
            //	c.u |= (c.u >> 5) & 0x070007;
            //	c.u |= (c.u >> 6) & 0x000300;
            //	color_array[1].u = c.u;

            if (col0.u > col1.u)
            {
                // Four-color block: derive the other two colors.
                color_array[2].r = (byte)((2 * color_array[0].r + color_array[1].r) / 3);
                color_array[2].g = (byte)((2 * color_array[0].g + color_array[1].g) / 3);
                color_array[2].b = (byte)((2 * color_array[0].b + color_array[1].b) / 3);
                color_array[2].a = 0xFF;

                color_array[3].r = (byte)((2 * color_array[1].r + color_array[0].r) / 3);
                color_array[3].g = (byte)((2 * color_array[1].g + color_array[0].g) / 3);
                color_array[3].b = (byte)((2 * color_array[1].b + color_array[0].b) / 3);
                color_array[3].a = 0xFF;

                return 4;
            }
            else
            {
                // Three-color block: derive the other color.
                color_array[2].r = (byte)((color_array[0].r + color_array[1].r) / 2);
                color_array[2].g = (byte)((color_array[0].g + color_array[1].g) / 2);
                color_array[2].b = (byte)((color_array[0].b + color_array[1].b) / 2);
                color_array[2].a = 0xFF;

                // Set all components to 0 to match DXT specs.
                color_array[3].r = 0x00; // color_array[2].r;
                color_array[3].g = 0x00; // color_array[2].g;
                color_array[3].b = 0x00; // color_array[2].b;
                color_array[3].a = 0x00;

                return 3;
            }
        }

        // Evaluate palette assuming 3 color block.
        public void evaluatePalette3(ref Color32[] color_array)
        {
            if (color_array.Length > 4)
                throw new ArgumentOutOfRangeException("color_array out of bounds");

            color_array[0].b = (byte)((col0.b << 3) | (col0.b >> 2));
            color_array[0].g = (byte)((col0.g << 2) | (col0.g >> 4));
            color_array[0].r = (byte)((col0.r << 3) | (col0.r >> 2));
            color_array[0].a = 0xFF;

            color_array[1].r = (byte)((col1.r << 3) | (col1.r >> 2));
            color_array[1].g = (byte)((col1.g << 2) | (col1.g >> 4));
            color_array[1].b = (byte)((col1.b << 3) | (col1.b >> 2));
            color_array[1].a = 0xFF;

            // Three-color block: derive the other color.
            color_array[2].r = (byte)((color_array[0].r + color_array[1].r) / 2);
            color_array[2].g = (byte)((color_array[0].g + color_array[1].g) / 2);
            color_array[2].b = (byte)((color_array[0].b + color_array[1].b) / 2);
            color_array[2].a = 0xFF;

            // Set all components to 0 to match DXT specs.
            color_array[3].r = 0x00; // color_array[2].r;
            color_array[3].g = 0x00; // color_array[2].g;
            color_array[3].b = 0x00; // color_array[2].b;
            color_array[3].a = 0x00;
        }

        // Evaluate palette assuming 4 color block.
        public void evaluatePalette4(ref Color32[] color_array)
        {
            if (color_array.Length > 4)
                throw new ArgumentOutOfRangeException("color_array out of bounds");

            color_array[0].b = (byte)((col0.b << 3) | (col0.b >> 2));
            color_array[0].g = (byte)((col0.g << 2) | (col0.g >> 4));
            color_array[0].r = (byte)((col0.r << 3) | (col0.r >> 2));
            color_array[0].a = 0xFF;

            color_array[1].r = (byte)((col1.r << 3) | (col1.r >> 2));
            color_array[1].g = (byte)((col1.g << 2) | (col1.g >> 4));
            color_array[1].b = (byte)((col1.b << 3) | (col1.b >> 2));
            color_array[1].a = 0xFF;

            // Four-color block: derive the other two colors.
            color_array[2].r = (byte)((2 * color_array[0].r + color_array[1].r) / 3);
            color_array[2].g = (byte)((2 * color_array[0].g + color_array[1].g) / 3);
            color_array[2].b = (byte)((2 * color_array[0].b + color_array[1].b) / 3);
            color_array[2].a = 0xFF;

            color_array[3].r = (byte)((2 * color_array[1].r + color_array[0].r) / 3);
            color_array[3].g = (byte)((2 * color_array[1].g + color_array[0].g) / 3);
            color_array[3].b = (byte)((2 * color_array[1].b + color_array[0].b) / 3);
            color_array[3].a = 0xFF;
        }

        public void decodeBlock(ColorBlock block)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            // Decode color block.
            Color32[] color_array = { new Color32(), new Color32(), new Color32(), new Color32() };
            evaluatePalette(ref color_array);

            // Write color block.
            for (uint j = 0; j < 4; j++)
            {
                for (uint i = 0; i < 4; i++)
                {
                    uint idx = ((uint)row(j) >> (ushort)(2 * i)) & 3;
                    block.Data[j * 4 + i] = color_array[idx];
                }
            }
        }

        public void setIndices(int[] idx)
        {
            indices = 0;
            for (uint i = 0; i < 16; i++)
            {
                indices |= (uint) ((idx[i] & 3) << (short)(2 * i));
            }
        }

        /// Flip DXT1 block vertically.
        public void flip4()
        {
            Utils.Swap(row(0), row(3));
            Utils.Swap(row(1), row(2));
        }

        /// Flip half DXT1 block vertically.
        public void flip2()
        {
            Utils.Swap(row(0), row(1));
        }
    }
}
