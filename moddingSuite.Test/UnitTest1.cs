using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using moddingSuite.BL;
using moddingSuite.BL.DDS;
using moddingSuite.BL.TGV;
using moddingSuite.Compressing;
using moddingSuite.Model.Textures;

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
            var inpath = @"C:\Users\enohka\Desktop\teststuff\leopard2\blob";
            var outpath = @"C:\Users\enohka\Desktop\teststuff\leopard2\blob.uncmp";

            var inFi = new FileInfo(inpath);

            byte[] inData = new byte[inFi.Length];

            using (var fs = new FileStream(inpath, FileMode.Open))
                fs.Read(inData, 0, inData.Length);

            var outData = Compressor.Decomp(inData);

            using (var fs = File.Exists(outpath) ? new FileStream(outpath, FileMode.Truncate) : File.Create(outpath))
                fs.Write(outData, 0, outData.Length);
        }

        [TestMethod]
        public void ExportTmsTest()
        {
            var inpath = @"C:\Users\enohka\Desktop\teststuff\leopard2\";
            var inFile = Path.Combine(inpath, "highdef.tmst_chunk_pc");

            var tgvReader = new TgvReader();

            var inFileInfo = new FileInfo(inFile);
            var inFileBuffer = new byte[inFileInfo.Length];

            TgvFile tgv; 


            using (var fs = new FileStream(inFile, FileMode.Open))
            {
                fs.Read(inFileBuffer, 0, inFileBuffer.Length);
                tgv = tgvReader.Read(inFileBuffer);
            }

            var writer = new TgvDDSWriter();

            byte[] content = writer.CreateDDSFile(tgv);

            var f = new FileInfo(inFile);

            using (var fs = new FileStream(Path.Combine(inpath, f.Name + ".dds"), FileMode.OpenOrCreate))
            {
                fs.Write(content, 0, content.Length);
                fs.Flush();
            }
        }
    }
}
