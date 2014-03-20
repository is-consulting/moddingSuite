using moddingSuite.BL.Compressing;
using moddingSuite.BL.DDS;
using moddingSuite.Model.Textures;
using moddingSuite.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace moddingSuite.BL.TGV
{
    public class TgvWriter
    {
        // We do write TGV files in version 2.
        public const uint Version = 0x00000002;

        public void Write(Stream destStream, TgvFile sourceFile, byte[] sourceChecksum, bool compress = true)
        {
            if (sourceChecksum.Length > 16)
                throw new ArgumentException("sourceChecksum");

            sourceFile.PixelFormatStr = TranslatePixelFormatBack(sourceFile.Format);
            var zipoMagic = Encoding.ASCII.GetBytes("ZIPO");

            var buffer = BitConverter.GetBytes(Version);
            destStream.Write(buffer, 0, buffer.Length);

            buffer = BitConverter.GetBytes(compress ? 1 : 0);
            destStream.Write(buffer, 0, buffer.Length);

            buffer = BitConverter.GetBytes(sourceFile.Width);
            destStream.Write(buffer, 0, buffer.Length);
            buffer = BitConverter.GetBytes(sourceFile.Height);
            destStream.Write(buffer, 0, buffer.Length);

            buffer = BitConverter.GetBytes(sourceFile.Width);
            destStream.Write(buffer, 0, buffer.Length);
            buffer = BitConverter.GetBytes(sourceFile.Height);
            destStream.Write(buffer, 0, buffer.Length);

            buffer = BitConverter.GetBytes((short)sourceFile.MipMapCount);
            destStream.Write(buffer, 0, buffer.Length);

            var fmtLen = (short)sourceFile.PixelFormatStr.Length;
            buffer = BitConverter.GetBytes(fmtLen);
            destStream.Write(buffer, 0, buffer.Length);

            buffer = Encoding.ASCII.GetBytes(sourceFile.PixelFormatStr);
            destStream.Write(buffer, 0, buffer.Length);
            destStream.Seek(Utils.RoundToNextDivBy4(fmtLen) - fmtLen, SeekOrigin.Current);

            destStream.Write(sourceChecksum, 0, sourceChecksum.Length);

            var mipdefOffset = (destStream.Position);

            var mipImgsizes = new List<int>();
            var tileSize = sourceFile.Width - sourceFile.Width / sourceFile.MipMapCount;

            for (int i = 0; i < sourceFile.MipMapCount; i++)
            {
                destStream.Seek(8, SeekOrigin.Current);
                mipImgsizes.Add((int)tileSize);
                tileSize /= 4;
            }

            var sortedMipMaps = sourceFile.MipMaps.OrderBy(x => x.Content.Length).ToList();

            //mipImgsizes = mipImgsizes.OrderBy(x => x).ToList();

            // Create the content and write all MipMaps, 
            // since we compress on this part its the first part where we know the size of a MipMap
            foreach (var sortedMipMap in sortedMipMaps)
            {
                sortedMipMap.Offset = (uint)destStream.Position;
                if (compress)
                {
                    destStream.Write(zipoMagic, 0, zipoMagic.Length);

                    //buffer = BitConverter.GetBytes(mipImgsizes[sortedMipMaps.IndexOf(sortedMipMap)]);
                    buffer = BitConverter.GetBytes((int)Math.Pow(4, sortedMipMaps.IndexOf(sortedMipMap)));
                    destStream.Write(buffer, 0, buffer.Length);

                    buffer = Compressor.Comp(sortedMipMap.Content);
                    destStream.Write(buffer, 0, buffer.Length);
                    sortedMipMap.Size = (uint)buffer.Length;
                }
                else
                {
                    destStream.Write(sortedMipMap.Content, 0, sortedMipMap.Content.Length);
                    sortedMipMap.Size = (uint)sortedMipMap.Content.Length;
                }
            }

            destStream.Seek(mipdefOffset, SeekOrigin.Begin);

            // Write the offset collection in the header.
            for (int i = 0; i < sourceFile.MipMapCount; i++)
            {
                buffer = BitConverter.GetBytes(sortedMipMaps[i].Offset);
                destStream.Write(buffer, 0, buffer.Length);
            }

            // Write the size collection into the header.
            for (int i = 0; i < sourceFile.MipMapCount; i++)
            {
                buffer = BitConverter.GetBytes(sortedMipMaps[i].Size + 8);
                destStream.Write(buffer, 0, buffer.Length);
            }
        }

        protected string TranslatePixelFormatBack(PixelFormats pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormats.R8G8B8A8_UNORM:
                    return "A8R8G8B8";
                case PixelFormats.B8G8R8X8_UNORM:
                    return "X8R8G8B8";
                case PixelFormats.B8G8R8X8_UNORM_SRGB:
                    return "X8R8G8B8_SRGB";

                case PixelFormats.R8G8B8A8_UNORM_SRGB:
                    return "A8R8G8B8_SRGB";

                case PixelFormats.R16G16B16A16_UNORM:
                    return "A16B16G16R16";

                case PixelFormats.R16G16B16A16_FLOAT:
                    return "A16B16G16R16F";

                case PixelFormats.R32G32B32A32_FLOAT:
                    return "A32B32G32R32F";

                case PixelFormats.A8_UNORM:
                    return "A8";
                case PixelFormats.A8P8:
                    return "A8P8";
                case PixelFormats.P8:
                    return "P8";
                case PixelFormats.R8_UNORM:
                    return "L8";
                case PixelFormats.R16_UNORM:
                    return "L16";
                case PixelFormats.D16_UNORM:
                    return "D16";
                case PixelFormats.R8G8_SNORM:
                    return "V8U8";
                case PixelFormats.R16G16_SNORM:
                    return "V16U16";

                case PixelFormats.BC1_UNORM:
                    return "DXT1";
                case PixelFormats.BC1_UNORM_SRGB:
                    return "DXT1_SRGB";
                case PixelFormats.BC2_UNORM:
                    return "DXT3";
                case PixelFormats.BC2_UNORM_SRGB:
                    return "DXT3_SRGB";
                case PixelFormats.BC3_UNORM:
                    return "DXT5";
                case PixelFormats.BC3_UNORM_SRGB:
                    return "DXT5_SRGB";

                default:
                    throw new NotSupportedException(string.Format("Unsupported PixelFormat {0}", pixelFormat));
            }
        }
    }
}
