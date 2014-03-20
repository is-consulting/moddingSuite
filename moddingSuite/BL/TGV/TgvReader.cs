using moddingSuite.BL.Compressing;
using moddingSuite.BL.DDS;
using moddingSuite.Model.Textures;
using moddingSuite.Util;
using System;
using System.IO;
using System.Text;

namespace moddingSuite.BL.TGV
{
    public class TgvReader
    {
        public TgvFile Read(Stream ms)
        {
            var file = new TgvFile();

            var buffer = new byte[4];

            ms.Read(buffer, 0, buffer.Length);
            file.Version = BitConverter.ToUInt32(buffer, 0);

            ms.Read(buffer, 0, buffer.Length);
            file.IsCompressed = BitConverter.ToInt32(buffer, 0) > 0;

            ms.Read(buffer, 0, buffer.Length);
            file.Width = BitConverter.ToUInt32(buffer, 0);
            ms.Read(buffer, 0, buffer.Length);
            file.Height = BitConverter.ToUInt32(buffer, 0);

            ms.Read(buffer, 0, buffer.Length);
            file.ImageWidth = BitConverter.ToUInt32(buffer, 0);
            ms.Read(buffer, 0, buffer.Length);
            file.ImageHeight = BitConverter.ToUInt32(buffer, 0);

            buffer = new byte[2];

            ms.Read(buffer, 0, buffer.Length);
            file.MipMapCount = BitConverter.ToUInt16(buffer, 0);

            ms.Read(buffer, 0, buffer.Length);
            ushort pixelFormatLen = BitConverter.ToUInt16(buffer, 0);

            buffer = new byte[pixelFormatLen];

            ms.Read(buffer, 0, buffer.Length);
            file.PixelFormatStr = Encoding.ASCII.GetString(buffer);

            ms.Seek(Utils.RoundToNextDivBy4(pixelFormatLen) - pixelFormatLen, SeekOrigin.Current);

            buffer = new byte[16];
            ms.Read(buffer, 0, buffer.Length);
            file.SourceChecksum = (byte[])buffer.Clone();

            buffer = new byte[4];

            for (int i = 0; i < file.MipMapCount; i++)
            {
                ms.Read(buffer, 0, buffer.Length);
                uint offset = BitConverter.ToUInt32(buffer, 0);
                file.Offsets.Add(offset);
            }

            for (int i = 0; i < file.MipMapCount; i++)
            {
                ms.Read(buffer, 0, buffer.Length);
                uint offset = BitConverter.ToUInt32(buffer, 0);
                file.Sizes.Add(offset);
            }

            for (int i = 0; i < file.MipMapCount; i++)
                file.MipMaps.Add(ReadMip(ms, file, i));

            file.Format = TranslatePixelFormat(file.PixelFormatStr);

            //if (file.Width != file.ImageWidth || file.Height != file.ImageHeight)
            //{
            //    throw new InvalidDataException("something interresting happened here");
            //}

            return file;
        }

        public TgvFile Read(byte[] data)
        {
            using (var ms = new MemoryStream(data))
                return Read(ms);

            //var file = new TgvFile();

            //using (var ms = new MemoryStream(data))
            //{
            //    var buffer = new byte[4];

            //    ms.Read(buffer, 0, buffer.Length);
            //    file.Version = BitConverter.ToUInt32(buffer, 0);

            //    ms.Read(buffer, 0, buffer.Length);
            //    file.IsCompressed = BitConverter.ToInt32(buffer, 0) > 0;

            //    ms.Read(buffer, 0, buffer.Length);
            //    file.Width = BitConverter.ToUInt32(buffer, 0);
            //    ms.Read(buffer, 0, buffer.Length);
            //    file.Height = BitConverter.ToUInt32(buffer, 0);

            //    ms.Read(buffer, 0, buffer.Length);
            //    file.ImageWidth = BitConverter.ToUInt32(buffer, 0);
            //    ms.Read(buffer, 0, buffer.Length);
            //    file.ImageHeight = BitConverter.ToUInt32(buffer, 0);

            //    buffer = new byte[2];

            //    ms.Read(buffer, 0, buffer.Length);
            //    file.MipMapCount = BitConverter.ToUInt16(buffer, 0);

            //    ms.Read(buffer, 0, buffer.Length);
            //    ushort pixelFormatLen = BitConverter.ToUInt16(buffer, 0);

            //    buffer = new byte[pixelFormatLen];

            //    ms.Read(buffer, 0, buffer.Length);
            //    file.PixelFormatStr = Encoding.ASCII.GetString(buffer);

            //    ms.Seek(Utils.RoundToNextDivBy4(pixelFormatLen) - pixelFormatLen, SeekOrigin.Current);

            //    buffer = new byte[16];
            //    ms.Read(buffer, 0, buffer.Length);
            //    file.SourceChecksum = (byte[])buffer.Clone();

            //    buffer = new byte[4];

            //    for (int i = 0; i < file.MipMapCount; i++)
            //    {
            //        ms.Read(buffer, 0, buffer.Length);
            //        uint offset = BitConverter.ToUInt32(buffer, 0);
            //        file.Offsets.Add(offset);
            //    }

            //    for (int i = 0; i < file.MipMapCount; i++)
            //    {
            //        ms.Read(buffer, 0, buffer.Length);
            //        uint offset = BitConverter.ToUInt32(buffer, 0);
            //        file.Sizes.Add(offset);
            //    }

            //    for (int i = 0; i < file.MipMapCount; i++)
            //        file.MipMaps.Add(ReadMip(i, data, file));
            //}

            //file.Format = TranslatePixelFormat(file.PixelFormatStr);

            ////if (file.Width != file.ImageWidth || file.Height != file.ImageHeight)
            ////{
            ////    throw new InvalidDataException("something interresting happened here");
            ////}

            //return file;
        }

        /// <summary>
        /// This method is stream and order dependant, don't use outside.
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="file"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private TgvMipMap ReadMip(Stream ms, TgvFile file, int id)
        {
            if (id > file.MipMapCount)
                throw new ArgumentException("id");

            var zipo = new byte[] { 0x5A, 0x49, 0x50, 0x4F };

            var mipMap = new TgvMipMap(file.Offsets[id], file.Sizes[id], 0);

            byte[] buffer;

            if (file.IsCompressed)
            {
                buffer = new byte[4];

                ms.Read(buffer, 0, buffer.Length);
                if (!Utils.ByteArrayCompare(buffer, zipo))
                    throw new InvalidDataException("Mipmap has to start with \"ZIPO\"!");

                ms.Read(buffer, 0, buffer.Length);
                mipMap.MipWidth = BitConverter.ToInt32(buffer, 0);

                buffer = new byte[mipMap.Size - 8];
            }
            else
                buffer = new byte[mipMap.Size];

            ms.Read(buffer, 0, buffer.Length);

            ms.Seek(Utils.RoundToNextDivBy4((int)mipMap.Size)-mipMap.Size, SeekOrigin.Current);

            if (file.IsCompressed)
                buffer = Compressor.Decomp(buffer);

            mipMap.Content = buffer;

            return mipMap;
        }

        protected PixelFormats TranslatePixelFormat(string pixelFormat)
        {
            switch (pixelFormat)
            {
                case "A8R8G8B8_HDR":
                case "A8R8G8B8_LIN":
                case "A8R8G8B8_LIN_HDR":
                case "A8R8G8B8":
                    return PixelFormats.R8G8B8A8_UNORM;
                case "X8R8G8B8":
                case "X8R8G8B8_LE":
                    return PixelFormats.B8G8R8X8_UNORM;
                case "X8R8G8B8_SRGB":
                    return PixelFormats.B8G8R8X8_UNORM_SRGB;

                case "A8R8G8B8_SRGB":
                case "A8R8G8B8_SRGB_HDR":
                    return PixelFormats.R8G8B8A8_UNORM_SRGB;

                case "A16B16G16R16":
                case "A16B16G16R16_EDRAM":
                    return PixelFormats.R16G16B16A16_UNORM;

                case "A16B16G16R16F":
                case "A16B16G16R16F_LIN":
                    return PixelFormats.R16G16B16A16_FLOAT;

                case "A32B32G32R32F":
                case "A32B32G32R32F_LIN":
                    return PixelFormats.R32G32B32A32_FLOAT;

                case "A8":
                case "A8_LIN":
                    return PixelFormats.A8_UNORM;
                case "A8P8":
                    return PixelFormats.A8P8;
                case "P8":
                    return PixelFormats.P8;
                case "L8":
                case "L8_LIN":
                    return PixelFormats.R8_UNORM;
                case "L16":
                case "L16_LIN":
                    return PixelFormats.R16_UNORM;
                case "D16_LOCKABLE":
                case "D16":
                case "D16F":
                    return PixelFormats.D16_UNORM;
                case "V8U8":
                    return PixelFormats.R8G8_SNORM;
                case "V16U16":
                    return PixelFormats.R16G16_SNORM;

                case "DXT1":
                case "DXT1_LIN":
                    return PixelFormats.BC1_UNORM;
                case "DXT1_SRGB":
                    return PixelFormats.BC1_UNORM_SRGB;
                case "DXT2":
                case "DXT3":
                case "DXT3_LIN":
                    return PixelFormats.BC2_UNORM;
                case "DXT3_SRGB":
                    return PixelFormats.BC2_UNORM_SRGB;
                case "DXT4":
                case "DXT5":
                case "DXT5_LIN":
                case "DXT5_FROM_ENCODE":
                    return PixelFormats.BC3_UNORM;
                case "DXT5_SRGB":
                    return PixelFormats.BC3_UNORM_SRGB;

                case "R5G6B5_LIN":
                case "R5G6B5":
                case "R8G8B8":
                case "X1R5G5B5":
                case "X1R5G5B5_LIN":
                case "A1R5G5B5":
                case "A4R4G4B4":
                case "R3G3B2":
                case "A8R3G3B2":
                case "X4R4G4B4":
                case "A8L8":
                case "A4L4":
                case "L6V5U5":
                case "X8L8V8U8":
                case "Q8W8U8V8":
                case "W11V11U10":
                case "UYVY":
                case "YUY2":
                case "D32":
                case "D32F_LOCKABLE":
                case "D15S1":
                case "D24S8":
                case "R16F":
                case "R32F":
                case "R32F_LIN":
                case "A2R10G10B10":
                case "D24X8":
                case "D24X8F":
                case "D24X4S4":
                case "G16R16":
                case "G16R16_EDRAM":
                case "G16R16F":
                case "G16R16F_LIN":
                case "G32R32F":
                case "G32R32F_LIN":
                case "A2R10G10B10_LE":
                case "CTX1":
                case "CTX1_LIN":
                case "DXN":
                case "DXN_LIN":
                case "INTZ":
                case "RAWZ":
                case "DF24":
                case "PIXNULL":
                    throw new NotSupportedException(string.Format("Pixelformat {0} not supported", pixelFormat));

                default:
                    throw new NotSupportedException(string.Format("Unknown Pixelformat {0} ", pixelFormat));
            }
        }
    }
}
