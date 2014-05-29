using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using moddingSuite.Model.Scenario;

namespace moddingSuite.BL.Scenario
{
    public class ScenarioWriter
    {
        public const int Version = 4;

        public byte[] CreateScenarioFile(ScenarioFile file)
        {
            var scenarioData = new List<byte>();

            scenarioData.AddRange(Encoding.ASCII.GetBytes("SCENARIO\r\n"));
            scenarioData.AddRange(BitConverter.GetBytes(Version));
            scenarioData.AddRange(BitConverter.GetBytes(file.ContentFiles.Count));

            foreach (var contentFile in file.ContentFiles)
            {
                scenarioData.AddRange(BitConverter.GetBytes(contentFile.Length));
                scenarioData.AddRange(contentFile);
            }

            byte[] hash = MD5.Create().ComputeHash(scenarioData.ToArray());

            scenarioData.InsertRange(10, hash.Concat(new byte[] {0x00, 0x00}));

            return scenarioData.ToArray();
        }
    }
}