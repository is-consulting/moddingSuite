using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using moddingSuite.BL.Ndf;
using moddingSuite.Model.Scenario;
using moddingSuite.Util;

namespace moddingSuite.BL.Scenario
{
    public class ScenarioWriter
    {
        public const int Version = 4;
        public const int AreaFileVersion = 0;
        public const int AreaPartVersion = 2;

        public byte[] Write(ScenarioFile file)
        {
            var scenarioData = new List<byte>();

            scenarioData.AddRange(Encoding.ASCII.GetBytes("SCENARIO\r\n"));
            scenarioData.AddRange(BitConverter.GetBytes(Version));
            scenarioData.AddRange(BitConverter.GetBytes(file.ContentFiles.Count));

            file.ContentFiles[0] = CreateAreaSubFile(file.ZoneData);

            var ndfBinWriter = new NdfbinWriter();

            file.ContentFiles[1] = ndfBinWriter.Write(file.NdfBinary, false); // something wrong here; enohka: Should be fixed by now?


            foreach (var contentFile in file.ContentFiles)
            {
                int padding = Utils.RoundToNextDivBy4(contentFile.Length) - contentFile.Length;

                scenarioData.AddRange(BitConverter.GetBytes(contentFile.Length + padding));
                scenarioData.AddRange(contentFile);

                var paddingLst = new List<byte>();

                for(int p = 0; p < padding; p++)
                    paddingLst.Add(0x0);

                scenarioData.AddRange(paddingLst);
            }

            byte[] hash = MD5.Create().ComputeHash(scenarioData.ToArray());

            scenarioData.InsertRange(10, hash.Concat(new byte[] { 0x00, 0x00 }));

            return scenarioData.ToArray();
        }

        protected byte[] CreateAreaSubFile(AreaFile file)
        {
            using (var ms = new MemoryStream())
            {
                ms.WriteAreaMagic();
                ms.Write(BitConverter.GetBytes(AreaFileVersion), 0, 4);

                // Seek size part, needs to be written as we know.
                ms.Seek(4, SeekOrigin.Current);

                ms.Write(BitConverter.GetBytes(file.AreaManagers.Count), 0, 4);

                foreach (var areaManager in file.AreaManagers)
                {
                    ms.Write(BitConverter.GetBytes(areaManager.Count), 0, 4);

                    foreach (var area in areaManager)
                    {
                        ms.WriteAreaMagic();
                        ms.Write(BitConverter.GetBytes(AreaPartVersion), 0, 4);
                        ms.Write(BitConverter.GetBytes(area.Id), 0, 4);
                        ms.Write(BitConverter.GetBytes(area.Name.Length), 0, 4);
                        var nameBuffer = Encoding.UTF8.GetBytes(area.Name);
                        ms.Write(nameBuffer, 0, nameBuffer.Length);
                        ms.Seek(Util.Utils.RoundToNextDivBy4(nameBuffer.Length) - nameBuffer.Length, SeekOrigin.Current);

                        ms.WriteAreaMagic();
                        ms.Write(BitConverter.GetBytes((float)area.AttachmentPoint.X), 0, 4);
                        ms.Write(BitConverter.GetBytes((float)area.AttachmentPoint.Y), 0, 4);
                        ms.Write(BitConverter.GetBytes((float)area.AttachmentPoint.Z), 0, 4);

                        ms.WriteAreaMagic();
                        ms.Write(BitConverter.GetBytes(area.Content.ClippedAreas.Count),0,4);
                        foreach (var clippedArea in area.Content.ClippedAreas)
                        {
                            ms.Write(BitConverter.GetBytes(clippedArea.StartTriangle), 0, 4);
                            ms.Write(BitConverter.GetBytes(clippedArea.TriangleCount), 0, 4);
                            ms.Write(BitConverter.GetBytes(clippedArea.StartVertex), 0, 4);
                            ms.Write(BitConverter.GetBytes(clippedArea.VertexCount), 0, 4);
                        }

                        ms.WriteAreaMagic();
                        ms.Write(BitConverter.GetBytes(area.Content.BorderTriangle.StartTriangle), 0, 4);
                        ms.Write(BitConverter.GetBytes(area.Content.BorderTriangle.TriangleCount), 0, 4);
                        ms.Write(BitConverter.GetBytes(area.Content.BorderTriangle.StartVertex), 0, 4);
                        ms.Write(BitConverter.GetBytes(area.Content.BorderTriangle.VertexCount), 0, 4);

                        ms.WriteAreaMagic();
                        ms.Write(BitConverter.GetBytes(area.Content.BorderVertex.StartVertex), 0, 4);
                        ms.Write(BitConverter.GetBytes(area.Content.BorderVertex.VertexCount), 0, 4);

                        ms.WriteAreaMagic();
                        ms.Write(BitConverter.GetBytes(area.Content.Vertices.Count), 0, 4);
                        ms.Write(BitConverter.GetBytes(area.Content.Triangles.Count), 0, 4);
                        foreach (var vertex in area.Content.Vertices)
                        {
                            ms.Write(BitConverter.GetBytes(vertex.X), 0, 4);
                            ms.Write(BitConverter.GetBytes(vertex.Y), 0, 4);
                            ms.Write(BitConverter.GetBytes(vertex.Z), 0, 4);
                            ms.Write(BitConverter.GetBytes(vertex.W), 0, 4);
                            ms.Write(BitConverter.GetBytes(vertex.Center), 0, 4);
                        }

                        ms.WriteAreaMagic();
                        foreach (var triangle in area.Content.Triangles)
                        {
                            ms.Write(BitConverter.GetBytes(triangle.Point1), 0, 4);
                            ms.Write(BitConverter.GetBytes(triangle.Point2), 0, 4);
                            ms.Write(BitConverter.GetBytes(triangle.Point3), 0, 4);
                        }

                        ms.WriteAreaMagic();
                        ms.Seek(4, SeekOrigin.Current);
                        ms.WriteAreaMagic();

                        ms.Write(BitConverter.GetBytes(809782853), 0, 4);
                    }
                }

                ms.Seek(8, SeekOrigin.Begin);
                ms.Write(BitConverter.GetBytes((uint)(ms.Length-12)),0,4);

                return ms.ToArray();
            }
        }
    }
}