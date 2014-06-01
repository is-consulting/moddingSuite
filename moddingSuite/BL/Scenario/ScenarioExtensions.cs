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

        public static void AssertAreaMagic(this MemoryStream ms)
        {
            var buffer = new byte[4];

            ms.Read(buffer, 0, buffer.Length);

            if (BitConverter.ToInt32(buffer, 0) != AreaMagic)
                throw new InvalidDataException("AREA expected");
        }
    }
}
