using moddingSuite.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.BL.ImageService
{
    public class ColorBlock
    {
        private Color32[] _data;

        public Color32[] Data
        {
            get { return _data; }
            //set { _data = value; }
        }

        static uint colorLuminance(Color32 c)
        {
            return (uint)(c.r + c.g + c.b);
        }

        // Get the euclidean distance between the given colors.
        static uint colorDistance(Color32 c0, Color32 c1)
        {
            return (uint)((c0.r - c1.r) * (c0.r - c1.r) + (c0.g - c1.g) * (c0.g - c1.g) + (c0.b - c1.b) * (c0.b - c1.b));
        }

        public Color32 Color(uint i) 
        {
            return Data[(int)i];
        }

        public Color32 Color(uint x, uint y)
        {
            if (!(x < 4 && y < 4))
                throw new InvalidOperationException("Colorblock is only 4 * 4");

            return Data[y * 4 + x];
        }

        /// Default constructor.
        public ColorBlock()
        {
            _data = new Color32[4 * 4];
        }

        /// Init the color block from an array of colors.
        public ColorBlock(uint[] linearImage)
            : this()
        {
            for (uint i = 0; i < 16; i++)
            {
                Data[(int)i] = new Color32(linearImage[i]);
            }
        }

        /// Init the color block with the contents of the given block.
        public ColorBlock(ColorBlock block)
            : this()
        {
            for (uint i = 0; i < 16; i++)
            {
                Data[(int)i] = block.Color(i);
            }
        }

        /// Initialize this color block.
        public ColorBlock(RawImage img, uint x, uint y)
            : this()
        {
            init(img, x, y);
        }

        void init(RawImage img, uint x, uint y)
        {
            if (img == null)
                throw new ArgumentNullException("img");

            uint bw = Math.Min(img.Width - x, 4U);
            uint bh = Math.Min(img.Height - y, 4U);

            if (bw == 0 || bh == 0)
                throw new InvalidOperationException("bw and bh have to be > 0)");

            uint[] remainder = {
		            0, 0, 0, 0,
		            0, 1, 0, 1,
		            0, 1, 2, 0,
		            0, 1, 2, 3 };

            // Blocks that are smaller than 4x4 are handled by repeating the pixels.
            // @@ Thats only correct when block size is 1, 2 or 4, but not with 3. :(

            for (uint i = 0; i < 4; i++)
            {
                //const int by = i % bh;
                uint by = remainder[(bh - 1) * 4 + i];
                for (uint e = 0; e < 4; e++)
                {
                    //const int bx = e % bw;
                    uint bx = remainder[(bw - 1) * 4 + e];
                    Data[i * 4 + e] = img.Pixel(x + bx, y + by);
                }
            }
        }

        void swizzleDXT5n()
        {
            for (int i = 0; i < 16; i++)
            {
                Color32 c = Data[i];
                Data[i] = new Color32(0xFF, c.g, 0, c.r);
            }
        }

        void splatX()
        {
            for (int i = 0; i < 16; i++)
            {
                byte x = Data[i].r;
                Data[i] = new Color32(x, x, x, x);
            }
        }

        void splatY()
        {
            for (int i = 0; i < 16; i++)
            {
                byte y = Data[i].g;
                Data[i] = new Color32(y, y, y, y);
            }
        }

        /// Returns true if the block has a single color.
        bool isSingleColor()
        {
            Color32 mask = new Color32(0xFF, 0xFF, 0xFF, 0x00);
            uint u = Data[0].u & mask.u;

            for (int i = 1; i < 16; i++)
            {
                if (u != (Data[i].u & mask.u))
                {
                    return false;
                }
            }

            return true;
        }

        /// Count number of unique colors in this color block.
        uint countUniqueColors()
        {
            uint count = 0;

            // @@ This does not have to be o(n^2)
            for (int i = 0; i < 16; i++)
            {
                bool unique = true;
                for (int j = 0; j < i; j++)
                {
                    if (Data[i] != Data[j])
                    {
                        unique = false;
                    }
                }

                if (unique)
                {
                    count++;
                }
            }

            return count;
        }

        /// Get average color of the block.
        Color32 averageColor()
        {
            uint r, g, b, a;
            r = g = b = a = 0;

            for (uint i = 0; i < 16; i++)
            {
                r += Data[i].r;
                g += Data[i].g;
                b += Data[i].b;
                a += Data[i].a;
            }

            return new Color32((byte)(r / 16), (byte)(g / 16), (byte)(b / 16), (byte)(a / 16));
        }

        /// Return true if the block is not fully opaque.
        bool hasAlpha()
        {
            for (uint i = 0; i < 16; i++)
            {
                if (Data[i].a != 255) return true;
            }
            return false;
        }

        /// Get diameter color range.
        void diameterRange(ref Color32 start, ref Color32 end)
        {
            //if (start == null)
            //    throw new ArgumentNullException("start");
            //if (end == null)
            //    throw new ArgumentNullException("end");

            Color32 c0 = new Color32();
            Color32 c1 = new Color32();
            uint best_dist = 0;

            for (int i = 0; i < 16; i++)
            {
                for (int j = i + 1; j < 16; j++)
                {
                    uint dist = colorDistance(Data[i], Data[j]);
                    if (dist > best_dist)
                    {
                        best_dist = dist;
                        c0 = Data[i];
                        c1 = Data[j];
                    }
                }
            }

            start = c0;
            end = c1;
        }

        /// Get luminance color range.
        void luminanceRange(Color32 start, Color32 end)
        {
            if (start == null)
                throw new ArgumentNullException("start");
            if (end == null)
                throw new ArgumentNullException("end");

            Color32 minColor = new Color32();
            Color32 maxColor = new Color32();
            uint minLuminance, maxLuminance;

            maxLuminance = minLuminance = colorLuminance(Data[0]);

            for (uint i = 1; i < 16; i++)
            {
                uint luminance = colorLuminance(Data[i]);

                if (luminance > maxLuminance)
                {
                    maxLuminance = luminance;
                    maxColor = Data[i];
                }
                else if (luminance < minLuminance)
                {
                    minLuminance = luminance;
                    minColor = Data[i];
                }
            }

            start = minColor;
            end = maxColor;
        }

        /// Get color range based on the bounding box. 
        void boundsRange(Color32 start, Color32 end)
        {
            if (start == null)
                throw new ArgumentNullException("start");
            if (end == null)
                throw new ArgumentNullException("end");

            var minColor = new Color32(255, 255, 255);
            var maxColor = new Color32(0, 0, 0);

            for (uint i = 0; i < 16; i++)
            {
                if (Data[i].r < minColor.r) { minColor.r = Data[i].r; }
                if (Data[i].g < minColor.g) { minColor.g = Data[i].g; }
                if (Data[i].b < minColor.b) { minColor.b = Data[i].b; }
                if (Data[i].r > maxColor.r) { maxColor.r = Data[i].r; }
                if (Data[i].g > maxColor.g) { maxColor.g = Data[i].g; }
                if (Data[i].b > maxColor.b) { maxColor.b = Data[i].b; }
            }

            // Offset range by 1/16 of the extents
            Color32 inset = new Color32();
            inset.r = (byte)((maxColor.r - minColor.r) >> 4);
            inset.g = (byte)((maxColor.g - minColor.g) >> 4);
            inset.b = (byte)((maxColor.b - minColor.b) >> 4);

            minColor.r = (byte)((minColor.r + inset.r <= 255) ? minColor.r + inset.r : 255);
            minColor.g = (byte)((minColor.g + inset.g <= 255) ? minColor.g + inset.g : 255);
            minColor.b = (byte)((minColor.b + inset.b <= 255) ? minColor.b + inset.b : 255);

            maxColor.r = (byte)((maxColor.r >= inset.r) ? maxColor.r - inset.r : 0);
            maxColor.g = (byte)((maxColor.g >= inset.g) ? maxColor.g - inset.g : 0);
            maxColor.b = (byte)((maxColor.b >= inset.b) ? maxColor.b - inset.b : 0);

            start = minColor;
            end = maxColor;
        }

        /// Get color range based on the bounding box. 
        void boundsRangeAlpha(Color32 start, Color32 end)
        {
            if (start == null)
                throw new ArgumentNullException("start");
            if (end == null)
                throw new ArgumentNullException("end");

            Color32 minColor = new Color32(255, 255, 255, 255);
            Color32 maxColor = new Color32(0, 0, 0, 0);

            for (uint i = 0; i < 16; i++)
            {
                if (Data[i].r < minColor.r) { minColor.r = Data[i].r; }
                if (Data[i].g < minColor.g) { minColor.g = Data[i].g; }
                if (Data[i].b < minColor.b) { minColor.b = Data[i].b; }
                if (Data[i].a < minColor.a) { minColor.a = Data[i].a; }
                if (Data[i].r > maxColor.r) { maxColor.r = Data[i].r; }
                if (Data[i].g > maxColor.g) { maxColor.g = Data[i].g; }
                if (Data[i].b > maxColor.b) { maxColor.b = Data[i].b; }
                if (Data[i].a > maxColor.a) { maxColor.a = Data[i].a; }
            }

            // Offset range by 1/16 of the extents
            Color32 inset = new Color32();
            inset.r = (byte)((maxColor.r - minColor.r) >> 4);
            inset.g = (byte)((maxColor.g - minColor.g) >> 4);
            inset.b = (byte)((maxColor.b - minColor.b) >> 4);
            inset.a = (byte)((maxColor.a - minColor.a) >> 4);

            minColor.r = (byte)((minColor.r + inset.r <= 255) ? minColor.r + inset.r : 255);
            minColor.g = (byte)((minColor.g + inset.g <= 255) ? minColor.g + inset.g : 255);
            minColor.b = (byte)((minColor.b + inset.b <= 255) ? minColor.b + inset.b : 255);
            minColor.a = (byte)((minColor.a + inset.a <= 255) ? minColor.a + inset.a : 255);

            maxColor.r = (byte)((maxColor.r >= inset.r) ? maxColor.r - inset.r : 0);
            maxColor.g = (byte)((maxColor.g >= inset.g) ? maxColor.g - inset.g : 0);
            maxColor.b = (byte)((maxColor.b >= inset.b) ? maxColor.b - inset.b : 0);
            maxColor.a = (byte)((maxColor.a >= inset.a) ? maxColor.a - inset.a : 0);

            start = minColor;
            end = maxColor;
        }


        //        /// Sort colors by abosolute value in their 16 bit representation.
        //        void sortColorsByAbsoluteValue()
        //        {
        //            // Dummy selection sort.
        //            for (uint a = 0; a < 16; a++)
        //            {
        //                uint max = a;
        //                Color16 cmax = new Color16(Data[a]);

        //                for (uint b = a + 1; b < 16; b++)
        //                {
        //                    Color16 cb = new Color16(Data[b]);

        //                    if (cb.u > cmax.u)
        //                    {
        //                        max = b;
        //                        cmax = cb;
        //                    }
        //                }
        //                Utils.Swap(Data[a], Data[max]);
        //            }
        //        }


        //        /// Find extreme colors in the given axis.
        //        void computeRange(Vector3::Arg axis, Color32 start, Color32 end) 
        //{
        //            if (start == null)
        //                throw new ArgumentNullException("start");
        //            if (end == null)
        //                throw new ArgumentNullException("end");

        //    int mini, maxi;
        //    mini = maxi = 0;

        //    float min, max;	
        //    min = max = dot(Vector3(Data[0].r, Data[0].g, Data[0].b), axis);

        //    for(uint i = 1; i < 16; i++)
        //    {
        //        const Vector3 vec(Data[i].r, Data[i].g, Data[i].b);

        //        float val = dot(vec, axis);
        //        if( val < min ) {
        //            mini = i;
        //            min = val;
        //        }
        //        else if( val > max ) {
        //            maxi = i;
        //            max = val;
        //        }
        //    }

        //    *start = Data[mini];
        //    *end = Data[maxi];
        //}


        //        /// Sort colors in the given axis.
        //        void sortColors(Vector3 axis)
        //{
        //    float[] luma_array = new float[16];

        //    for(uint i = 0; i < 16; i++) {
        //        const Vector3 vec(Data[i].r, Data[i].g, Data[i].b);
        //        luma_array[i] = dot(vec, axis);
        //    }

        //    // Dummy selection sort.
        //    for( uint a = 0; a < 16; a++ ) {
        //        uint min = a;
        //        for( uint b = a+1; b < 16; b++ ) {
        //            if( luma_array[b] < luma_array[min] ) {
        //                min = b;
        //            }
        //        }
        //         Utils.Swap( luma_array[a], luma_array[min] );
        //         Utils.Swap( Data[a], Data[min] );
        //    }
        //}


        //        /// Get the volume of the color block.
        //        float volume() 
        //{
        //    Box bounds;
        //    bounds.clearBounds();

        //    for(int i = 0; i < 16; i++) {
        //        const Vector3 point(Data[i].r, Data[i].g, Data[i].b);
        //        bounds.addPointToBounds(point);
        //    }

        //    return bounds.volume();
        //}
    }
}
