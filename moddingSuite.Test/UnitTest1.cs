using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using moddingSuite.BL;
using moddingSuite.BL.Compressing;
using moddingSuite.BL.DDS;
using moddingSuite.BL.Mesh;
using moddingSuite.BL.TGV;
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
            var inFile = Path.Combine(inpath, "lowdef.tmst_chunk_pc");

            var tgvReader = new TgvReader();

            var inFileInfo = new FileInfo(inFile);

            TgvFile tgv;

            using (var fs = new FileStream(inFile, FileMode.Open))
            {
                var writer = new TgvDDSWriter();

                int index = 1;

                while (fs.Position < fs.Length)
                {
                    var sepBuffer = new byte[4];
                    fs.Read(sepBuffer, 0, sepBuffer.Length);

                    if (fs.Position >= fs.Length) continue;

                    tgv = tgvReader.Read(fs);

                    byte[] content = writer.CreateDDSFile(tgv);

                    var f = new FileInfo(inFile);

                    var path = Path.Combine(inpath, string.Format("{0}_{1}", f.Name, "export"));

                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    using (var outFs = new FileStream(Path.Combine(path, string.Format("{0}.dds", index)), FileMode.OpenOrCreate))
                    {
                        outFs.Write(content, 0, content.Length);
                        outFs.Flush();
                    }

                    index++;
                }
            }

        }

        [TestMethod]
        public void TestMeshReader()
        {
            var file = Path.Combine(@"C:\Users\enohka\Desktop\teststuff", "mesh_all.spk");

            var mreader = new MeshReader();

            using (var fs = new FileStream(file, FileMode.Open))
                mreader.Read(fs);

        }

    }
}
