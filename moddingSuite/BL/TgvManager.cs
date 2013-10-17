using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using moddingSuite.Compressing;
using moddingSuite.Model.Textures;
using moddingSuite.Util;

namespace moddingSuite.BL
{
    public class TgvManager
    {
        public TgvManager(byte[] fileData)
        {
            Data = fileData;

            CurrentFile = ReadFile(Data);
        }

        public byte[] Data { get; protected set; }

        public TgvFile CurrentFile { get; protected set; }

        private TgvFile ReadFile(byte[] data)
        {
            var file = new TgvFile();

            using (var ms = new MemoryStream(data))
            {
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
                file.ImageHeight = BitConverter.ToUInt32(buffer, 0);
                ms.Read(buffer, 0, buffer.Length);
                file.ImageWidth = BitConverter.ToUInt32(buffer, 0);

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
                file.SourceChecksum = (byte[]) buffer.Clone();

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
            }

            return file;
        }

        public byte[] ReadMipMap(uint id)
        {
            if (id > CurrentFile.MipMapCount)
                throw new ArgumentException("id");

            var zipo = new byte[] {0x5A, 0x49, 0x50, 0x4F};

            using (
                var ms = new MemoryStream(Data, (int) CurrentFile.Offsets[(int) id], (int) CurrentFile.Sizes[(int) id]))
            {
                var buffer = new byte[4];
                var mipSize = (int) CurrentFile.Sizes[(int) id];

                if (CurrentFile.IsCompressed)
                {
                    ms.Read(buffer, 0, buffer.Length);

                    if (!Utils.ByteArrayCompare(buffer, zipo))
                        throw new InvalidDataException("Mipmap has to start with \"ZIPO\"!");

                    mipSize = ms.Read(buffer, 0, buffer.Length);
                }

                buffer = new byte[ms.Length - ms.Position];
                ms.Read(buffer, 0, buffer.Length);

                if (CurrentFile.IsCompressed)
                    buffer = Compressor.Decomp(buffer);

                //if (mipSize != buffer.Length)
                //    throw new InvalidDataException("Wrong sizes for mipmap given!");

                return buffer;
            }
        }

        public List<byte[]> GetMipMaps()
        {
            var ret = new List<byte[]>();

            for (int i = 0; i < CurrentFile.MipMapCount; i++)
                ret.Add(ReadMipMap((uint) i));

            return ret.OrderByDescending(ret.IndexOf).ToList();
        }

        /// <summary>
        ///  http://msdn.microsoft.com/en-us/library/windows/desktop/bb943991(v=vs.85).aspx
        /// </summary>
        /// <returns></returns>
        public byte[] CreateDds()
        {
            var ddsMagic = new byte[] {0x44, 0x44, 0x53, 0x20};

            int flags = 0x1 | 0x2 | 0x4 | 0x1000;

            if (CurrentFile.MipMapCount > 1)
                flags |= 0x20000;

            int pitch = (((int) CurrentFile.Width + 1) >> 1)*4;
            int depth = 0;

            int caps = 0x1000;

            int pfFlags = 0x1 | 0x4 | 0x40;
            string pfFourCc = "DXT5";
            int pfBitsPerPixel = 8;
            uint pfRBitMask = 0x00ff0000;
            uint pfGBitMask = 0x0000ff00;
            uint pfBBitMask = 0x000000ff;
            uint pfABitMask = 0xff000000;


            using (var ms = new MemoryStream())
            {
                // MAGIC
                ms.Write(ddsMagic, 0, ddsMagic.Length);

                // HEADER
                byte[] buffer = BitConverter.GetBytes(124);
                ms.Write(buffer, 0, buffer.Length);

                buffer = BitConverter.GetBytes(flags);
                ms.Write(buffer, 0, buffer.Length);

                buffer = BitConverter.GetBytes(CurrentFile.ImageHeight);
                ms.Write(buffer, 0, buffer.Length);

                buffer = BitConverter.GetBytes(CurrentFile.ImageWidth);
                ms.Write(buffer, 0, buffer.Length);

                buffer = BitConverter.GetBytes(pitch);
                ms.Write(buffer, 0, buffer.Length);

                buffer = BitConverter.GetBytes(depth);
                ms.Write(buffer, 0, buffer.Length);

                buffer = BitConverter.GetBytes(CurrentFile.MipMapCount - 1);
                ms.Write(buffer, 0, buffer.Length);

                buffer = new byte[11*4];
                ms.Write(buffer, 0, buffer.Length);

                // Pixel Format
                buffer = BitConverter.GetBytes(32);
                ms.Write(buffer, 0, buffer.Length);

                buffer = BitConverter.GetBytes(pfFlags);
                ms.Write(buffer, 0, buffer.Length);

                buffer = Encoding.ASCII.GetBytes(pfFourCc);
                if (buffer.Length > 4)
                    throw new InvalidDataException("Invalid Four CC in PixelFormat");
                ms.Write(buffer, 0, buffer.Length);

                buffer = BitConverter.GetBytes(pfBitsPerPixel);
                ms.Write(buffer, 0, buffer.Length);

                buffer = BitConverter.GetBytes(pfRBitMask);
                ms.Write(buffer, 0, buffer.Length);
                buffer = BitConverter.GetBytes(pfGBitMask);
                ms.Write(buffer, 0, buffer.Length);
                buffer = BitConverter.GetBytes(pfBBitMask);
                ms.Write(buffer, 0, buffer.Length);
                buffer = BitConverter.GetBytes(pfABitMask);
                ms.Write(buffer, 0, buffer.Length);
                // Pixel Format

                buffer = BitConverter.GetBytes(caps);
                ms.Write(buffer, 0, buffer.Length);

                buffer = BitConverter.GetBytes(0);
                ms.Write(buffer, 0, buffer.Length);

                buffer = BitConverter.GetBytes(0);
                ms.Write(buffer, 0, buffer.Length);
                buffer = BitConverter.GetBytes(0);
                ms.Write(buffer, 0, buffer.Length);

                buffer = BitConverter.GetBytes(0);
                ms.Write(buffer, 0, buffer.Length);
                // Header

                foreach (var mipMap in GetMipMaps())
                {
                    ms.Write(mipMap, 0, mipMap.Length);
                }

                return ms.ToArray();
            }
        }
    }
}