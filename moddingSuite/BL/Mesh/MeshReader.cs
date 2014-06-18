using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using moddingSuite.BL.Ndf;
using moddingSuite.Model.Edata;
using moddingSuite.Model.Mesh;
using System.Runtime.InteropServices;
using moddingSuite.Util;

namespace moddingSuite.BL.Mesh
{
    public class MeshReader
    {
        public const uint MeshMagic = 1213416781; // "MESH"
        public const uint ProxyMagic = 1498960464; // "PRXY"

        public MeshFile Read(byte[] data)
        {
            using (var ms = new MemoryStream(data))
                return Read(ms);
        }

        public MeshFile Read(Stream s)
        {
            var file = new MeshFile();

            file.Header = ReadHeader(s);
            file.SubHeader = ReadSubHeader(s);

            file.MultiMaterialMeshFiles = ReadMeshDictionary(s, file);

            file.TextureBindings = ReadTextureBindings(s, file);

            return file;
        }

        private Model.Ndfbin.NdfBinary ReadTextureBindings(Stream s, MeshFile file)
        {
            var buffer = new byte[file.SubHeader.MeshMaterial.Size];
            
            s.Seek(file.SubHeader.MeshMaterial.Offset, SeekOrigin.Begin);
            s.Read(buffer, 0, buffer.Length);

            var ndfReader = new NdfbinReader();

            return ndfReader.Read(buffer);
        }

        protected MeshSubHeader ReadSubHeader(Stream ms)
        {
            var shead = new MeshSubHeader();

            var buffer = new byte[4];

            ms.Read(buffer, 0, buffer.Length);
            shead.MeshCount = BitConverter.ToUInt32(buffer, 0);

            shead.Dictionary = ReadSubHeaderEntryWithCount(ms);
            shead.VertexTypeNames = ReadSubHeaderEntryWithCount(ms);
            shead.MeshMaterial = ReadSubHeaderEntryWithCount(ms);

            shead.KeyedMeshSubPart = ReadSubHeaderEntryWithCount(ms);
            shead.KeyedMeshSubPartVectors = ReadSubHeaderEntryWithCount(ms);
            shead.MultiMaterialMeshes = ReadSubHeaderEntryWithCount(ms);
            shead.SingleMaterialMeshes = ReadSubHeaderEntryWithCount(ms);
            shead.Index1DBufferHeaders = ReadSubHeaderEntryWithCount(ms);
            shead.Index1DBufferStreams = ReadSubHeaderEntry(ms);
            shead.Vertex1DBufferHeaders = ReadSubHeaderEntryWithCount(ms);
            shead.Vertex1DBufferStreams = ReadSubHeaderEntry(ms);

            return shead;
        }

        protected MeshHeaderEntry ReadSubHeaderEntry(Stream s)
        {
            var entry = new MeshHeaderEntry();

            var buffer = new byte[4];

            s.Read(buffer, 0, buffer.Length);
            entry.Offset = BitConverter.ToUInt32(buffer, 0);

            s.Read(buffer, 0, buffer.Length);
            entry.Size = BitConverter.ToUInt32(buffer, 0);

            return entry;
        }

        protected MeshHeaderEntryWithCount ReadSubHeaderEntryWithCount(Stream s)
        {
            var entry = ReadSubHeaderEntry(s);

            var entryWithCount = new MeshHeaderEntryWithCount()
                {
                    Offset = entry.Offset,
                    Size = entry.Size
                };

            var buffer = new byte[4];
            s.Read(buffer, 0, buffer.Length);

            entryWithCount.Count = BitConverter.ToUInt32(buffer, 0);

            return entryWithCount;
        }

        protected MeshHeader ReadHeader(Stream ms)
        {
            var head = new MeshHeader();

            var buffer = new byte[4];

            ms.Read(buffer, 0, buffer.Length);

            if (BitConverter.ToUInt32(buffer, 0) != MeshMagic)
                throw new InvalidDataException("Wrong header magic");

            ms.Read(buffer, 0, buffer.Length);
            head.Platform = BitConverter.ToUInt32(buffer, 0);

            ms.Read(buffer, 0, buffer.Length);
            head.Version = BitConverter.ToUInt32(buffer, 0);

            ms.Read(buffer, 0, buffer.Length);
            head.FileSize = BitConverter.ToUInt32(buffer, 0);

            var chkSumBuffer = new byte[16];

            ms.Read(chkSumBuffer, 0, chkSumBuffer.Length);
            head.Checksum = chkSumBuffer;

            ms.Read(buffer, 0, buffer.Length);
            head.HeaderOffset = BitConverter.ToUInt32(buffer, 0);
            ms.Read(buffer, 0, buffer.Length);
            head.HeaderSize = BitConverter.ToUInt32(buffer, 0);

            ms.Read(buffer, 0, buffer.Length);
            head.ContentOffset = BitConverter.ToUInt32(buffer, 0);
            ms.Read(buffer, 0, buffer.Length);
            head.ContentSize = BitConverter.ToUInt32(buffer, 0);

            return head;
        }

        protected ObservableCollection<MeshContentFile> ReadMeshDictionary(Stream s, MeshFile f)
        {
            var files = new ObservableCollection<MeshContentFile>();
            var dirs = new List<EdataDir>();
            var endings = new List<long>();

            s.Seek(f.SubHeader.Dictionary.Offset, SeekOrigin.Begin);

            long dirEnd = f.SubHeader.Dictionary.Offset + f.SubHeader.Dictionary.Size;

            while (s.Position < dirEnd)
            {
                var buffer = new byte[4];
                s.Read(buffer, 0, 4);
                int fileGroupId = BitConverter.ToInt32(buffer, 0);

                if (fileGroupId == 0)
                {
                    var file = new MeshContentFile();
                    s.Read(buffer, 0, 4);
                    file.FileEntrySize = BitConverter.ToUInt32(buffer, 0);

                    var minp = new Point3D();
                    s.Read(buffer, 0, buffer.Length);
                    minp.X = BitConverter.ToSingle(buffer, 0);
                    s.Read(buffer, 0, buffer.Length);
                    minp.Y = BitConverter.ToSingle(buffer, 0);
                    s.Read(buffer, 0, buffer.Length);
                    minp.Z = BitConverter.ToSingle(buffer, 0);
                    file.MinBoundingBox = minp;

                    var maxp = new Point3D();
                    s.Read(buffer, 0, buffer.Length);
                    maxp.X = BitConverter.ToSingle(buffer, 0);
                    s.Read(buffer, 0, buffer.Length);
                    maxp.Y = BitConverter.ToSingle(buffer, 0);
                    s.Read(buffer, 0, buffer.Length);
                    maxp.Z = BitConverter.ToSingle(buffer, 0);
                    file.MaxBoundingBox = maxp;

                    s.Read(buffer, 0, buffer.Length);
                    file.Flags = BitConverter.ToUInt32(buffer, 0);

                    buffer = new byte[2];

                    s.Read(buffer, 0, buffer.Length);
                    file.MultiMaterialMeshIndex = BitConverter.ToUInt16(buffer, 0);

                    s.Read(buffer, 0, buffer.Length);
                    file.HierarchicalAseModelSkeletonIndex = BitConverter.ToUInt16(buffer, 0);

                    file.Name = Utils.ReadString(s);
                    file.Path = MergePath(dirs, file.Name);

                    if (file.Name.Length % 2 == 0)
                        s.Seek(1, SeekOrigin.Current);

                    files.Add(file);

                    while (endings.Count > 0 && s.Position == endings.Last())
                    {
                        dirs.Remove(dirs.Last());
                        endings.Remove(endings.Last());
                    }
                }
                else if (fileGroupId > 0)
                {
                    var dir = new EdataDir();

                    s.Read(buffer, 0, 4);
                    dir.FileEntrySize = BitConverter.ToInt32(buffer, 0);

                    if (dir.FileEntrySize != 0)
                        endings.Add(dir.FileEntrySize + s.Position - 8);
                    else if (endings.Count > 0)
                        endings.Add(endings.Last());

                    dir.Name = Utils.ReadString(s);

                    if (dir.Name.Length % 2 == 0)
                        s.Seek(1, SeekOrigin.Current);

                    dirs.Add(dir);
                }
            }

            return files;
        }

        protected string MergePath(IEnumerable<EdataDir> dirs, string fileName)
        {
            var b = new StringBuilder();

            foreach (var dir in dirs)
                b.Append(dir.Name);

            b.Append(fileName);

            return b.ToString();
        }
    }
}
