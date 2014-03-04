using System;
using System.Runtime.InteropServices;

namespace moddingSuite.BL.DDS
{
    public class DDS
    {
        #region CubemapFlags enum

        /// <summary>
        /// DDS Cubemap flags.
        /// </summary>
        [Flags]
        public enum CubemapFlags
        {
            CubeMap = 0x00000200, // DDSCAPS2_CUBEMAP
            Volume = 0x00200000, // DDSCAPS2_VOLUME
            PositiveX = 0x00000600, // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_POSITIVEX
            NegativeX = 0x00000a00, // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_NEGATIVEX
            PositiveY = 0x00001200, // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_POSITIVEY
            NegativeY = 0x00002200, // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_NEGATIVEY
            PositiveZ = 0x00004200, // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_POSITIVEZ
            NegativeZ = 0x00008200, // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_NEGATIVEZ

            AllFaces = PositiveX | NegativeX | PositiveY | NegativeY | PositiveZ | NegativeZ,
        }

        #endregion

        #region HeaderFlags enum

        /// <summary>
        /// DDS Header flags.
        /// </summary>
        [Flags]
        public enum HeaderFlags
        {
            Texture = 0x00001007, // DDSD_CAPS | DDSD_HEIGHT | DDSD_WIDTH | DDSD_PIXELFORMAT 
            Mipmap = 0x00020000, // DDSD_MIPMAPCOUNT
            Volume = 0x00800000, // DDSD_DEPTH
            Pitch = 0x00000008, // DDSD_PITCH
            LinearSize = 0x00080000, // DDSD_LINEARSIZE
            Height = 0x00000002, // DDSD_HEIGHT
            Width = 0x00000004, // DDSD_WIDTH
        };

        #endregion

        #region PixelFormatFlags enum

        /// <summary>
        /// PixelFormat flags.
        /// </summary>
        [Flags]
        public enum PixelFormatFlags
        {
            FourCC = 0x00000004, // DDPF_FOURCC
            Rgb = 0x00000040, // DDPF_RGB
            Rgba = 0x00000041, // DDPF_RGB | DDPF_ALPHAPIXELS
            Luminance = 0x00020000, // DDPF_LUMINANCE
            LuminanceAlpha = 0x00020001, // DDPF_LUMINANCE | DDPF_ALPHAPIXELS
            Alpha = 0x00000002, // DDPF_ALPHA
            Pal8 = 0x00000020, // DDPF_PALETTEINDEXED8            
        }

        #endregion

        #region SurfaceFlags enum

        /// <summary>
        /// DDS Surface flags.
        /// </summary>
        [Flags]
        public enum SurfaceFlags
        {
            Texture = 0x00001000, // DDSCAPS_TEXTURE
            Mipmap = 0x00400008, // DDSCAPS_COMPLEX | DDSCAPS_MIPMAP
            Cubemap = 0x00000008, // DDSCAPS_COMPLEX
        }

        #endregion

        /// <summary>
        /// Magic code to identify DDS header
        /// </summary>
        public const uint MagicHeader = 0x20534444; // "DDS "

        //[StructLayout(LayoutKind.Sequential, Pack = 1)]
        //public struct HeaderDXT10
        //{
        //    public DXGI.Format DXGIFormat;
        //    public ResourceDimension ResourceDimension;
        //    public ResourceOptionFlags MiscFlags; // see DDS_RESOURCE_MISC_FLAG
        //    public int ArraySize;

        //    private readonly uint Unused;
        //}

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

        public static void ComputePitch(PixelFormats fmt, int width, int height, out int rowPitch, out int slicePitch,
                                  out int widthCount, out int heightCount, PitchFlags flags = PitchFlags.None)
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
                           || fmt == PixelFormats.BC4_SNORM)
                              ? 8
                              : 16;
                widthCount = Math.Max(1, (width + 3)/4);
                heightCount = Math.Max(1, (height + 3)/4);
                rowPitch = widthCount*bpb;

                slicePitch = rowPitch*heightCount;
            }
            else if (IsPacked(fmt))
            {
                rowPitch = ((width + 1) >> 1)*4;

                slicePitch = rowPitch*height;
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
                    rowPitch = ((width*bpp + 31)/32)*sizeof (int);
                    slicePitch = rowPitch*height;
                }
                else
                {
                    rowPitch = (width*bpp + 7)/8;
                    slicePitch = rowPitch*height;
                }
            }
        }

        public static bool IsPacked(PixelFormats fmt)
        {
            return ((fmt == PixelFormats.R8G8_B8G8_UNORM) || (fmt == PixelFormats.G8R8_G8B8_UNORM));
        }

        public static bool IsCompressedFormat(PixelFormats format)
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

        public static PixelFormat PixelFormatFromDXGIFormat(PixelFormats format)
        {
            PixelFormat ddpf = default(PixelFormat);

            switch (format)
            {
                case PixelFormats.R8G8B8A8_UNORM:
                    ddpf = PixelFormat.A8B8G8R8;
                    break;
                case PixelFormats.R16G16_UNORM:
                    ddpf = PixelFormat.G16R16;
                    break;
                case PixelFormats.R8G8_UNORM:
                    ddpf = PixelFormat.A8L8;
                    break;
                case PixelFormats.R16_UNORM:
                    ddpf = PixelFormat.L16;
                    break;
                case PixelFormats.R8_UNORM:
                    ddpf = PixelFormat.L8;
                    break;
                case PixelFormats.A8_UNORM:
                    ddpf = PixelFormat.A8;
                    break;
                case PixelFormats.R8G8_B8G8_UNORM:
                    ddpf = PixelFormat.R8G8_B8G8;
                    break;
                case PixelFormats.G8R8_G8B8_UNORM:
                    ddpf = PixelFormat.G8R8_G8B8;
                    break;
                case PixelFormats.BC1_UNORM:
                case PixelFormats.BC1_UNORM_SRGB:
                    ddpf = PixelFormat.DXT1;
                    break;
                case PixelFormats.BC2_UNORM:
                case PixelFormats.BC2_UNORM_SRGB:
                    ddpf = PixelFormat.DXT3;
                    break;
                case PixelFormats.BC3_UNORM:
                case PixelFormats.BC3_UNORM_SRGB:
                    ddpf = PixelFormat.DXT5;
                    break;
                case PixelFormats.BC4_UNORM:
                    ddpf = PixelFormat.BC4_UNorm;
                    break;
                case PixelFormats.BC4_SNORM:
                    ddpf = PixelFormat.BC4_SNorm;
                    break;
                case PixelFormats.BC5_UNORM:
                    ddpf = PixelFormat.BC5_UNorm;
                    break;
                case PixelFormats.BC5_SNORM:
                    ddpf = PixelFormat.BC5_SNorm;
                    break;
                case PixelFormats.B5G6R5_UNORM:
                    ddpf = PixelFormat.R5G6B5;
                    break;
                case PixelFormats.B5G5R5A1_UNORM:
                    ddpf = PixelFormat.A1R5G5B5;
                    break;
                case PixelFormats.B8G8R8A8_UNORM:
                    ddpf = PixelFormat.A8R8G8B8;
                    break; // DXGI 1.1
                case PixelFormats.B8G8R8X8_UNORM:
                    ddpf = PixelFormat.X8R8G8B8;
                    break; // DXGI 1.1

                    // Legacy D3DX formats using D3DFMT enum value as FourCC
                case PixelFormats.R32G32B32A32_FLOAT:
                    ddpf.Size = 32;
                    ddpf.Flags = PixelFormatFlags.FourCC;
                    ddpf.FourCC = 116; // D3DFMT_A32B32G32R32F
                    break;
                case PixelFormats.R16G16B16A16_FLOAT:
                    ddpf.Size = 32;
                    ddpf.Flags = PixelFormatFlags.FourCC;
                    ddpf.FourCC = 113; // D3DFMT_A16B16G16R16F
                    break;
                case PixelFormats.R16G16B16A16_UNORM:
                    ddpf.Size = 32;
                    ddpf.Flags = PixelFormatFlags.FourCC;
                    ddpf.FourCC = 36; // D3DFMT_A16B16G16R16
                    break;
                case PixelFormats.R16G16B16A16_SNORM:
                    ddpf.Size = 32;
                    ddpf.Flags = PixelFormatFlags.FourCC;
                    ddpf.FourCC = 110; // D3DFMT_Q16W16V16U16
                    break;
                case PixelFormats.R32G32_FLOAT:
                    ddpf.Size = 32;
                    ddpf.Flags = PixelFormatFlags.FourCC;
                    ddpf.FourCC = 115; // D3DFMT_G32R32F
                    break;
                case PixelFormats.R16G16_FLOAT:
                    ddpf.Size = 32;
                    ddpf.Flags = PixelFormatFlags.FourCC;
                    ddpf.FourCC = 112; // D3DFMT_G16R16F
                    break;
                case PixelFormats.R32_FLOAT:
                    ddpf.Size = 32;
                    ddpf.Flags = PixelFormatFlags.FourCC;
                    ddpf.FourCC = 114; // D3DFMT_R32F
                    break;
                case PixelFormats.R16_FLOAT:
                    ddpf.Size = 32;
                    ddpf.Flags = PixelFormatFlags.FourCC;
                    ddpf.FourCC = 111; // D3DFMT_R16F
                    break;
            }

            return ddpf;
        }

        #region Nested type: Header

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Header
        {
            public int Size;
            public HeaderFlags Flags;
            public uint Height;
            public uint Width;
            public int PitchOrLinearSize;
            public int Depth; // only if DDS_HEADER_FLAGS_VOLUME is set in dwFlags
            public int MipMapCount;

            private readonly uint unused1;
            private readonly uint unused2;
            private readonly uint unused3;
            private readonly uint unused4;
            private readonly uint unused5;
            private readonly uint unused6;
            private readonly uint unused7;
            private readonly uint unused8;
            private readonly uint unused9;
            private readonly uint unused10;
            private readonly uint unused11;

            public PixelFormat PixelFormat;
            public SurfaceFlags SurfaceFlags;
            public CubemapFlags CubemapFlags;

            private readonly uint Unused12;
            private readonly uint Unused13;

            private readonly uint Unused14;
        }

        #endregion

        #region Nested type: PitchFlags

        [Flags]
        public enum PitchFlags
        {
            None = 0x0, // Normal operation
            LegacyDword = 0x1, // Assume pitch is DWORD aligned instead of BYTE aligned
            Bpp24 = 0x10000, // Override with a legacy 24 bits-per-pixel format size
            Bpp16 = 0x20000, // Override with a legacy 16 bits-per-pixel format size
            Bpp8 = 0x40000, // Override with a legacy 8 bits-per-pixel format size
        };

        #endregion

        #region Nested type: PixelFormat

        /// <summary>
        /// Internal structure used to describe a DDS pixel format.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PixelFormat
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="PixelFormat" /> struct.
            /// </summary>
            /// <param name="flags">The flags.</param>
            /// <param name="fourCC">The four CC.</param>
            /// <param name="rgbBitCount">The RGB bit count.</param>
            /// <param name="rBitMask">The r bit mask.</param>
            /// <param name="gBitMask">The g bit mask.</param>
            /// <param name="bBitMask">The b bit mask.</param>
            /// <param name="aBitMask">A bit mask.</param>
            public PixelFormat(PixelFormatFlags flags, int fourCC, int rgbBitCount, uint rBitMask, uint gBitMask,
                               uint bBitMask, uint aBitMask)
            {
                Size = 32;
                Flags = flags;
                FourCC = fourCC;
                RGBBitCount = rgbBitCount;
                RBitMask = rBitMask;
                GBitMask = gBitMask;
                BBitMask = bBitMask;
                ABitMask = aBitMask;
            }

            public int Size;
            public PixelFormatFlags Flags;
            public int FourCC;
            public int RGBBitCount;
            public uint RBitMask;
            public uint GBitMask;
            public uint BBitMask;
            public uint ABitMask;

            public static readonly PixelFormat DXT1 = new PixelFormat(PixelFormatFlags.FourCC,
                                                                      new FourCC('D', 'X', 'T', '1'), 0, 0, 0, 0, 0);

            public static readonly PixelFormat DXT2 = new PixelFormat(PixelFormatFlags.FourCC,
                                                                      new FourCC('D', 'X', 'T', '2'), 0, 0, 0, 0, 0);

            public static readonly PixelFormat DXT3 = new PixelFormat(PixelFormatFlags.FourCC,
                                                                      new FourCC('D', 'X', 'T', '3'), 0, 0, 0, 0, 0);

            public static readonly PixelFormat DXT4 = new PixelFormat(PixelFormatFlags.FourCC,
                                                                      new FourCC('D', 'X', 'T', '4'), 0, 0, 0, 0, 0);

            public static readonly PixelFormat DXT5 = new PixelFormat(PixelFormatFlags.FourCC,
                                                                      new FourCC('D', 'X', 'T', '5'), 0, 0, 0, 0, 0);

            //public static readonly PixelFormat DXT5 = new PixelFormat(PixelFormatFlags.Rgba, 0, 8, 0x1f000000, 0x003f0000, 0x00001f00, 0x000000ff);

            public static readonly PixelFormat BC4_UNorm = new PixelFormat(PixelFormatFlags.FourCC,
                                                                           new FourCC('B', 'C', '4', 'U'), 0, 0, 0, 0, 0);

            public static readonly PixelFormat BC4_SNorm = new PixelFormat(PixelFormatFlags.FourCC,
                                                                           new FourCC('B', 'C', '4', 'S'), 0, 0, 0, 0, 0);

            public static readonly PixelFormat BC5_UNorm = new PixelFormat(PixelFormatFlags.FourCC,
                                                                           new FourCC('B', 'C', '5', 'U'), 0, 0, 0, 0, 0);

            public static readonly PixelFormat BC5_SNorm = new PixelFormat(PixelFormatFlags.FourCC,
                                                                           new FourCC('B', 'C', '5', 'S'), 0, 0, 0, 0, 0);

            public static readonly PixelFormat R8G8_B8G8 = new PixelFormat(PixelFormatFlags.FourCC,
                                                                           new FourCC('R', 'G', 'B', 'G'), 0, 0, 0, 0, 0);

            public static readonly PixelFormat G8R8_G8B8 = new PixelFormat(PixelFormatFlags.FourCC,
                                                                           new FourCC('G', 'R', 'G', 'B'), 0, 0, 0, 0, 0);

            public static readonly PixelFormat A8R8G8B8 = new PixelFormat(PixelFormatFlags.Rgba, 0, 32, 0x00ff0000,
                                                                          0x0000ff00, 0x000000ff, 0xff000000);

            public static readonly PixelFormat X8R8G8B8 = new PixelFormat(PixelFormatFlags.Rgb, 0, 32, 0x00ff0000,
                                                                          0x0000ff00, 0x000000ff, 0x00000000);

            public static readonly PixelFormat A8B8G8R8 = new PixelFormat(PixelFormatFlags.Rgba, 0, 32, 0x000000ff,
                                                                          0x0000ff00, 0x00ff0000, 0xff000000);

            public static readonly PixelFormat X8B8G8R8 = new PixelFormat(PixelFormatFlags.Rgb, 0, 32, 0x000000ff,
                                                                          0x0000ff00, 0x00ff0000, 0x00000000);

            public static readonly PixelFormat G16R16 = new PixelFormat(PixelFormatFlags.Rgb, 0, 32, 0x0000ffff,
                                                                        0xffff0000, 0x00000000, 0x00000000);

            public static readonly PixelFormat R5G6B5 = new PixelFormat(PixelFormatFlags.Rgb, 0, 16, 0x0000f800,
                                                                        0x000007e0, 0x0000001f, 0x00000000);

            public static readonly PixelFormat A1R5G5B5 = new PixelFormat(PixelFormatFlags.Rgba, 0, 16, 0x00007c00,
                                                                          0x000003e0, 0x0000001f, 0x00008000);

            public static readonly PixelFormat A4R4G4B4 = new PixelFormat(PixelFormatFlags.Rgba, 0, 16, 0x00000f00,
                                                                          0x000000f0, 0x0000000f, 0x0000f000);

            public static readonly PixelFormat R8G8B8 = new PixelFormat(PixelFormatFlags.Rgb, 0, 24, 0x00ff0000,
                                                                        0x0000ff00, 0x000000ff, 0x00000000);

            public static readonly PixelFormat L8 = new PixelFormat(PixelFormatFlags.Luminance, 0, 8, 0xff, 0x00, 0x00,
                                                                    0x00);

            public static readonly PixelFormat L16 = new PixelFormat(PixelFormatFlags.Luminance, 0, 16, 0xffff, 0x0000,
                                                                     0x0000, 0x0000);

            public static readonly PixelFormat A8L8 = new PixelFormat(PixelFormatFlags.LuminanceAlpha, 0, 16, 0x00ff,
                                                                      0x0000, 0x0000, 0xff00);

            public static readonly PixelFormat A8 = new PixelFormat(PixelFormatFlags.Alpha, 0, 8, 0x00, 0x00, 0x00, 0xff);

            //public static readonly PixelFormat DX10 = new PixelFormat(PixelFormatFlags.FourCC, new FourCC('D', 'X', '1', '0'), 0, 0, 0, 0, 0);
        }

        #endregion
    }
}