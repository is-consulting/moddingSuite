using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using moddingSuite.BL.Utils;

namespace moddingSuite.BL.Test
{
    [TestClass]
    public class UtilsTests
    {

        [DataTestMethod]
        [DataRow("K4", "4505000000000000")]
        public void TestHash(string name, string expectedHashView)
        {
            var hash = StdUtils.CreateLocalisationHash(name, name.Length);

            var newHashView = StdUtils.ByteArrayToBigEndianHexByteString(hash);
            newHashView.Should().Be(expectedHashView);
        }
    }
}
