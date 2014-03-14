using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.Model.Mesh
{
    /// <summary>
    /// typedef struct meshFileHeader_s
    /// {
    ///    DWORD magic;
    ///    DWORD platform;
    ///    DWORD version;
    ///    DWORD fileSize;
    ///    blob checksum[16];
    ///    DWORD headerOffset;
    ///    DWORD headerSize;
    ///    DWORD dataOffset;
    ///    DWORD dataSize;
    /// } meshFileHeader;
    /// </summary>
    public class MeshHeader
    {
        public uint Platform { get; set; }
        public uint Version { get; set; }
        public uint FileSize { get; set; }
        public byte[] Checksum { get; set; }
        public uint HeaderOffset { get; set; }
        public uint HeaderSize { get; set; }
        public uint ContentOffset { get; set; }
        public uint ContentSize { get; set; }
    }
}
