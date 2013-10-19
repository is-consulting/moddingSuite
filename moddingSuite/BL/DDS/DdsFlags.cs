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
    public enum DdsFlags
    {
        /// <summary>
        /// Required in every .dds file.
        /// </summary>
        DDSD_CAPS = 0x1,
        /// <summary>
        /// Required in every .dds file.
        /// </summary>
        DDSD_HEIGHT = 0x2,
        /// <summary>
        /// Required in every .dds file.
        /// </summary>
        DDSD_WIDTH = 0x4,
        /// <summary>
        /// Required when pitch is provided for an uncompressed texture.
        /// </summary>
        DDSD_PITCH = 0x8,
        /// <summary>
        /// Required in every .dds file.
        /// </summary>
        DDSD_PIXELFORMAT = 0x1000,
        /// <summary>
        /// Required in a mipmapped texture.
        /// </summary>
        DDSD_MIPMAPCOUNT = 0x20000,
        /// <summary>
        /// Required when pitch is provided for a compressed texture.
        /// </summary>
        DDSD_LINEARSIZE = 0x80000,
        /// <summary>
        /// Required in a depth texture.
        /// </summary>
        DDSD_DEPTH = 0x800000,


        /// <summary>
        /// All required header flags. DDSD_CAPS | DDSD_HEIGHT | DDSD_WIDTH | DDSD_PIXELFORMAT
        /// </summary>
        DDS_HEADER_FLAGS_TEXTURE  = DDSD_CAPS | DDSD_HEIGHT | DDSD_WIDTH | DDSD_PIXELFORMAT,

        /// <summary>
        /// DDS_HEADER_FLAGS_TEXTURE | DDSD_MIPMAPCOUNT
        /// </summary>
        DDS_HEADER_FLAGS_MIPMAP = DDS_HEADER_FLAGS_TEXTURE | DDSD_MIPMAPCOUNT,
       
    }
}
