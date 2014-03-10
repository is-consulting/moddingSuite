using System;
using System.IO;
using System.Runtime.InteropServices;
using moddingSuite.BL.DDS;
using moddingSuite.Model.Textures;
using moddingSuite.Util;

namespace moddingSuite.BL.TGV
{
    public class TgvDDSReader
    {
        public TgvFile ReadDDS(byte[] input)
        {
            int contentSize = input.Length - Marshal.SizeOf(typeof(DDS.DDS.Header)) - Marshal.SizeOf((typeof(uint)));

            var file = new TgvFile();

            using (var ms = new MemoryStream(input))
            {
                var buffer = new byte[4];
                ms.Read(buffer, 0, buffer.Length);

                if (BitConverter.ToUInt32(buffer, 0) != DDS.DDS.MagicHeader)
                    throw new ArgumentException("Wrong DDS magic");

                buffer = new byte[Marshal.SizeOf(typeof(DDS.DDS.Header))];
                ms.Read(buffer, 0, buffer.Length);

                var header = Utils.ByteArrayToStructure<DDS.DDS.Header>(buffer);

                int mipSize = contentSize;

                if (header.MipMapCount == 0)
                    header.MipMapCount = 1;        
                else
                    mipSize -= contentSize / header.MipMapCount;

                for (ushort i = 0; i < header.MipMapCount; i++)
                {
                    buffer = new byte[mipSize];
                    ms.Read(buffer, 0, buffer.Length);

                    var mip = new TgvMipMap { Content = buffer };

                    file.MipMaps.Add(mip);

                    mipSize /= 4;
                }

                file.Height = header.Height;
                file.ImageHeight = header.Height;
                file.Width = header.Width;
                file.ImageWidth = header.Width;

                file.MipMapCount = (ushort)header.MipMapCount;

                DDSHelper.ConversionFlags conversionFlags;

                file.Format = DDSHelper.GetDXGIFormat(ref header.PixelFormat, out conversionFlags);
            }

            return file;
        }
    }
}