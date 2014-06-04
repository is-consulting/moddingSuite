using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.BL.Scenario
{
    public static class ScenarioExtensions
    {
        public const int AreaMagic = 1095062081;

        public static void AssertAreaMagic(this Stream s)
        {
            var buffer = new byte[4];

            s.Read(buffer, 0, buffer.Length);

            if (BitConverter.ToInt32(buffer, 0) != AreaMagic)
                throw new InvalidDataException("AREA expected");
        }

        public static void WriteAreaMagic(this Stream s)
        {
            var buffer = BitConverter.GetBytes(AreaMagic);
            s.Write(buffer, 0, buffer.Length);
        }
    }
}
