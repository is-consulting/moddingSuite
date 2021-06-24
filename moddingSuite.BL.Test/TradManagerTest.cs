using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using moddingSuite.BL;
using moddingSuite.Model.Edata;
using System.IO;
using System.Linq;

namespace moddingSuite.Test
{
    [TestClass]
    public class TradManagerTest: BaseTests
    {
        [DataTestMethod]
        [DataRow("/48574/ZZ_Win")]
        public void CanParseWindowsReadRawData(string path)
        {
            var manager = new EdataManager($"{RedDragonGameDataPath}{path}.dat");
            AssertAntiRadar(manager, StringType.Default);
        }

        private static void AssertAntiRadar(EdataManager manager, StringType stringType)
        {
            AssertUnites("DC02000000000000", "Anti-Radar", manager, stringType);
        }

        private static void AssertUnites(string hashView, string content, EdataManager manager, StringType stringType)
        {
            manager.ParseEdataFile();
            
            var unites = manager.Files.First(f => f.Path == EdataManager.KnownLocation.Unites);
            unites.FileType.Should().Be(EdataFileType.Dictionary);
            var bytes = manager.GetRawData(unites);
            bytes.Should().NotBeEmpty();

            var tradManager = new TradManager(bytes, stringType);
            tradManager.Entries.Should().NotBeEmpty();
            var antiRadar = tradManager.Entries.First(t => t.HashView == hashView);
            antiRadar.Content.Should().Be(content);
        }

        [DataTestMethod]
        [DataRow("/430000319/ZZ_Linux")]
        public void CanParseLinuxReadRawData(string path)
        {
            var manager = new EdataManager($"{RedDragonLinuxGameDataPath}{path}.dat");
            AssertAntiRadar(manager, StringType.Utf32);
        }
    }
}
