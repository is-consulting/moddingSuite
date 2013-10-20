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
    public class DDSReader
    {
        public TgvFile ReadDDS(byte[] input)
        {
            int contentSize = input.Length - Marshal.SizeOf(typeof(DDS.Header)) - Marshal.SizeOf((typeof(uint)));

            var file = new TgvFile();

            using (var ms = new MemoryStream(input))
            {
                var buffer = new byte[4];
                ms.Read(buffer, 0, buffer.Length);

                if (BitConverter.ToUInt32(buffer, 0) != DDS.MagicHeader)
                    throw new ArgumentException("Wrong DDS magic");

                buffer = new byte[Marshal.SizeOf(typeof(DDS.Header))];
                ms.Read(buffer, 0, buffer.Length);

                var header = ByteArrayToStructure<DDS.Header>(buffer);

                var mipSize = contentSize - contentSize / header.MipMapCount;

                for (ushort i = 0; i < header.MipMapCount; i++)
                {
                    buffer = new byte[mipSize];
                    ms.Read(buffer, 0, buffer.Length);

                    file.MipMaps.Add(buffer);

                    mipSize /= 4;
                }

                file.Height = header.Height;
                file.ImageHeight = header.Height;
                file.Width = header.Width;
                file.ImageHeight = header.Width;

                file.MipMapCount = (ushort)header.MipMapCount;

                DDSHelper.ConversionFlags conversionFlags;

                file.Format = DDSHelper.GetDXGIFormat(ref header.PixelFormat, out conversionFlags);
            }

            return file;
        }

        T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            var stuff = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();

            return stuff;
        }
    }
}
