using System;
using System.Globalization;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using moddingSuite.BL;
using moddingSuite.BL.Compressing;
using moddingSuite.BL.DDS;
using moddingSuite.BL.Mesh;
using moddingSuite.BL.TGV;
using moddingSuite.Model.Textures;
using moddingSuite.Util;

namespace moddingSuite.Test
{
    [TestClass]
    public class UnitTest1: BaseTests
    {
        
        [TestMethod]
        public void TestMethod1()
        {
            SavManager mgr = new SavManager();

            using (var fs = new FileStream($@"{AirLandUserDataPath}\remote\profile.wargame", FileMode.Open))
            {
                using (var ms = new MemoryStream())
                {
                    fs.CopyTo(ms);

                    var save = mgr.Read(ms.ToArray());
                    save.Checksum.Should().NotBeEmpty();
                }
            }
        }

        //[TestMethod]
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

        //[TestMethod]
        public void ExportTmsTest()
        {
            var inpath = @"C:\Users\Anders\wargameexport\r2";
            var inFile = Path.Combine(inpath, "lowdef.tmst_chunk_pc");

            var tgvReader = new TgvReader();

            var inFileInfo = new FileInfo(inFile);

            TgvFile tgv;

            using (var fs = new FileStream(inFile, FileMode.Open))
            {
                var writer = new TgvDDSWriter();

                int index = 1;

                const uint fatMagic = 810828102;
                Console.WriteLine("start");
                while (fs.Position < fs.Length)
                {
                    fs.Seek(4, SeekOrigin.Current);

                    var buffer = new byte[4];
                    fs.Read(buffer, 0, buffer.Length);

                    if (BitConverter.ToUInt32(buffer, 0) != fatMagic)
                        throw new InvalidDataException();
                    Console.WriteLine("passe");
                    fs.Seek(8, SeekOrigin.Current);

                    fs.Read(buffer, 0, buffer.Length);
                    var blockSize = BitConverter.ToUInt32(buffer, 0);

                    if (fs.Position >= fs.Length) continue;

                    var tileBuffer = new byte[blockSize];

                    fs.Read(tileBuffer, 0, tileBuffer.Length);

                    tgv = tgvReader.Read(tileBuffer);

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

        //[TestMethod]
        public void TestMeshReader()
        {
            var file = Path.Combine(@"C:\Users\enohka\Desktop\teststuff", "mesh_all.spk");

            var mreader = new MeshReader();

            using (var fs = new FileStream(file, FileMode.Open))
                mreader.Read(fs);
        }

        //[TestMethod]
        public void ExportAreaVerteces()
        {
            string input = Path.Combine(@"C:\Users\enohka\Desktop\teststuff\scen", "zone_test");
            string output = Path.Combine(@"C:\Users\enohka\Desktop\teststuff\scen", "zone_test.csv");

            var enc = Encoding.Unicode;
            var sep = enc.GetBytes(";");
            var nl = enc.GetBytes("\r\n");

            const int areaMagic = 1095062081;

            using (var i = File.OpenRead(input))
            {
                var buffer = new byte[4];
                i.Read(buffer, 0, buffer.Length);

                if (BitConverter.ToInt32(buffer, 0) != areaMagic)
                    throw new InvalidDataException();

                i.Read(buffer, 0, buffer.Length);
                int vertexCount = BitConverter.ToInt32(buffer, 0);

                i.Read(buffer, 0, buffer.Length);
                int facesCount = BitConverter.ToInt32(buffer, 0);

                using (var o = File.OpenWrite(output))
                {
                    for (int v = 0; v < vertexCount; v++)
                    {
                        i.Read(buffer, 0, buffer.Length);
                        var x = enc.GetBytes(BitConverter.ToSingle(buffer, 0).ToString());

                        i.Read(buffer, 0, buffer.Length);
                        var y = enc.GetBytes(BitConverter.ToSingle(buffer, 0).ToString());

                        i.Seek(8, SeekOrigin.Current);

                        i.Read(buffer, 0, buffer.Length);
                        var z = enc.GetBytes(BitConverter.ToSingle(buffer, 0).ToString());

                        o.Write(y, 0, y.Length);
                        o.Write(sep, 0, sep.Length);
                        o.Write(x, 0, x.Length);
                        o.Write(sep, 0, sep.Length);
                        o.Write(z,0,z.Length);
                        o.Write(nl, 0, nl.Length);
                    }

                    i.Read(buffer, 0, buffer.Length);
                    if (BitConverter.ToInt32(buffer, 0) != areaMagic)
                        throw new InvalidDataException();

                    for (int f = 0; f < facesCount; f++)
                    {
                        i.Read(buffer, 0, buffer.Length);
                        var f1 = enc.GetBytes(BitConverter.ToInt32(buffer, 0).ToString());

                        i.Read(buffer, 0, buffer.Length);
                        var f2 = enc.GetBytes(BitConverter.ToInt32(buffer, 0).ToString());

                        i.Read(buffer, 0, buffer.Length);
                        var f3 = enc.GetBytes(BitConverter.ToInt32(buffer, 0).ToString());

                        o.Write(f1, 0, f1.Length);
                        o.Write(sep, 0, sep.Length);
                        o.Write(f2, 0, f2.Length);
                        o.Write(sep, 0, sep.Length);
                        o.Write(f3, 0, f3.Length);
                        o.Write(nl, 0, nl.Length);
                    }
                }
            }
        }
    }
}
