using moddingSuite.Model.Sav;
using System;
using System.IO;
namespace moddingSuite.BL
{
    /// <summary>
    /// 
    ///struct savheader
    ///{
    ///    DWORD magic;
    ///    DWORD null;
    ///    blob checksum[16];
    ///    DWORD fileSize;
    ///    DWORD contentSize;
    ///    DWORD contentSize2;
    ///   DWORD unknown1;
    ///   DWORD unknown2;
    ///    BYTE headerSize;
    ///    BYTE two;
    ///    blob padding[22];
    ///};
    /// </summary>
    public class SavManager
    {
        public const uint SavMagic = 0x30564153;

        public SavFile Read(byte[] data)
        {
            var ret = new SavFile();

            byte[] buffer = new byte[4];

            using (var ms = new MemoryStream(data))
            {
                ms.Read(buffer, 0, buffer.Length);
                uint magic = BitConverter.ToUInt32(buffer, 0);

                if (magic != SavMagic)
                    throw new InvalidDataException("Wrong sav magic");

                ms.Seek(4, SeekOrigin.Current);

                buffer = new byte[16];
                ms.Read(buffer, 0, buffer.Length);
                Array.Copy(buffer, ret.Checksum, 16);

                buffer = new byte[4];
                ms.Read(buffer, 0, buffer.Length);
                ret.FileSize = BitConverter.ToUInt32(buffer, 0);

                if (data.Length != ret.FileSize)
                    throw new InvalidDataException("Header doesn't match to filesize");

                ms.Read(buffer, 0, buffer.Length);
                ret.ContentSize1 = BitConverter.ToUInt32(buffer, 0);

                ms.Read(buffer, 0, buffer.Length);
                ret.ContentSize2 = BitConverter.ToUInt32(buffer, 0);

                buffer = new byte[8];
                ms.Read(buffer, 0, buffer.Length);

                ret.SaveDate = BitConverter.ToUInt64(buffer, 0);

            }

            return ret;
        }
    }
}
