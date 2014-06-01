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
                s.Read(buffer, 0, buffer.Length);
                var contentFileBuffer = new byte[BitConverter.ToUInt32(buffer, 0)];
                s.Read(contentFileBuffer, 0, contentFileBuffer.Length);
                f.ContentFiles.Add(contentFileBuffer);
            }

            var reader = new NdfbinReader();
            f.NdfBinary = reader.Read(f.ContentFiles[1]);

            f.ZoneData = ReadZoneData(f.ContentFiles[0]);

            return f;
        }

        public List<AreaData> ReadZoneData(byte[] data)
        {
            var areaList = new List<AreaData>();

            using (var ms = new MemoryStream(data))
            {
                var buffer = new byte[4];

                ms.AssertAreaMagic();

                ms.Seek(4, SeekOrigin.Current);

                ms.Read(buffer, 0, buffer.Length);
                uint restLen = BitConverter.ToUInt32(buffer, 0);
                long startPos = ms.Position;

                ms.Read(buffer, 0, buffer.Length);
                int areaVersion = BitConverter.ToInt32(buffer, 0); // TODO: assumtion

                ms.Seek(4, SeekOrigin.Current);

                while (ms.Position - startPos < restLen)
                {
                    ms.Read(buffer, 0, buffer.Length);
                    int areasToRead = BitConverter.ToInt32(buffer, 0);

                    for (int a = 0; a < areasToRead; a++)
                    {
                        var currentZone = new AreaData();

                        ms.AssertAreaMagic();

                        ms.Read(buffer, 0, buffer.Length);
                        int alwaysTwo = BitConverter.ToInt32(buffer, 0); // TODO: dont know what that is, always 2 and beginning of id block
                        ms.Seek(4, SeekOrigin.Current);

                        ms.Read(buffer, 0, buffer.Length);
                        int idStrLen = BitConverter.ToInt32(buffer, 0);
                        var idStrBuffer = new byte[Utils.RoundToNextDivBy4(idStrLen)];
                        ms.Read(idStrBuffer, 0, idStrBuffer.Length);
                        currentZone.Id = Encoding.UTF8.GetString(idStrBuffer);

                        ms.AssertAreaMagic();

                        var unkownVector = new Point3D(); // TODO: unknown
                        ms.Read(buffer, 0, buffer.Length);
                        unkownVector.X = BitConverter.ToSingle(buffer, 0);
                        ms.Read(buffer, 0, buffer.Length);
                        unkownVector.Y = BitConverter.ToSingle(buffer, 0);
                        ms.Read(buffer, 0, buffer.Length);
                        unkownVector.Z = BitConverter.ToSingle(buffer, 0);

                        ms.AssertAreaMagic();
                        ms.Seek(4 * 9, SeekOrigin.Current); // TODO: 9 unkown ints

                        ms.AssertAreaMagic();
                        ms.Seek(4 * 4, SeekOrigin.Current); // TODO: 4 unkown ints

                        ms.AssertAreaMagic();
                        ms.Seek(4 * 2, SeekOrigin.Current); // TODO: 2 unkown ints

                        ms.AssertAreaMagic();

                        ms.Read(buffer, 0, buffer.Length);
                        int vertexCount = BitConverter.ToInt32(buffer, 0);

                        ms.Read(buffer, 0, buffer.Length);
                        int facesCount = BitConverter.ToInt32(buffer, 0);

                        for (int v = 0; v < vertexCount; v++)
                        {
                            var currentVertex = new Point3D();

                            ms.Read(buffer, 0, buffer.Length);
                            currentVertex.Y = BitConverter.ToSingle(buffer, 0);

                            ms.Read(buffer, 0, buffer.Length);
                            currentVertex.X = BitConverter.ToSingle(buffer, 0);

                            // TODO unkown 8 bytes
                            ms.Seek(8, SeekOrigin.Current);

                            ms.Read(buffer, 0, buffer.Length);
                            currentVertex.Y = BitConverter.ToSingle(buffer, 0);

                            currentZone.Vertices.Add(currentVertex);
                        }

                        ms.AssertAreaMagic();

                        for (int f = 0; f < facesCount; f++)
                        {
                            var currentFace = new MeshTriangularFace();

                            ms.Read(buffer, 0, buffer.Length);
                            currentFace.Point1 = BitConverter.ToInt32(buffer, 0);

                            ms.Read(buffer, 0, buffer.Length);
                            currentFace.Point2 = BitConverter.ToInt32(buffer, 0);

                            ms.Read(buffer, 0, buffer.Length);
                            currentFace.Point3 = BitConverter.ToInt32(buffer, 0);

                            currentZone.Faces.Add(currentFace);
                        }

                        ms.AssertAreaMagic();
                        ms.Seek(4, SeekOrigin.Current);

                        ms.AssertAreaMagic();

                        ms.Read(buffer, 0, buffer.Length);
                        if (BitConverter.ToUInt32(buffer, 0) != 809782853)
                            throw  new InvalidDataException("END0 expected!");

                        areaList.Add(currentZone);
                    }
                }
            }

            return areaList;
        }
    }
}
