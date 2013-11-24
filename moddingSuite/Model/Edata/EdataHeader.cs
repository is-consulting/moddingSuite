using moddingSuite.BL;

namespace moddingSuite.Model.Edata
{
    /// <summary>
    /// Thanks to Wargame:EE DAT Unpacker by Giovanni Condello
    /// struct edataHeader
    /// {
    ///	    CHAR edat[4];
    ///	    blob junk[21];
    ///	    DWORD dirOffset;
    ///	    DWORD dirLength;
    ///	    DWORD fileOffset;
    ///	    DWORD fileLength;
    /// };
    /// </summary>
    public class EdataHeader : EdataEntity
    {
        public EdataHeader(EdataManager mgr) : base(mgr)
        {

        }

        public int Version { get; set; }
        public byte[] Checksum { get; set; }
        public int DirOffset { get; set; }
        public int DirLengh { get; set; }
        public int FileOffset { get; set; }
        public int FileLengh { get; set; }
        
    }
}