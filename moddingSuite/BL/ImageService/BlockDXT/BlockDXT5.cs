using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.BL.ImageService.BlockDXT
{
    public struct BlockDXT5
    {
        AlphaBlockDXT5 alpha;
        BlockDXT1 color;

        public void decodeBlock(ref ColorBlock block)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            // Decode color.
            color.decodeBlock(block);

            // Decode alpha.
            alpha.decodeBlock(block);
        }

        /// Flip DXT5 block vertically.
        void flip4()
        {
            alpha.flip4();
            color.flip4();
        }

        /// Flip half DXT5 block vertically.
        void flip2()
        {
            alpha.flip2();
            color.flip2();
        }
    }
}
