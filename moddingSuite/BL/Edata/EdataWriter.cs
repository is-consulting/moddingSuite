using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using moddingSuite.Model.Edata;

namespace moddingSuite.BL.Edata
{
    public class EdataWriter
    {
        // RUSE, Wargame: EE
        public const int Version1 = 1;
        // Wargame: ALB, RD
        public const int Version2 = 2;

        public const uint DictionaryOffset = 1037;

        public void Write(EdataPackage package, Stream toStream, int version = 2)
        {
            if (version != Version1 && version != Version2)
                throw new NotSupportedException("Requested Edata version not supported");

            var initStreamPos = toStream.Position;

            toStream.Write(EdataPackage.EdataMagic, 0, EdataPackage.EdataMagic.Length);
            toStream.Write(BitConverter.GetBytes(version), 0, 4);

            // v1 checksum, keep space free if > v1
            toStream.Seek(16, SeekOrigin.Current);
            // Skip
            toStream.Seek(1, SeekOrigin.Current);

            // DictionaryOffset ASM seems to say it must be 1037: CMP DWORD PTR DS:[ESI+0x19],0x40D   (Compare value at offset 25 is 1037)
            toStream.Write(BitConverter.GetBytes(DictionaryOffset), 0, 4);

            // DictionaryLength, FileOffset, FileLength
            toStream.Seek(12, SeekOrigin.Current);

            // unkown, padding, will be kept as 0.
            toStream.Seek(8, SeekOrigin.Current);

            toStream.Seek(16, SeekOrigin.Current);

            NormalizeDirs(package.Root);
            package.Root = NormalizeFiles(package.Root);

            toStream.Seek(initStreamPos + DictionaryOffset, SeekOrigin.Begin);
            WriteFilesToStream(package, toStream);

            toStream.Seek(initStreamPos + DictionaryOffset, SeekOrigin.Begin);
            WriteDirToStream(package.Root, toStream);
        }

        public void WriteFilesToStream(EdataPackage pack, Stream s, int version = Version2)
        {
            int dirSize = GetSubTreeSize(pack.Root, Version2); // TODO verify

            s.Seek(dirSize, SeekOrigin.Current);

            WriteAllFilesInTree(pack.Root, pack, s, s.Position);
        }

        public void WriteAllFilesInTree(EdataDir node, EdataPackage pack, Stream s, long fileOffset)
        {
            foreach (var file in node.Files)
            {
                var data = pack.GetRawData(file, false);
                file.Offset = s.Position - fileOffset;
                file.Size = data.Length;
                s.Write(data, 0, data.Length);
            }

            foreach (var child in node.Children)
                WriteAllFilesInTree(child, pack, s, fileOffset);
        }

        public void WriteDirToStream(EdataDir dir, Stream s, int version = Version2)
        {
            foreach (var child in dir.Children)
            {
                s.Write(BitConverter.GetBytes(GetSubTreeSize(child)), 0, 4);
                s.Write(BitConverter.GetBytes(GetDirSize(child)), 0, 4);
                var nameData = Encoding.Unicode.GetBytes(dir.Name);
                s.Write(nameData, 0, nameData.Length);
                s.Write(Encoding.Unicode.GetBytes("\0"), 0, 1);
                if ((nameData.Length + 1) % 2 == 1)
                    s.Seek(1, SeekOrigin.Current);
            }

            foreach (var file in dir.Files)
            {
                s.Write(BitConverter.GetBytes(0), 0, 4);
                s.Write(BitConverter.GetBytes(GetFileSize(file)), 0, 4);
                s.Write(BitConverter.GetBytes(file.Offset), 0, 8);
                s.Write(BitConverter.GetBytes(file.Size), 0, 8);

                s.Write(file.Checksum, 0, 16);

                var nameData = Encoding.Unicode.GetBytes(file.Name);

                s.Write(nameData, 0, nameData.Length);

                if (nameData.Length % 2 == 1)
                    s.Seek(1, SeekOrigin.Current);
            }
        }

        public void NormalizeDirs(EdataDir dir)
        {
            if (dir.Parent != null && dir.Files.Count == 0 && dir.Children.Count == 1)
            {
                var child = dir.Children[0];
                dir.Name += child.Name;

                foreach (var edataDir in child.Children)
                {
                    dir.Children.Add(edataDir);
                    edataDir.Parent = dir;
                }

                foreach (var edataDir in dir.Children.Where(c => c != child))
                    child.Children.Remove(edataDir);

                foreach (var file in child.Files)
                {
                    dir.Files.Add(file);
                    file.Directory = dir;
                }

                foreach (var file in dir.Files)
                    child.Files.Remove(file);

                dir.Children.Remove(child);
            }

            if (dir.Parent != null && dir.Files.Count == 1 && dir.Children.Count == 0)
            {
                var file = dir.Files[0];

                file.Name = dir.Name + file.Name;
                file.Directory = dir.Parent;
                dir.Parent.Files.Add(file);

                dir.Parent.Children.Remove(dir);
                dir.Parent = null;
            }

            for (int c = 0; c < dir.Children.Count; c++)
                NormalizeDirs(dir.Children[c]);
        }

        public EdataDir NormalizeFiles(EdataDir dir, EdataDir recursionRoot = null)
        {
            if (recursionRoot == null)
                recursionRoot = dir;

            var matches = new Dictionary<string, int>();

            foreach (var child in dir.Children)
            {
                NormalizeFiles(child, recursionRoot);

                var tmp = new StringBuilder();
                foreach (var c in child.Name)
                {
                    tmp.Append(c);
                    var tmpStr = tmp.ToString();
                    if (matches.ContainsKey(tmpStr))
                        matches[tmpStr]++;
                    else
                        matches.Add(tmpStr, 1);
                }
            }

            foreach (var file in dir.Files)
            {
                var tmp = new StringBuilder();
                foreach (var c in file.Name)
                {
                    tmp.Append(c);
                    var tmpStr = tmp.ToString();
                    if (matches.ContainsKey(tmpStr))
                        matches[tmpStr]++;
                    else
                        matches.Add(tmpStr, 1);
                }
            }

            while (matches.Count > 0)
            {
                var max = matches.OrderByDescending(x => x.Value).ThenByDescending(x => x.Key.Length).FirstOrDefault();

                if (max.Equals(default(KeyValuePair<string, int>)) || max.Value == 1)
                    break;

                var newDir = new EdataDir(max.Key, dir);

                var dirsToClean = new List<EdataDir>();
                foreach (var subDir in dir.Children.Where(d => d.Name.StartsWith(max.Key)).Where(subDir => subDir != newDir))
                {
                    dirsToClean.Add(subDir);
                    subDir.Parent = newDir;
                    newDir.Children.Add(subDir);
                    subDir.Name = subDir.Name.TrimStart(max.Key.ToCharArray());
                }

                foreach (var subDir in dirsToClean.Where(subDir => dir.Children.Contains(subDir)))
                    dir.Children.Remove(subDir);

                var filesToClean = new List<EdataContentFile>();
                foreach (var file in dir.Files.Where(file => file.Name.StartsWith(max.Key)))
                {
                    filesToClean.Add(file);
                    file.Directory = newDir;
                    newDir.Files.Add(file);
                    file.Name = file.Name.TrimStart(max.Key.ToCharArray());
                }

                foreach (var file in filesToClean.Where(file => dir.Files.Contains(file)))
                    dir.Files.Remove(file);

                matches = matches.Where(x => !(x.Key.StartsWith(max.Key) || max.Key.StartsWith(x.Key))).ToDictionary(match => match.Key, match => match.Value);

                NormalizeFiles(newDir, recursionRoot);
            }

            return recursionRoot;
        }

        public int GetSubTreeSize(EdataDir dir, int version = Version2)
        {
            int size = 0;
            foreach (var child in dir.Children)
            {
                size += GetDirSize(child);
                size += GetSubTreeSize(dir);
            }

            size += dir.Files.Sum(file => GetFileSize(file));

            return size;
        }

        public int GetDirSize(EdataDir dir, int version = Version2)
        {
            int size = 0;
            // GroupSize
            size += 4;
            // Entry Size
            size += 4;
            // Zero terminated string
            size += dir.Name.Length + 1;

            if (size % 2 == 1)
                size++;

            return size;
        }

        public int GetFileSize(EdataContentFile file)
        {
            int size = 0;
            // GroupSize
            size += 4;
            // Entry Size
            size += 4;
            // Zero terminated string
            size += file.Name.Length + 1;

            if (size % 2 == 1)
                size++;

            size += 8;
            size += 8;
            size += 16;

            return size;
        }
    }
}
