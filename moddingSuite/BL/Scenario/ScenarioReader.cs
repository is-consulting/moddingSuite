using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using moddingSuite.BL.Ndf;
using moddingSuite.Model.Common;
using moddingSuite.Model.Scenario;
using moddingSuite.Util;
using moddingSuite.Model;
using moddingSuite.Model.Settings;


namespace moddingSuite.BL.Scenario
{
    public class ScenarioReader
    {
        public ScenarioFile Read(byte[] data)
        {
            using (var ms = new MemoryStream(data))
                return Read(ms);
        }

        public ScenarioFile Read(Stream s)
        {
            var f = new ScenarioFile();

            var buffer = new byte[10];
            s.Read(buffer, 0, buffer.Length);

            if (!Utils.ByteArrayCompare(Encoding.ASCII.GetBytes("SCENARIO\r\n"), buffer))
                throw new InvalidDataException("Wrong scenario header magic!");

            buffer = new byte[16];
            s.Read(buffer, 0, buffer.Length);
            f.Checksum = buffer;

            s.Seek(2, SeekOrigin.Current);

            buffer = new byte[4];
            s.Read(buffer, 0, buffer.Length);
            f.Version = BitConverter.ToInt32(buffer, 0);

            s.Read(buffer, 0, buffer.Length);
            int subFilesCount = BitConverter.ToInt32(buffer, 0);
            
            for (int i = 0; i < subFilesCount; i++)
            {
                f.lastPartStartByte=s.Position;
                s.Read(buffer, 0, buffer.Length);
                var contentFileBuffer = new byte[BitConverter.ToUInt32(buffer, 0)];
                s.Read(contentFileBuffer, 0, contentFileBuffer.Length);
                f.ContentFiles.Add(contentFileBuffer);
            }

            var reader = new NdfbinReader();
            f.NdfBinary = reader.Read(f.ContentFiles[1]);

            f.ZoneData = ReadZoneData(f.ContentFiles[0]);
            uncompressedPrintToFile(f.ContentFiles[2], "thirdPart");
            return f;
        }

        public AreaFile ReadZoneData(byte[] data)
        {
            var areaFile = new AreaFile();

            using (var ms = new MemoryStream(data))
            {
                var buffer = new byte[4];

                ms.AssertAreaMagic();

                ms.Read(buffer, 0, buffer.Length);
                int version = BitConverter.ToInt32(buffer, 0);
                if (version != 0)
                    throw new InvalidDataException("Not supported version of area format!");

                ms.Seek(4, SeekOrigin.Current);
                //ms.Read(buffer, 0, buffer.Length);
                //uint dataLen = BitConverter.ToUInt32(buffer, 0);

                ms.Read(buffer, 0, buffer.Length);
                int layerCount = BitConverter.ToInt32(buffer, 0);

                for (int lc = 0; lc < layerCount; lc++)
                {
                    var areaList = new AreaColletion();

                    ms.Read(buffer, 0, buffer.Length);
                    int areasToRead = BitConverter.ToInt32(buffer, 0);

                    for (int a = 0; a < areasToRead; a++)
                        areaList.Add(ReadArea(ms));

                    areaFile.AreaManagers.Add(areaList);
                }
            }

            return areaFile;
        }

        protected Area ReadArea(Stream ms)
        {
            var currentZone = new Area {Content = new AreaContent()};
            var buffer = new byte[4];

            ms.AssertAreaMagic();

            ms.Read(buffer, 0, buffer.Length);
            int zoneDataVersion = BitConverter.ToInt32(buffer, 0);
            if (zoneDataVersion != 2)
                throw new InvalidDataException("Zone data version != 2 not supported!");

            ms.Read(buffer, 0, buffer.Length);
            currentZone.Id = BitConverter.ToInt32(buffer, 0);

            ms.Read(buffer, 0, buffer.Length);
            int idStrLen = BitConverter.ToInt32(buffer, 0);
            var idStrBuffer = new byte[Utils.RoundToNextDivBy4(idStrLen)];
            ms.Read(idStrBuffer, 0, idStrBuffer.Length);
            currentZone.Name = Encoding.UTF8.GetString(idStrBuffer).TrimEnd('\0');

            ms.AssertAreaMagic();

            var attachmentPt = new Point3D();
            ms.Read(buffer, 0, buffer.Length);
            attachmentPt.X = BitConverter.ToSingle(buffer, 0);
            ms.Read(buffer, 0, buffer.Length);
            attachmentPt.Y = BitConverter.ToSingle(buffer, 0);
            ms.Read(buffer, 0, buffer.Length);
            attachmentPt.Z = BitConverter.ToSingle(buffer, 0);
            currentZone.AttachmentPoint = attachmentPt;

            ms.AssertAreaMagic();

            ms.Read(buffer, 0, buffer.Length);
            var subParts = BitConverter.ToInt32(buffer, 0);

            for (int sp = 0; sp < subParts; sp++)
            {
                var aced = new AreaClipped();
                ms.Read(buffer, 0, buffer.Length);
                aced.StartTriangle = BitConverter.ToInt32(buffer, 0);
                ms.Read(buffer, 0, buffer.Length);
                aced.TriangleCount = BitConverter.ToInt32(buffer, 0);
                ms.Read(buffer, 0, buffer.Length);
                aced.StartVertex = BitConverter.ToInt32(buffer, 0);
                ms.Read(buffer, 0, buffer.Length);
                aced.VertexCount = BitConverter.ToInt32(buffer, 0);
                currentZone.Content.ClippedAreas.Add(aced);
            }

            ms.AssertAreaMagic();

            ms.Read(buffer, 0, buffer.Length);
            currentZone.Content.BorderTriangle.StartTriangle = BitConverter.ToInt32(buffer, 0);
            ms.Read(buffer, 0, buffer.Length);
            currentZone.Content.BorderTriangle.TriangleCount = BitConverter.ToInt32(buffer, 0);
            ms.Read(buffer, 0, buffer.Length);
            currentZone.Content.BorderTriangle.StartVertex = BitConverter.ToInt32(buffer, 0);
            ms.Read(buffer, 0, buffer.Length);
            currentZone.Content.BorderTriangle.VertexCount = BitConverter.ToInt32(buffer, 0);

            ms.AssertAreaMagic();

            ms.Read(buffer, 0, buffer.Length);
            currentZone.Content.BorderVertex.StartVertex = BitConverter.ToInt32(buffer, 0);
            ms.Read(buffer, 0, buffer.Length);
            currentZone.Content.BorderVertex.VertexCount = BitConverter.ToInt32(buffer, 0);

            ms.AssertAreaMagic();

            ms.Read(buffer, 0, buffer.Length);
            int vertexCount = BitConverter.ToInt32(buffer, 0);

            ms.Read(buffer, 0, buffer.Length);
            int trianglesCount = BitConverter.ToInt32(buffer, 0);

            for (int v = 0; v < vertexCount; v++)
            {
                var curVertex = new AreaVertex();

                ms.Read(buffer, 0, buffer.Length);
                curVertex.X = BitConverter.ToSingle(buffer, 0);

                ms.Read(buffer, 0, buffer.Length);
                curVertex.Y = BitConverter.ToSingle(buffer, 0);

                ms.Read(buffer, 0, buffer.Length);
                curVertex.Z = BitConverter.ToSingle(buffer, 0);

                ms.Read(buffer, 0, buffer.Length);
                curVertex.W = BitConverter.ToSingle(buffer, 0);

                ms.Read(buffer, 0, buffer.Length);
                curVertex.Center = BitConverter.ToSingle(buffer, 0);

                currentZone.Content.Vertices.Add(curVertex);
            }

            ms.AssertAreaMagic();

            for (int f = 0; f < trianglesCount; f++)
            {
                var currentTriangle = new MeshTriangularFace();

                ms.Read(buffer, 0, buffer.Length);
                currentTriangle.Point1 = BitConverter.ToInt32(buffer, 0);

                ms.Read(buffer, 0, buffer.Length);
                currentTriangle.Point2 = BitConverter.ToInt32(buffer, 0);

                ms.Read(buffer, 0, buffer.Length);
                currentTriangle.Point3 = BitConverter.ToInt32(buffer, 0);

                currentZone.Content.Triangles.Add(currentTriangle);
            }

            ms.AssertAreaMagic();
            ms.Seek(4, SeekOrigin.Current);

            ms.AssertAreaMagic();

            ms.Read(buffer, 0, buffer.Length);
            if (BitConverter.ToUInt32(buffer, 0) != 809782853)
                throw new InvalidDataException("END0 expected!");

            return currentZone;
        }
        private void uncompressedPrintToFile(byte[] buffer, string name, StreamWriter logFile = null)
        {
            Settings settings = SettingsManager.Load();
            using (var fs = new FileStream(Path.Combine(settings.SavePath, name), FileMode.OpenOrCreate))
            {
                //var buffer = new byte[length];
                //var start = ms.Position;
                //ms.Read(buffer, 0, length);
                //var end = ms.Position;
                fs.Write(buffer, 0, buffer.Length);
                fs.Flush();
                //if (logFile != null) logFile.WriteLine("{0}: {1}/{2}/{3}", name, start, end, length);
            }
        }
    }
}
