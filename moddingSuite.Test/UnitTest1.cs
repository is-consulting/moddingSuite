using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using moddingSuite.BL;
using moddingSuite.BL.DDS;

namespace moddingSuite.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            SavManager mgr = new SavManager();

            using (var fs = new FileStream(@"D:\Programme\Steam\userdata\2346410\58610\remote\profile.wargame", FileMode.Open))
            {
                using (var ms = new MemoryStream())
                {
                    fs.CopyTo(ms);

                    mgr.Read(ms.ToArray());
                }
            }
        }

        [TestMethod]
        public void RepackIt()
        {
            //var path = @"C:\Users\enohka\Desktop\Teststuff\texture reversing\tsccombds_combineddstexture01.tgv";
            //var path2 = @"C:\Users\enohka\Desktop\Teststuff\texture reversing\tsccombds_combineddstexture01.dds";


            //var mgr = new TgvManager(File.ReadAllBytes(path));

            //var wr = new DDSWriter();


            //var buffer = wr.CreateDDSFile(mgr.CurrentFile);

            //using (var fs = File.Create(path2))
            //    fs.Write(buffer, 0, buffer.Length);


        }
    }
}
