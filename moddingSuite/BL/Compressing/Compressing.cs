using System.IO;
using Ionic.Zlib;

namespace moddingSuite.BL.Compressing
{
    public static class Compressor
    {
        public static byte[] Decomp(byte[] input)
        {
            using (var output = new MemoryStream())
            {
                Decomp(input, output);
                return output.ToArray();
            }
        }

        public static void Decomp(byte[] input, Stream outputStream)
        {
            using (var zipStream = new ZlibStream(outputStream, CompressionMode.Decompress))
            {
                using (var inputStream = new MemoryStream(input))
                {
                    byte[] buffer = input.Length > 4096 ? new byte[4096] : new byte[input.Length];

                    int size;

                    while ((size = inputStream.Read(buffer, 0, buffer.Length)) != 0)
                        zipStream.Write(buffer, 0, size);
                }
            }
        }

        public static byte[] Comp(byte[] input)
        {
            using (var sourceStream = new MemoryStream(input))
            {
                using (var compressed = new MemoryStream())
                {
                    using (var zipSteam = new ZlibStream(compressed, CompressionMode.Compress, CompressionLevel.Level9, true))
                    {
                        zipSteam.FlushMode = FlushType.Full;

                        //var buffer = new byte[1024];
                        //int len = sourceStream.Read(buffer, 0, buffer.Length);

                        //while (len > 0)
                        //{
                        //    zipSteam.Write(buffer, 0, len);
                        //    len = sourceStream.Read(buffer, 0, buffer.Length);
                        //}

                        sourceStream.CopyTo(zipSteam);

                        zipSteam.Flush();

                        return compressed.ToArray();
                    }
                }
            }
        }

    }
}
