using moddingSuite.BL.DDS;
using moddingSuite.BL.ImageService;
using moddingSuite.BL.ImageService.BlockDXT;
using moddingSuite.Model.Textures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.BL.TGV
{
    public class TgvBitmapReader
    {
        public RawImage GetMip(TgvFile file, uint mip)
        {
            RawImage ret;

            uint width = file.ImageWidth;
            uint height = file.ImageHeight;

            // Compute width and height.
            for (uint m = 0; m < mip; m++)
            {
                width = Math.Max(1, width / 2);
                height = Math.Max(1, height / 2);
            }

            ret = new RawImage(width, height);

            using (var ms = new MemoryStream(file.MipMaps[(int)mip].Content))
            {
                if (DDS.DDS.IsCompressedFormat(file.Format))
                {
                    ReadBlockFormat(ret);
                }
            }

            return ret;
        }

        private static void ReadBlockFormat(RawImage ret)
        {
            ret.ColFormat = RawImage.Format.Format_ARGB;

            uint w = ret.Width;
            uint h = ret.Height;

            uint bw = (w + 3) / 4;
            uint bh = (h + 3) / 4;

            for (uint by = 0; by < bh; by++)
                for (uint bx = 0; bx < bw; bx++)
                {
                    ColorBlock block = new ColorBlock();

                    // Read color block.
                    ReadBlock(block);

                    // Write color block.
                    for (uint y = 0; y < Math.Min(4, h - 4 * by); y++)
                        for (uint x = 0; x < Math.Min(4, w - 4 * bx); x++)
                            ret.Data[(4 * by + y) * ret.Width + (4 * bx + x)] = block.Color(x, y);
                }

        }

        private static void ReadBlock(ColorBlock rgba)
        {
            BlockDXT5 blockdxt5 = new BlockDXT5();

            blockdxt5.decodeBlock(rgba);
        }

    }
}
