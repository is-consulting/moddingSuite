using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.BL.DDS
{
    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/windows/desktop/bb943984(v=vs.85).aspx
    /// </summary>
    public class DdsPixelFormat
    {
        public const int Size = 32;
        public DdsPixelFormatFlags Flags { get; set; }
        public DdsPixelFormatFourCc FourCc { get; set; }
        public uint RgbBitCount { get; set; }

        public int RBitMask { get; set; }
        public int GBitMask { get; set; }
        public int BBitMask { get; set; }
        public int ABitMask { get; set; }

        public static byte[] GetFourCc(DdsPixelFormatFourCc fourCc)
        {
            if (fourCc == DdsPixelFormatFourCc.None)
                return new byte[4];

            return Encoding.ASCII.GetBytes(Enum.GetName(typeof(DdsPixelFormatFourCc), fourCc));
        }

        /// <summary>
        /// http://www.getcodesamples.com/src/16371480/B0244AD5
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static uint GetBitsPerPixel(DxGiPixelFormat format)
        {
            switch (format)
            {
                case DxGiPixelFormat.DXGI_FORMAT_R32G32B32A32_TYPELESS:
                case DxGiPixelFormat.DXGI_FORMAT_R32G32B32A32_FLOAT:
                case DxGiPixelFormat.DXGI_FORMAT_R32G32B32A32_UINT:
                case DxGiPixelFormat.DXGI_FORMAT_R32G32B32A32_SINT:
                    return 128;

                case DxGiPixelFormat.DXGI_FORMAT_R32G32B32_TYPELESS:
                case DxGiPixelFormat.DXGI_FORMAT_R32G32B32_FLOAT:
                case DxGiPixelFormat.DXGI_FORMAT_R32G32B32_UINT:
                case DxGiPixelFormat.DXGI_FORMAT_R32G32B32_SINT:
                    return 96;

                case DxGiPixelFormat.DXGI_FORMAT_R16G16B16A16_TYPELESS:
                case DxGiPixelFormat.DXGI_FORMAT_R16G16B16A16_FLOAT:
                case DxGiPixelFormat.DXGI_FORMAT_R16G16B16A16_UNORM:
                case DxGiPixelFormat.DXGI_FORMAT_R16G16B16A16_UINT:
                case DxGiPixelFormat.DXGI_FORMAT_R16G16B16A16_SNORM:
                case DxGiPixelFormat.DXGI_FORMAT_R16G16B16A16_SINT:
                case DxGiPixelFormat.DXGI_FORMAT_R32G32_TYPELESS:
                case DxGiPixelFormat.DXGI_FORMAT_R32G32_FLOAT:
                case DxGiPixelFormat.DXGI_FORMAT_R32G32_UINT:
                case DxGiPixelFormat.DXGI_FORMAT_R32G32_SINT:
                case DxGiPixelFormat.DXGI_FORMAT_R32G8X24_TYPELESS:
                case DxGiPixelFormat.DXGI_FORMAT_D32_FLOAT_S8X24_UINT:
                case DxGiPixelFormat.DXGI_FORMAT_R32_FLOAT_X8X24_TYPELESS:
                case DxGiPixelFormat.DXGI_FORMAT_X32_TYPELESS_G8X24_UINT:
                    return 64;

                case DxGiPixelFormat.DXGI_FORMAT_R10G10B10A2_TYPELESS:
                case DxGiPixelFormat.DXGI_FORMAT_R10G10B10A2_UNORM:
                case DxGiPixelFormat.DXGI_FORMAT_R10G10B10A2_UINT:
                case DxGiPixelFormat.DXGI_FORMAT_R11G11B10_FLOAT:
                case DxGiPixelFormat.DXGI_FORMAT_R8G8B8A8_TYPELESS:
                case DxGiPixelFormat.DXGI_FORMAT_R8G8B8A8_UNORM:
                case DxGiPixelFormat.DXGI_FORMAT_R8G8B8A8_UNORM_SRGB:
                case DxGiPixelFormat.DXGI_FORMAT_R8G8B8A8_UINT:
                case DxGiPixelFormat.DXGI_FORMAT_R8G8B8A8_SNORM:
                case DxGiPixelFormat.DXGI_FORMAT_R8G8B8A8_SINT:
                case DxGiPixelFormat.DXGI_FORMAT_R16G16_TYPELESS:
                case DxGiPixelFormat.DXGI_FORMAT_R16G16_FLOAT:
                case DxGiPixelFormat.DXGI_FORMAT_R16G16_UNORM:
                case DxGiPixelFormat.DXGI_FORMAT_R16G16_UINT:
                case DxGiPixelFormat.DXGI_FORMAT_R16G16_SNORM:
                case DxGiPixelFormat.DXGI_FORMAT_R16G16_SINT:
                case DxGiPixelFormat.DXGI_FORMAT_R32_TYPELESS:
                case DxGiPixelFormat.DXGI_FORMAT_D32_FLOAT:
                case DxGiPixelFormat.DXGI_FORMAT_R32_FLOAT:
                case DxGiPixelFormat.DXGI_FORMAT_R32_UINT:
                case DxGiPixelFormat.DXGI_FORMAT_R32_SINT:
                case DxGiPixelFormat.DXGI_FORMAT_R24G8_TYPELESS:
                case DxGiPixelFormat.DXGI_FORMAT_D24_UNORM_S8_UINT:
                case DxGiPixelFormat.DXGI_FORMAT_R24_UNORM_X8_TYPELESS:
                case DxGiPixelFormat.DXGI_FORMAT_X24_TYPELESS_G8_UINT:
                case DxGiPixelFormat.DXGI_FORMAT_R9G9B9E5_SHAREDEXP:
                case DxGiPixelFormat.DXGI_FORMAT_R8G8_B8G8_UNORM:
                case DxGiPixelFormat.DXGI_FORMAT_G8R8_G8B8_UNORM:
                case DxGiPixelFormat.DXGI_FORMAT_B8G8R8A8_UNORM:
                case DxGiPixelFormat.DXGI_FORMAT_B8G8R8X8_UNORM:
                case DxGiPixelFormat.DXGI_FORMAT_R10G10B10_XR_BIAS_A2_UNORM:
                case DxGiPixelFormat.DXGI_FORMAT_B8G8R8A8_TYPELESS:
                case DxGiPixelFormat.DXGI_FORMAT_B8G8R8A8_UNORM_SRGB:
                case DxGiPixelFormat.DXGI_FORMAT_B8G8R8X8_TYPELESS:
                case DxGiPixelFormat.DXGI_FORMAT_B8G8R8X8_UNORM_SRGB:
                    return 32;

                case DxGiPixelFormat.DXGI_FORMAT_R8G8_TYPELESS:
                case DxGiPixelFormat.DXGI_FORMAT_R8G8_UNORM:
                case DxGiPixelFormat.DXGI_FORMAT_R8G8_UINT:
                case DxGiPixelFormat.DXGI_FORMAT_R8G8_SNORM:
                case DxGiPixelFormat.DXGI_FORMAT_R8G8_SINT:
                case DxGiPixelFormat.DXGI_FORMAT_R16_TYPELESS:
                case DxGiPixelFormat.DXGI_FORMAT_R16_FLOAT:
                case DxGiPixelFormat.DXGI_FORMAT_D16_UNORM:
                case DxGiPixelFormat.DXGI_FORMAT_R16_UNORM:
                case DxGiPixelFormat.DXGI_FORMAT_R16_UINT:
                case DxGiPixelFormat.DXGI_FORMAT_R16_SNORM:
                case DxGiPixelFormat.DXGI_FORMAT_R16_SINT:
                case DxGiPixelFormat.DXGI_FORMAT_B5G6R5_UNORM:
                case DxGiPixelFormat.DXGI_FORMAT_B5G5R5A1_UNORM:
                case DxGiPixelFormat.DXGI_FORMAT_B4G4R4A4_UNORM:
                    return 16;

                case DxGiPixelFormat.DXGI_FORMAT_R8_TYPELESS:
                case DxGiPixelFormat.DXGI_FORMAT_R8_UNORM:
                case DxGiPixelFormat.DXGI_FORMAT_R8_UINT:
                case DxGiPixelFormat.DXGI_FORMAT_R8_SNORM:
                case DxGiPixelFormat.DXGI_FORMAT_R8_SINT:
                case DxGiPixelFormat.DXGI_FORMAT_A8_UNORM:
                    return 8;

                case DxGiPixelFormat.DXGI_FORMAT_R1_UNORM:
                    return 1;

                case DxGiPixelFormat.DXGI_FORMAT_BC1_TYPELESS:
                case DxGiPixelFormat.DXGI_FORMAT_BC1_UNORM:
                case DxGiPixelFormat.DXGI_FORMAT_BC1_UNORM_SRGB:
                case DxGiPixelFormat.DXGI_FORMAT_BC4_TYPELESS:
                case DxGiPixelFormat.DXGI_FORMAT_BC4_UNORM:
                case DxGiPixelFormat.DXGI_FORMAT_BC4_SNORM:
                    return 4;

                case DxGiPixelFormat.DXGI_FORMAT_BC2_TYPELESS:
                case DxGiPixelFormat.DXGI_FORMAT_BC2_UNORM:
                case DxGiPixelFormat.DXGI_FORMAT_BC2_UNORM_SRGB:
                case DxGiPixelFormat.DXGI_FORMAT_BC3_TYPELESS:
                case DxGiPixelFormat.DXGI_FORMAT_BC3_UNORM:
                case DxGiPixelFormat.DXGI_FORMAT_BC3_UNORM_SRGB:
                case DxGiPixelFormat.DXGI_FORMAT_BC5_TYPELESS:
                case DxGiPixelFormat.DXGI_FORMAT_BC5_UNORM:
                case DxGiPixelFormat.DXGI_FORMAT_BC5_SNORM:
                case DxGiPixelFormat.DXGI_FORMAT_BC6H_TYPELESS:
                case DxGiPixelFormat.DXGI_FORMAT_BC6H_UF16:
                case DxGiPixelFormat.DXGI_FORMAT_BC6H_SF16:
                case DxGiPixelFormat.DXGI_FORMAT_BC7_TYPELESS:
                case DxGiPixelFormat.DXGI_FORMAT_BC7_UNORM:
                case DxGiPixelFormat.DXGI_FORMAT_BC7_UNORM_SRGB:
                    return 8;

                default:
                    return 0;
            }
        }

        public static int[] GetBitMask(DxGiPixelFormat bits)
        {
            return null;
        }
    } 

    public enum DdsPixelFormatFourCc
    {
        None,
        DXT1,
        DXT2,
        DXT3,
        DXT4,
        DXT5
    }
}
