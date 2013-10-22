using System;
using System.IO;
using System.Linq;
using moddingSuite.Model.Textures;
using moddingSuite.Util;

namespace moddingSuite.BL.DDS
{
    /// <summary>
    /// Writes a DDS File     
    /// </summary>
    public class DDSWriter
    {
        public byte[] CreateDDSFile(TgvFile file)
        {
            using (var ms = new MemoryStream())
            {
                byte[] buffer = BitConverter.GetBytes(DDS.MagicHeader);
                ms.Write(buffer, 0, buffer.Length);

                buffer = CreateDDSHeader(file);
                ms.Write(buffer, 0, buffer.Length);

                buffer = file.MipMaps.OrderByDescending(x => x.MipWidth).First().Content;
                ms.Write(buffer, 0, buffer.Length);

                return ms.ToArray();
            }
        }

        protected byte[] CreateDDSHeader(TgvFile file)
        {
            var hd = new DDS.Header
                         {
                             Size = 124,
                             Flags = DDS.HeaderFlags.Texture,
                             SurfaceFlags = DDS.SurfaceFlags.Texture,
                             Width = file.Width,
                             Height = file.Height,
                             Depth = 1,
                         };

            DDS.PixelFormat ddpf = DDS.PixelFormatFromDXGIFormat(file.Format);

            int rowPitch, slicePitch;
            int newWidth, newHeight;
            DDS.ComputePitch(file.Format, (int)file.Width, (int)file.Height, out rowPitch, out slicePitch, out newWidth,
                         out newHeight);

            if (DDS.IsCompressedFormat(file.Format))
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

            return Utils.StructToBytes(hd);
        }
    }
}