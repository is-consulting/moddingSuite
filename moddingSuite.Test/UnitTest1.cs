using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using moddingSuite.BL;

namespace moddingSuite.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {

        }

        [TestMethod]
        public void RepackIt()
        {
            var path = @"C:\Users\enohka\Desktop\Teststuff\texture reversing\t72m1_tsccombds_combineddstexture01.tgv";
            var path2 = @"C:\Users\enohka\Desktop\Teststuff\texture reversing\t72m1_tsccombds_combineddstexture01.dds";


            var mgr = new TgvManager(File.ReadAllBytes(path));


            var buffer = mgr.CreateDds();

            using (var fs = File.Create(path2))
                fs.Write(buffer, 0, buffer.Length);


        }
    }
}
