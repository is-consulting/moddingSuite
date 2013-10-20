using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using moddingSuite.Model.Textures;

namespace moddingSuite.BL.DDS
{
    /// <summary>
    /// Writes a DDS File     
    /// </summary>
    public class DdsWriter
    {
        public TgvManager Manager { get; protected set; }

        public DdsWriter(TgvManager mgr)
        {
            Manager = mgr;
        }

        public byte[] CreateDDSFile(TgvFile file)
        {
            using (var ms = new MemoryStream())
            {
                var buffer = BitConverter.GetBytes(DDS.MagicHeader);
                ms.Write(buffer, 0, buffer.Length);

                buffer = CreateDDSHeader(file);
                ms.Write(buffer, 0, buffer.Length);

                buffer = Manager.ReadMipMap((uint)file.MipMapCount - 1);
                ms.Write(buffer, 0, buffer.Length);

                return ms.ToArray();
            }
        }


        public byte[] CreateDDSHeader(TgvFile file)
        {
            var hd = new DDS.Header()
                         {
                             Size = 124,
                             Flags = DDS.HeaderFlags.Texture,
                             SurfaceFlags = DDS.SurfaceFlags.Texture,
                             Width = file.Width,
                             Height = file.Height,
                             Depth = 1,
                         };

            var ddpf = PixelFormatFromDXGIFormat(file.Format);

            int rowPitch, slicePitch;
            int newWidth, newHeight;
            ComputePitch(file.Format, (int)file.Width, (int)file.Height, out rowPitch, out slicePitch, out newWidth, out newHeight);

            if (IsCompressedFormat(file.Format))
            {
                hd.Flags |= DDS.HeaderFlags.LinearSize;
                hd.PitchOrLinearSize = slicePitch;
            }
            else
            {
                hd.Flags |= DDS.HeaderFlags.Pitch;
                hd.PitchOrLinearSize = rowPitch;
            }

            hd.PixelFormat = ddpf;

            return StructToBytes(hd);
        }

        private byte[] StructToBytes(object str)
        {
            if (!IsValueType(str))
                throw new ArgumentException("str");

            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }

        public bool IsValueType(object obj)
        {
            return obj != null && obj.GetType().IsValueType;
        }

        public DDS.PixelFormat PixelFormatFromDXGIFormat(PixelFormats format)
        {
            var ddpf = default(DDS.PixelFormat);

            switch (format)
            {
                case PixelFormats.R8G8B8A8_UNORM:
                    ddpf = DDS.PixelFormat.A8B8G8R8;
                    break;
                case PixelFormats.R16G16_UNORM:
                    ddpf = DDS.PixelFormat.G16R16;
                    break;
                case PixelFormats.R8G8_UNORM:
                    ddpf = DDS.PixelFormat.A8L8;
                    break;
                case PixelFormats.R16_UNORM:
                    ddpf = DDS.PixelFormat.L16;
                    break;
                case PixelFormats.R8_UNORM:
                    ddpf = DDS.PixelFormat.L8;
                    break;
                case PixelFormats.A8_UNORM:
                    ddpf = DDS.PixelFormat.A8;
                    break;
                case PixelFormats.R8G8_B8G8_UNORM:
                    ddpf = DDS.PixelFormat.R8G8_B8G8;
                    break;
                case PixelFormats.G8R8_G8B8_UNORM:
                    ddpf = DDS.PixelFormat.G8R8_G8B8;
                    break;
                case PixelFormats.BC1_UNORM:
                    ddpf = DDS.PixelFormat.DXT1;
                    break;
                case PixelFormats.BC2_UNORM:
                case PixelFormats.BC2_UNORM_SRGB:
                    ddpf = DDS.PixelFormat.DXT3;
                    break;
                case PixelFormats.BC3_UNORM:
                case PixelFormats.BC3_UNORM_SRGB:
                    ddpf = DDS.PixelFormat.DXT5;
                    break;
                case PixelFormats.BC4_UNORM:
                    ddpf = DDS.PixelFormat.BC4_UNorm;
                    break;
                case PixelFormats.BC4_SNORM:
                    ddpf = DDS.PixelFormat.BC4_SNorm;
                    break;
                case PixelFormats.BC5_UNORM:
                    ddpf = DDS.PixelFormat.BC5_UNorm;
                    break;
                case PixelFormats.BC5_SNORM:
                    ddpf = DDS.PixelFormat.BC5_SNorm;
                    break;
                case PixelFormats.B5G6R5_UNORM:
                    ddpf = DDS.PixelFormat.R5G6B5;
                    break;
                case PixelFormats.B5G5R5A1_UNORM:
                    ddpf = DDS.PixelFormat.A1R5G5B5;
                    break;
                case PixelFormats.B8G8R8A8_UNORM:
                    ddpf = DDS.PixelFormat.A8R8G8B8;
                    break; // DXGI 1.1
                case PixelFormats.B8G8R8X8_UNORM:
                    ddpf = DDS.PixelFormat.X8R8G8B8;
                    break; // DXGI 1.1

                // Legacy D3DX formats using D3DFMT enum value as FourCC
                case PixelFormats.R32G32B32A32_FLOAT:
                    ddpf.Size = 32;
                    ddpf.Flags = DDS.PixelFormatFlags.FourCC;
                    ddpf.FourCC = 116; // D3DFMT_A32B32G32R32F
                    break;
                case PixelFormats.R16G16B16A16_FLOAT:
                    ddpf.Size = 32;
                    ddpf.Flags = DDS.PixelFormatFlags.FourCC;
                    ddpf.FourCC = 113; // D3DFMT_A16B16G16R16F
                    break;
                case PixelFormats.R16G16B16A16_UNORM:
                    ddpf.Size = 32;
                    ddpf.Flags = DDS.PixelFormatFlags.FourCC;
                    ddpf.FourCC = 36; // D3DFMT_A16B16G16R16
                    break;
                case PixelFormats.R16G16B16A16_SNORM:
                    ddpf.Size = 32;
                    ddpf.Flags = DDS.PixelFormatFlags.FourCC;
                    ddpf.FourCC = 110; // D3DFMT_Q16W16V16U16
                    break;
                case PixelFormats.R32G32_FLOAT:
                    ddpf.Size = 32;
                    ddpf.Flags = DDS.PixelFormatFlags.FourCC;
                    ddpf.FourCC = 115; // D3DFMT_G32R32F
                    break;
                case PixelFormats.R16G16_FLOAT:
                    ddpf.Size = 32;
                    ddpf.Flags = DDS.PixelFormatFlags.FourCC;
                    ddpf.FourCC = 112; // D3DFMT_G16R16F
                    break;
                case PixelFormats.R32_FLOAT:
                    ddpf.Size = 32;
                    ddpf.Flags = DDS.PixelFormatFlags.FourCC;
                    ddpf.FourCC = 114; // D3DFMT_R32F
                    break;
                case PixelFormats.R16_FLOAT:
                    ddpf.Size = 32;
                    ddpf.Flags = DDS.PixelFormatFlags.FourCC;
                    ddpf.FourCC = 111; // D3DFMT_R16F
                    break;
            }

            return ddpf;
        }

        public bool IsCompressedFormat(PixelFormats format)
        {
            switch (format)
            {
                case PixelFormats.BC1_TYPELESS:
                case PixelFormats.BC1_UNORM:
                case PixelFormats.BC1_UNORM_SRGB:
                case PixelFormats.BC2_TYPELESS:
                case PixelFormats.BC2_UNORM:
                case PixelFormats.BC2_UNORM_SRGB:
                case PixelFormats.BC3_TYPELESS:
                case PixelFormats.BC3_UNORM:
                case PixelFormats.BC3_UNORM_SRGB:
                case PixelFormats.BC4_TYPELESS:
                case PixelFormats.BC4_UNORM:
                case PixelFormats.BC4_SNORM:
                case PixelFormats.BC5_TYPELESS:
                case PixelFormats.BC5_UNORM:
                case PixelFormats.BC5_SNORM:
                case PixelFormats.BC6H_TYPELESS:
                case PixelFormats.BC6H_UF16:
                case PixelFormats.BC6H_SF16:
                case PixelFormats.BC7_TYPELESS:
                case PixelFormats.BC7_UNORM:
                case PixelFormats.BC7_UNORM_SRGB:
                    return true;
                default:
                    return false;
            }
        }

        private void ComputePitch(PixelFormats fmt, int width, int height, out int rowPitch, out int slicePitch, out int widthCount, out int heightCount, PitchFlags flags = PitchFlags.None)
        {
            widthCount = width;
            heightCount = height;

            if (IsCompressedFormat(fmt))
            {
                int bpb = (fmt == PixelFormats.BC1_TYPELESS
                             || fmt == PixelFormats.BC1_UNORM
                             || fmt == PixelFormats.BC1_UNORM_SRGB
                             || fmt == PixelFormats.BC4_TYPELESS
                             || fmt == PixelFormats.BC4_UNORM
                             || fmt == PixelFormats.BC4_SNORM) ? 8 : 16;
                widthCount = Math.Max(1, (width + 3) / 4);
                heightCount = Math.Max(1, (height + 3) / 4);
                rowPitch = widthCount * bpb;

                slicePitch = rowPitch * heightCount;
            }
            else if (IsPacked(fmt))
            {
                rowPitch = ((width + 1) >> 1) * 4;

                slicePitch = rowPitch * height;
            }
            else
            {
                int bpp;

                if ((flags & PitchFlags.Bpp24) != 0)
                    bpp = 24;
                else if ((flags & PitchFlags.Bpp16) != 0)
                    bpp = 16;
                else if ((flags & PitchFlags.Bpp8) != 0)
                    bpp = 8;
                else
                    bpp = GetBitsPerPixel(fmt);

                if ((flags & PitchFlags.LegacyDword) != 0)
                {
                    // Special computation for some incorrectly created DDS files based on
                    // legacy DirectDraw assumptions about pitch alignment
                    rowPitch = ((width * bpp + 31) / 32) * sizeof(int);
                    slicePitch = rowPitch * height;
                }
                else
                {
                    rowPitch = (width * bpp + 7) / 8;
                    slicePitch = rowPitch * height;
                }
            }
        }

        [Flags]
        internal enum PitchFlags
        {
            None = 0x0,         // Normal operation
            LegacyDword = 0x1,  // Assume pitch is DWORD aligned instead of BYTE aligned
            Bpp24 = 0x10000,    // Override with a legacy 24 bits-per-pixel format size
            Bpp16 = 0x20000,    // Override with a legacy 16 bits-per-pixel format size
            Bpp8 = 0x40000,     // Override with a legacy 8 bits-per-pixel format size
        };

        /// <summary>
        /// http://www.getcodesamples.com/src/16371480/B0244AD5
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static int GetBitsPerPixel(PixelFormats format)
        {
            switch (format)
            {
                case PixelFormats.R32G32B32A32_TYPELESS:
                case PixelFormats.R32G32B32A32_FLOAT:
                case PixelFormats.R32G32B32A32_UINT:
                case PixelFormats.R32G32B32A32_SINT:
                    return 128;

                case PixelFormats.R32G32B32_TYPELESS:
                case PixelFormats.R32G32B32_FLOAT:
                case PixelFormats.R32G32B32_UINT:
                case PixelFormats.R32G32B32_SINT:
                    return 96;

                case PixelFormats.R16G16B16A16_TYPELESS:
                case PixelFormats.R16G16B16A16_FLOAT:
                case PixelFormats.R16G16B16A16_UNORM:
                case PixelFormats.R16G16B16A16_UINT:
                case PixelFormats.R16G16B16A16_SNORM:
                case PixelFormats.R16G16B16A16_SINT:
                case PixelFormats.R32G32_TYPELESS:
                case PixelFormats.R32G32_FLOAT:
                case PixelFormats.R32G32_UINT:
                case PixelFormats.R32G32_SINT:
                case PixelFormats.R32G8X24_TYPELESS:
                case PixelFormats.D32_FLOAT_S8X24_UINT:
                case PixelFormats.R32_FLOAT_X8X24_TYPELESS:
                case PixelFormats.X32_TYPELESS_G8X24_UINT:
                    return 64;

                case PixelFormats.R10G10B10A2_TYPELESS:
                case PixelFormats.R10G10B10A2_UNORM:
                case PixelFormats.R10G10B10A2_UINT:
                case PixelFormats.R11G11B10_FLOAT:
                case PixelFormats.R8G8B8A8_TYPELESS:
                case PixelFormats.R8G8B8A8_UNORM:
                case PixelFormats.R8G8B8A8_UNORM_SRGB:
                case PixelFormats.R8G8B8A8_UINT:
                case PixelFormats.R8G8B8A8_SNORM:
                case PixelFormats.R8G8B8A8_SINT:
                case PixelFormats.R16G16_TYPELESS:
                case PixelFormats.R16G16_FLOAT:
                case PixelFormats.R16G16_UNORM:
                case PixelFormats.R16G16_UINT:
                case PixelFormats.R16G16_SNORM:
                case PixelFormats.R16G16_SINT:
                case PixelFormats.R32_TYPELESS:
                case PixelFormats.D32_FLOAT:
                case PixelFormats.R32_FLOAT:
                case PixelFormats.R32_UINT:
                case PixelFormats.R32_SINT:
                case PixelFormats.R24G8_TYPELESS:
                case PixelFormats.D24_UNORM_S8_UINT:
                case PixelFormats.R24_UNORM_X8_TYPELESS:
                case PixelFormats.X24_TYPELESS_G8_UINT:
                case PixelFormats.R9G9B9E5_SHAREDEXP:
                case PixelFormats.R8G8_B8G8_UNORM:
                case PixelFormats.G8R8_G8B8_UNORM:
                case PixelFormats.B8G8R8A8_UNORM:
                case PixelFormats.B8G8R8X8_UNORM:
                case PixelFormats.R10G10B10_XR_BIAS_A2_UNORM:
                case PixelFormats.B8G8R8A8_TYPELESS:
                case PixelFormats.B8G8R8A8_UNORM_SRGB:
                case PixelFormats.B8G8R8X8_TYPELESS:
                case PixelFormats.B8G8R8X8_UNORM_SRGB:
                    return 32;

                case PixelFormats.R8G8_TYPELESS:
                case PixelFormats.R8G8_UNORM:
                case PixelFormats.R8G8_UINT:
                case PixelFormats.R8G8_SNORM:
                case PixelFormats.R8G8_SINT:
                case PixelFormats.R16_TYPELESS:
                case PixelFormats.R16_FLOAT:
                case PixelFormats.D16_UNORM:
                case PixelFormats.R16_UNORM:
                case PixelFormats.R16_UINT:
                case PixelFormats.R16_SNORM:
                case PixelFormats.R16_SINT:
                case PixelFormats.B5G6R5_UNORM:
                case PixelFormats.B5G5R5A1_UNORM:
                case PixelFormats.B4G4R4A4_UNORM:
                    return 16;

                case PixelFormats.R8_TYPELESS:
                case PixelFormats.R8_UNORM:
                case PixelFormats.R8_UINT:
                case PixelFormats.R8_SNORM:
                case PixelFormats.R8_SINT:
                case PixelFormats.A8_UNORM:
                    return 8;

                case PixelFormats.R1_UNORM:
                    return 1;

                case PixelFormats.BC1_TYPELESS:
                case PixelFormats.BC1_UNORM:
                case PixelFormats.BC1_UNORM_SRGB:
                case PixelFormats.BC4_TYPELESS:
                case PixelFormats.BC4_UNORM:
                case PixelFormats.BC4_SNORM:
                    return 4;

                case PixelFormats.BC2_TYPELESS:
                case PixelFormats.BC2_UNORM:
                case PixelFormats.BC2_UNORM_SRGB:
                case PixelFormats.BC3_TYPELESS:
                case PixelFormats.BC3_UNORM:
                case PixelFormats.BC3_UNORM_SRGB:
                case PixelFormats.BC5_TYPELESS:
                case PixelFormats.BC5_UNORM:
                case PixelFormats.BC5_SNORM:
                case PixelFormats.BC6H_TYPELESS:
                case PixelFormats.BC6H_UF16:
                case PixelFormats.BC6H_SF16:
                case PixelFormats.BC7_TYPELESS:
                case PixelFormats.BC7_UNORM:
                case PixelFormats.BC7_UNORM_SRGB:
                    return 8;

                default:
                    return 0;
            }
        }

        public static bool IsPacked(PixelFormats fmt)
        {
            return ((fmt == PixelFormats.R8G8_B8G8_UNORM) || (fmt == PixelFormats.G8R8_G8B8_UNORM));
        }

    }
}

