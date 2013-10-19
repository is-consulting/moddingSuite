using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.BL.DDS
{
    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/windows/desktop/bb943982(v=vs.85).aspx
    /// </summary>
    [Flags]
    public enum DdsCaps
    {
        /// <summary>
        /// Optional; must be used on any file that contains more than one surface (a mipmap, a cubic environment map, or mipmapped volume texture).
        /// </summary>
        DDSCAPS_COMPLEX = 0x8,

        /// <summary>
        /// Optional; should be used for a mipmap.
        /// </summary>
        DDSCAPS_MIPMAP = 0x400000,

        /// <summary>
        /// Required
        /// </summary>
        DDSCAPS_TEXTURE = 0x1000
    }
}
