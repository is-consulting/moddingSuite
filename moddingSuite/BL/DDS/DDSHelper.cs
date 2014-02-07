using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.BL.DDS
{
    class DDSHelper
    {
        [Flags]
        public enum ConversionFlags
        {
            None = 0x0,
            Expand = 0x1, // Conversion requires expanded pixel size
            NoAlpha = 0x2, // Conversion requires setting alpha to known value
            Swizzle = 0x4, // BGR/RGB order swizzling required
            Pal8 = 0x8, // Has an 8-bit palette
            Format888 = 0x10, // Source is an 8:8:8 (24bpp) format
            Format565 = 0x20, // Source is a 5:6:5 (16bpp) format
            Format5551 = 0x40, // Source is a 5:5:5:1 (16bpp) format
            Format4444 = 0x80, // Source is a 4:4:4:4 (16bpp) format
            Format44 = 0x100, // Source is a 4:4 (8bpp) format
            Format332 = 0x200, // Source is a 3:3:2 (8bpp) format
            Format8332 = 0x400, // Source is a 8:3:3:2 (16bpp) format
            FormatA8P8 = 0x800, // Has an 8-bit palette with an alpha channel
            CopyMemory = 0x1000, // The content of the memory passed to the DDS Loader is copied to another internal buffer.
            DX10 = 0x10000, // Has the 'DX10' extension header
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LegacyMap
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="LegacyMap" /> struct.
            /// </summary>
            /// <param name="format">The format.</param>
            /// <param name="conversionFlags">The conversion flags.</param>
            /// <param name="pixelFormat">The pixel format.</param>
            public LegacyMap(PixelFormats format, ConversionFlags conversionFlags, DDS.PixelFormat pixelFormat)
            {
                Format = format;
                ConversionFlags = conversionFlags;
                PixelFormat = pixelFormat;
            }

            public PixelFormats Format;
            public ConversionFlags ConversionFlags;
            public DDS.PixelFormat PixelFormat;
        };

        private static readonly LegacyMap[] LegacyMaps = new[]
                                                             {
                                                                 new LegacyMap(PixelFormats.BC1_UNORM_SRGB, ConversionFlags.None, DDS.PixelFormat.DXT1), // D3DFMT_DXT1
                                                                 new LegacyMap(PixelFormats.BC2_UNORM_SRGB, ConversionFlags.None, DDS.PixelFormat.DXT3), // D3DFMT_DXT3
                                                                 new LegacyMap(PixelFormats.BC3_UNORM_SRGB, ConversionFlags.None, DDS.PixelFormat.DXT5), // D3DFMT_DXT5

                                                                 new LegacyMap(PixelFormats.BC2_UNORM_SRGB, ConversionFlags.None, DDS.PixelFormat.DXT2), // D3DFMT_DXT2 (ignore premultiply)
                                                                 new LegacyMap(PixelFormats.BC3_UNORM_SRGB, ConversionFlags.None, DDS.PixelFormat.DXT4), // D3DFMT_DXT4 (ignore premultiply)

                                                                 new LegacyMap(PixelFormats.BC4_UNORM, ConversionFlags.None, DDS.PixelFormat.BC4_UNorm),
                                                                 new LegacyMap(PixelFormats.BC4_SNORM, ConversionFlags.None, DDS.PixelFormat.BC4_SNorm),
                                                                 new LegacyMap(PixelFormats.BC5_UNORM, ConversionFlags.None, DDS.PixelFormat.BC5_UNorm),
                                                                 new LegacyMap(PixelFormats.BC5_SNORM, ConversionFlags.None, DDS.PixelFormat.BC5_SNorm),

                                                                 new LegacyMap(PixelFormats.BC4_UNORM, ConversionFlags.None, new DDS.PixelFormat(DDS.PixelFormatFlags.FourCC, new FourCC('A', 'T', 'I', '1'), 0, 0, 0, 0, 0)),
                                                                 new LegacyMap(PixelFormats.BC5_UNORM, ConversionFlags.None, new DDS.PixelFormat(DDS.PixelFormatFlags.FourCC, new FourCC('A', 'T', 'I', '2'), 0, 0, 0, 0, 0)),

                                                                 new LegacyMap(PixelFormats.R8G8_B8G8_UNORM, ConversionFlags.None, DDS.PixelFormat.R8G8_B8G8), // D3DFMT_R8G8_B8G8
                                                                 new LegacyMap(PixelFormats.G8R8_G8B8_UNORM, ConversionFlags.None, DDS.PixelFormat.G8R8_G8B8), // D3DFMT_G8R8_G8B8

                                                                 new LegacyMap(PixelFormats.B8G8R8A8_UNORM, ConversionFlags.None, DDS.PixelFormat.A8R8G8B8), // D3DFMT_A8R8G8B8 (uses DXGI 1.1 format)
                                                                 new LegacyMap(PixelFormats.B8G8R8X8_UNORM, ConversionFlags.None, DDS.PixelFormat.X8R8G8B8), // D3DFMT_X8R8G8B8 (uses DXGI 1.1 format)
                                                                 new LegacyMap(PixelFormats.R8G8B8A8_UNORM, ConversionFlags.None, DDS.PixelFormat.A8B8G8R8), // D3DFMT_A8B8G8R8
                                                                 new LegacyMap(PixelFormats.R8G8B8A8_UNORM, ConversionFlags.NoAlpha, DDS.PixelFormat.X8B8G8R8), // D3DFMT_X8B8G8R8
                                                                 new LegacyMap(PixelFormats.R16G16_UNORM, ConversionFlags.None, DDS.PixelFormat.G16R16), // D3DFMT_G16R16

                                                                 new LegacyMap(PixelFormats.R10G10B10A2_UNORM, ConversionFlags.Swizzle, new DDS.PixelFormat(DDS.PixelFormatFlags.Rgb, 0, 32, 0x000003ff, 0x000ffc00, 0x3ff00000, 0xc0000000)),
                                                                 // D3DFMT_A2R10G10B10 (D3DX reversal issue workaround)
                                                                 new LegacyMap(PixelFormats.R10G10B10A2_UNORM, ConversionFlags.None, new DDS.PixelFormat(DDS.PixelFormatFlags.Rgb, 0, 32, 0x3ff00000, 0x000ffc00, 0x000003ff, 0xc0000000)),
                                                                 // D3DFMT_A2B10G10R10 (D3DX reversal issue workaround)

                                                                 new LegacyMap(PixelFormats.R8G8B8A8_UNORM, ConversionFlags.Expand
                                                                                                           | ConversionFlags.NoAlpha
                                                                                                           | ConversionFlags.Format888, DDS.PixelFormat.R8G8B8), // D3DFMT_R8G8B8

                                                                 new LegacyMap(PixelFormats.B5G6R5_UNORM, ConversionFlags.Format565, DDS.PixelFormat.R5G6B5), // D3DFMT_R5G6B5
                                                                 new LegacyMap(PixelFormats.B5G5R5A1_UNORM, ConversionFlags.Format5551, DDS.PixelFormat.A1R5G5B5), // D3DFMT_A1R5G5B5
                                                                 new LegacyMap(PixelFormats.B5G5R5A1_UNORM, ConversionFlags.Format5551
                                                                                                           | ConversionFlags.NoAlpha, new DDS.PixelFormat(DDS.PixelFormatFlags.Rgb, 0, 16, 0x7c00, 0x03e0, 0x001f, 0x0000)), // D3DFMT_X1R5G5B5
     
                                                                 new LegacyMap(PixelFormats.R8G8B8A8_UNORM, ConversionFlags.Expand
                                                                                                           | ConversionFlags.Format8332, new DDS.PixelFormat(DDS.PixelFormatFlags.Rgb, 0, 16, 0x00e0, 0x001c, 0x0003, 0xff00)),
                                                                 // D3DFMT_A8R3G3B2
                                                                 new LegacyMap(PixelFormats.B5G6R5_UNORM, ConversionFlags.Expand
                                                                                                         | ConversionFlags.Format332, new DDS.PixelFormat(DDS.PixelFormatFlags.Rgb, 0, 8, 0xe0, 0x1c, 0x03, 0x00)), // D3DFMT_R3G3B2
  
                                                                 new LegacyMap(PixelFormats.R8_UNORM, ConversionFlags.None, DDS.PixelFormat.L8), // D3DFMT_L8
                                                                 new LegacyMap(PixelFormats.R16_UNORM, ConversionFlags.None, DDS.PixelFormat.L16), // D3DFMT_L16
                                                                 new LegacyMap(PixelFormats.R8G8_UNORM, ConversionFlags.None, DDS.PixelFormat.A8L8), // D3DFMT_A8L8

                                                                 new LegacyMap(PixelFormats.A8_UNORM, ConversionFlags.None, DDS.PixelFormat.A8), // D3DFMT_A8

                                                                 new LegacyMap(PixelFormats.R16G16B16A16_UNORM, ConversionFlags.None, new DDS.PixelFormat(DDS.PixelFormatFlags.FourCC, 36, 0, 0, 0, 0, 0)), // D3DFMT_A16B16G16R16
                                                                 new LegacyMap(PixelFormats.R16G16B16A16_SNORM, ConversionFlags.None, new DDS.PixelFormat(DDS.PixelFormatFlags.FourCC, 110, 0, 0, 0, 0, 0)), // D3DFMT_Q16W16V16U16
                                                                 new LegacyMap(PixelFormats.R16_FLOAT, ConversionFlags.None, new DDS.PixelFormat(DDS.PixelFormatFlags.FourCC, 111, 0, 0, 0, 0, 0)), // D3DFMT_R16F
                                                                 new LegacyMap(PixelFormats.R16G16_FLOAT, ConversionFlags.None, new DDS.PixelFormat(DDS.PixelFormatFlags.FourCC, 112, 0, 0, 0, 0, 0)), // D3DFMT_G16R16F
                                                                 new LegacyMap(PixelFormats.R16G16B16A16_FLOAT, ConversionFlags.None, new DDS.PixelFormat(DDS.PixelFormatFlags.FourCC, 113, 0, 0, 0, 0, 0)), // D3DFMT_A16B16G16R16F
                                                                 new LegacyMap(PixelFormats.R32_FLOAT, ConversionFlags.None, new DDS.PixelFormat(DDS.PixelFormatFlags.FourCC, 114, 0, 0, 0, 0, 0)), // D3DFMT_R32F
                                                                 new LegacyMap(PixelFormats.R32G32_FLOAT, ConversionFlags.None, new DDS.PixelFormat(DDS.PixelFormatFlags.FourCC, 115, 0, 0, 0, 0, 0)), // D3DFMT_G32R32F
                                                                 new LegacyMap(PixelFormats.R32G32B32A32_FLOAT, ConversionFlags.None, new DDS.PixelFormat(DDS.PixelFormatFlags.FourCC, 116, 0, 0, 0, 0, 0)), // D3DFMT_A32B32G32R32F

                                                                 new LegacyMap(PixelFormats.R32_FLOAT, ConversionFlags.None, new DDS.PixelFormat(DDS.PixelFormatFlags.Rgb, 0, 32, 0xffffffff, 0x00000000, 0x00000000, 0x00000000)),
                                                                 // D3DFMT_R32F (D3DX uses FourCC 114 instead)

                                                                 new LegacyMap(PixelFormats.R8G8B8A8_UNORM, ConversionFlags.Expand
                                                                                                           | ConversionFlags.Pal8
                                                                                                           | ConversionFlags.FormatA8P8, new DDS.PixelFormat(DDS.PixelFormatFlags.Pal8, 0, 16, 0, 0, 0, 0)), // D3DFMT_A8P8
                                                                 new LegacyMap(PixelFormats.R8G8B8A8_UNORM, ConversionFlags.Expand
                                                                                                           | ConversionFlags.Pal8, new DDS.PixelFormat(DDS.PixelFormatFlags.Pal8, 0, 8, 0, 0, 0, 0)), // D3DFMT_P8
#if DIRECTX11_1
    new LegacyMap( PixelFormats.B4G4R4A4_UNorm,     ConversionFlags.Format4444,        DDS.PixelFormat.A4R4G4B4 ), // D3DFMT_A4R4G4B4 (uses DXGI 1.2 format)
    new LegacyMap( PixelFormats.B4G4R4A4_UNorm,     ConversionFlags.NoAlpha
                                      | ConversionFlags.Format4444,      new DDS.PixelFormat(DDS.PixelFormatFlags.Rgb,       0, 16, 0x0f00,     0x00f0,     0x000f,     0x0000     ) ), // D3DFMT_X4R4G4B4 (uses DXGI 1.2 format)
    new LegacyMap( PixelFormats.B4G4R4A4_UNorm,     ConversionFlags.Expand
                                      | ConversionFlags.Format44,        new DDS.PixelFormat(DDS.PixelFormatFlags.Luminance, 0,  8, 0x0f,       0x00,       0x00,       0xf0       ) ), // D3DFMT_A4L4 (uses DXGI 1.2 format)
#else
                                                                 // !DXGI_1_2_FORMATS
                                                                 new LegacyMap(PixelFormats.R8G8B8A8_UNORM, ConversionFlags.Expand
                                                                                                           | ConversionFlags.Format4444, DDS.PixelFormat.A4R4G4B4), // D3DFMT_A4R4G4B4
                                                                 new LegacyMap(PixelFormats.R8G8B8A8_UNORM, ConversionFlags.Expand
                                                                                                           | ConversionFlags.NoAlpha
                                                                                                           | ConversionFlags.Format4444, new DDS.PixelFormat(DDS.PixelFormatFlags.Rgb, 0, 16, 0x0f00, 0x00f0, 0x000f, 0x0000)),
                                                                 // D3DFMT_X4R4G4B4
                                                                 new LegacyMap(PixelFormats.R8G8B8A8_UNORM, ConversionFlags.Expand
                                                                                                           | ConversionFlags.Format44, new DDS.PixelFormat(DDS.PixelFormatFlags.Luminance, 0, 8, 0x0f, 0x00, 0x00, 0xf0)), // D3DFMT_A4L4
#endif
                                                             };


        // Note that many common DDS reader/writers (including D3DX) swap the
        // the RED/BLUE masks for 10:10:10:2 formats. We assume
        // below that the 'backwards' header mask is being used since it is most
        // likely written by D3DX. The more robust solution is to use the 'DX10'
        // header extension and specify the Format.R10G10B10A2_UNorm format directly

        // We do not support the following legacy Direct3D 9 formats:
        //      BumpDuDv D3DFMT_V8U8, D3DFMT_Q8W8V8U8, D3DFMT_V16U16, D3DFMT_A2W10V10U10
        //      BumpLuminance D3DFMT_L6V5U5, D3DFMT_X8L8V8U8
        //      FourCC "UYVY" D3DFMT_UYVY
        //      FourCC "YUY2" D3DFMT_YUY2
        //      FourCC 117 D3DFMT_CxV8U8
        //      ZBuffer D3DFMT_D16_LOCKABLE
        //      FourCC 82 D3DFMT_D32F_LOCKABLE
        public static PixelFormats GetDXGIFormat(ref DDS.PixelFormat pixelFormat, out ConversionFlags conversionFlags, DDSFlags flags = DDSFlags.None)
        {
            conversionFlags = ConversionFlags.None;

            int index = 0;
            for (index = 0; index < LegacyMaps.Length; ++index)
            {
                var entry = LegacyMaps[index];

                if ((pixelFormat.Flags & entry.PixelFormat.Flags) != 0)
                {
                    if ((entry.PixelFormat.Flags & DDS.PixelFormatFlags.FourCC) != 0)
                    {
                        if (pixelFormat.FourCC == entry.PixelFormat.FourCC)
                            break;
                    }
                    else if ((entry.PixelFormat.Flags & DDS.PixelFormatFlags.Pal8) != 0)
                    {
                        if (pixelFormat.RGBBitCount == entry.PixelFormat.RGBBitCount)
                            break;
                    }
                    else if (pixelFormat.RGBBitCount == entry.PixelFormat.RGBBitCount)
                    {
                        // RGB, RGBA, ALPHA, LUMINANCE
                        if (pixelFormat.RBitMask == entry.PixelFormat.RBitMask
                            && pixelFormat.GBitMask == entry.PixelFormat.GBitMask
                            && pixelFormat.BBitMask == entry.PixelFormat.BBitMask
                            && pixelFormat.ABitMask == entry.PixelFormat.ABitMask)
                            break;
                    }
                }
            }

            if (index >= LegacyMaps.Length)
                return PixelFormats.UNKNOWN;

            conversionFlags = LegacyMaps[index].ConversionFlags;
            var format = LegacyMaps[index].Format;

            if ((conversionFlags & ConversionFlags.Expand) != 0 && (flags & DDSFlags.NoLegacyExpansion) != 0)
                return PixelFormats.UNKNOWN;

            if ((format == PixelFormats.R10G10B10A2_UNORM) && (flags & DDSFlags.NoR10B10G10A2Fixup) != 0)
            {
                conversionFlags ^= ConversionFlags.Swizzle;
            }

            return format;
        }

        /// <summary>
        /// Flags used by <see cref="DDSHelper.LoadFromDDSMemory"/>.
        /// </summary>
        [Flags]
        internal enum DDSFlags
        {
            None = 0x0,
            LegacyDword = 0x1, // Assume pitch is DWORD aligned instead of BYTE aligned (used by some legacy DDS files)
            NoLegacyExpansion = 0x2, // Do not implicitly convert legacy formats that result in larger pixel sizes (24 bpp, 3:3:2, A8L8, A4L4, P8, A8P8) 
            NoR10B10G10A2Fixup = 0x4, // Do not use work-around for long-standing D3DX DDS file format issue which reversed the 10:10:10:2 color order masks
            ForceRgb = 0x8, // Convert DXGI 1.1 BGR formats to Format.R8G8B8A8_UNorm to avoid use of optional WDDM 1.1 formats
            No16Bpp = 0x10, // Conversions avoid use of 565, 5551, and 4444 formats and instead expand to 8888 to avoid use of optional WDDM 1.2 formats
            CopyMemory = 0x20, // The content of the memory passed to the DDS Loader is copied to another internal buffer.
            ForceDX10Ext = 0x10000, // Always use the 'DX10' header extension for DDS writer (i.e. don't try to write DX9 compatible DDS files)
        };
    }
}
