using System.Collections.Generic;
using System.Windows.Media;
using moddingSuite.BL.DDS;
using moddingSuite.ViewModel.Base;
using PixelFormats = moddingSuite.BL.DDS.PixelFormats;

namespace moddingSuite.Model.Textures
{
    public class TgvFile : ViewModelBase
    {
        private PixelFormats _format;
        private uint _height;
        private uint _imageHeight;
        private uint _imageWidth;
        private bool _isCompressed;

        private ushort _mipMapCount;

        private List<uint> _offsets = new List<uint>();
        private string _pixelFormatStr;
        private List<uint> _sizes = new List<uint>();
        private byte[] _sourceChecksum;
        private uint _version;
        private uint _width;

        private readonly List<TgvMipMap> _mipMaps = new List<TgvMipMap>();

        public uint Version
        {
            get { return _version; }
            set
            {
                _version = value;
                OnPropertyChanged("Version");
            }
        }

        public bool IsCompressed
        {
            get { return _isCompressed; }
            set
            {
                _isCompressed = value;
                OnPropertyChanged("IsCompressed");
            }
        }

        public uint Width
        {
            get { return _width; }
            set
            {
                _width = value;
                OnPropertyChanged("Width");
            }
        }

        public uint Height
        {
            get { return _height; }
            set
            {
                _height = value;
                OnPropertyChanged("Height");
            }
        }

        public uint ImageWidth
        {
            get { return _imageWidth; }
            set
            {
                _imageWidth = value;
                OnPropertyChanged("ImageWidth");
            }
        }

        public uint ImageHeight
        {
            get { return _imageHeight; }
            set
            {
                _imageHeight = value;
                OnPropertyChanged("ImageHeight");
            }
        }

        public ushort MipMapCount
        {
            get { return _mipMapCount; }
            set
            {
                _mipMapCount = value;
                OnPropertyChanged("MipMapCount");
            }
        }

        public PixelFormats Format
        {
            get { return _format; }
            set
            {
                _format = value;
                OnPropertyChanged("Format");
            }
        }

        public byte[] SourceChecksum
        {
            get { return _sourceChecksum; }
            set
            {
                _sourceChecksum = value;
                OnPropertyChanged("SourceChecksum");
            }
        }

        public List<uint> Offsets
        {
            get { return _offsets; }
            set
            {
                _offsets = value;
                OnPropertyChanged("Offsets");
            }
        }

        public List<uint> Sizes
        {
            get { return _sizes; }
            set
            {
                _sizes = value;
                OnPropertyChanged("Sizes");
            }
        }

        public string PixelFormatStr
        {
            get { return _pixelFormatStr; }
            set
            {
                _pixelFormatStr = value;
                OnPropertyChanged("PixelFormatStr");
            }
        }

        public List<TgvMipMap> MipMaps
        {
            get { return _mipMaps; }
        }
    }
}