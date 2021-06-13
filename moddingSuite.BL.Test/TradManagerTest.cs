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
            manager.ParseEdataFile();

            var config = manager.Files.First(f => f.Path == @"pc\localisation\us\localisation\unites.dic");
            config.FileType.Should().Be(EdataFileType.Dictionary);
            var bytes = manager.GetRawData(config);
            bytes.Should().NotBeEmpty();

            var tradManager = new TradManager(bytes, StringType.Default);
            tradManager.Entries.Should().NotBeEmpty();
            var antiRadar = tradManager.Entries.First(t => t.HashView == "DC02000000000000");
            antiRadar.Content.Should().Be("Anti-Radar");
        }

        [DataTestMethod]
        [DataRow("/430000319/ZZ_Linux")]
        public void CanParseLinuxReadRawData(string path)
        {
            var manager = new EdataManager($"{RedDragonLinuxGameDataPath}{path}.dat");
            manager.ParseEdataFile();

            var config = manager.Files.First(f => f.Path == @"pc\localisation\us\localisation\unites.dic");
            config.FileType.Should().Be(EdataFileType.Dictionary);
            var bytes = manager.GetRawData(config);
            bytes.Should().NotBeEmpty();

            var tradManager = new TradManager(bytes, StringType.Utf32);
            tradManager.Entries.Should().NotBeEmpty();
            var antiRadar = tradManager.Entries.First(t => t.HashView == "DC02000000000000");
            antiRadar.Content.Should().Be("Anti-Radar");
        }
    }
}
