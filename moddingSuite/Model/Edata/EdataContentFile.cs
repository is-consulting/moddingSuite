using System.Globalization;
using moddingSuite.BL;

namespace moddingSuite.Model.Edata
{
    /// <summary>
    /// The reversed struct from Hex Workshop - much love to Hob_gadling for his hard work and help.
    /// The chunks after offset and fileSize are because of long (int64)
    /// 
    /// struct dictFileEntry {
    ///     DWORD groupId;
    ///     DWORD fileEntrySize;
    ///     DWORD offset;
    ///     DWORD chunk2;   
    ///     DWORD fileSize;
    ///     DWORD chunk4;
    ///     blob checksum[16];
    ///     zstring name;
    /// };
    /// </summary>
    public class EdataContentFile : EdataEntity
    {
        private byte[] _checkSum = new byte[16];
        private uint _id;
        private long _offset;
        private string _path;
        private long _size;
        private EdataDir _directory;
        private EdataFileType _fileType = EdataFileType.Unknown;

        public EdataContentFile(string name, EdataDir dir = null)
            : base(name)
        {
            Directory = dir;
            if (dir != null)
                dir.Files.Add(this);
        }

        public EdataContentFile()
            : base(string.Empty)
        {

        }

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                OnPropertyChanged("Path");
            }
        }

        public long Offset
        {
            get { return _offset; }
            set
            {
                _offset = value;
                OnPropertyChanged("Offset");
            }
        }

        public long Size
        {
            get { return _size; }
            set
            {
                _size = value;
                OnPropertyChanged("Size");
            }
        }

        public byte[] Checksum
        {
            get { return _checkSum; }
            set
            {
                _checkSum = value;
                OnPropertyChanged("Checksum");
            }
        }

        public uint Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("Name");
            }
        }

        public EdataFileType FileType
        {
            get { return _fileType; }
            set
            {
                _fileType = value;
                OnPropertyChanged("FileType");
            }
        }

        public EdataDir Directory
        {
            get { return _directory; }
            set
            {
                _directory = value;
                OnPropertyChanged("Directory");
            }
        }

        public override string ToString()
        {
            return Path.ToString(CultureInfo.CurrentCulture);
        }
    }
}