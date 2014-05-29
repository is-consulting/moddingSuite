using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using moddingSuite.BL.Ndf;
using moddingSuite.Model.Scenario;
using moddingSuite.Util;

namespace moddingSuite.BL.Scenario
{
    public class ScenarioReader
    {
        public ScenarioFile Read(byte[] data)
        {
            using (var ms = new MemoryStream(data))
                return Read(ms);
        }

        public ScenarioFile Read(Stream s)
        {
            var f = new ScenarioFile();

            var buffer = new byte[10];
            s.Read(buffer, 0, buffer.Length);

            if (!Utils.ByteArrayCompare(Encoding.ASCII.GetBytes("SCENARIO\r\n"), buffer))
                throw new InvalidDataException("Wrong scenario header magic!");

            buffer = new byte[16];
            s.Read(buffer, 0, buffer.Length);
            f.Checksum = buffer;

            s.Seek(2, SeekOrigin.Current);

            buffer = new byte[4];
            s.Read(buffer, 0, buffer.Length);
            f.Version = BitConverter.ToInt32(buffer, 0);

            s.Read(buffer, 0, buffer.Length);
            int subFilesCount = BitConverter.ToInt32(buffer, 0);

            for (int i = 0; i < subFilesCount; i++)
            {
                s.Read(buffer, 0, buffer.Length);
                var contentFileBuffer = new byte[BitConverter.ToInt32(buffer, 0)];
                s.Read(contentFileBuffer, 0, contentFileBuffer.Length);
                f.ContentFiles.Add(contentFileBuffer);
            }

            var reader = new NdfbinReader();
            f.NdfBinary = reader.Read(f.ContentFiles[1]);

            return f;
        }

    }
}
