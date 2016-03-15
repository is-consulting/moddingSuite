using moddingSuite.BL.TGV;
using moddingSuite.Model.Textures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tgvExporter
{
    class Program
    {
        static void Main(string[] args)
        {
            //var inpath = @"C:\Users\Anders\wargameexport\port_Wonsan";
            //var inFile = Path.Combine(inpath, "lowdef.tmst_chunk_pc");
            if (args == null || args.Length == 0) return;
            var inFile = args[0];
            var inpath = Directory.GetCurrentDirectory();
            var tgvReader = new TgvReader();

            var inFileInfo = new FileInfo(inFile);

            TgvFile tgv;

            using (var fs = new FileStream(inFile, FileMode.Open))
            {
                var writer = new TgvDDSWriter();

                int index = 1;

                const uint fatMagic = 810828102;
                Console.WriteLine("start");
                
                while (fs.Position+4 < fs.Length)
                {
                    fs.Seek(4, SeekOrigin.Current);

                    var buffer = new byte[4];
                    fs.Read(buffer, 0, buffer.Length);

                    if (BitConverter.ToUInt32(buffer, 0) != fatMagic)
                        break;

                    // Always 1
                    fs.Read(buffer, 0, buffer.Length);
                    uint int1 = BitConverter.ToUInt32(buffer, 0);

                    // Always 16
                    fs.Read(buffer, 0, buffer.Length);
                    uint int2 = BitConverter.ToUInt32(buffer, 0);

                    //Console.WriteLine("{0} - {1}", int1, int2);

                    //fs.Seek(8, SeekOrigin.Current);

                    fs.Read(buffer, 0, buffer.Length);
                    var blockSize = BitConverter.ToUInt32(buffer, 0);

                    if (fs.Position >= fs.Length) continue;

                    var tileBuffer = new byte[blockSize];

                    fs.Read(tileBuffer, 0, tileBuffer.Length);

                    tgv = tgvReader.Read(tileBuffer);
                    Console.WriteLine(index);
                    byte[] content = writer.CreateDDSFile(tgv);

                    var f = new FileInfo(inFile);
                    
                    var path = Path.Combine(inpath, string.Format("{0}_{1}", f.Name, "export"));

                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    //if (index%21==3)
                    using (var outFs = new FileStream(Path.Combine(path, string.Format("{0}.dds", index)), FileMode.OpenOrCreate))
                    {
                        outFs.Write(content, 0, content.Length);
                        outFs.Flush();
                    }

                    index++;
                }
            }
        }
    }
}
