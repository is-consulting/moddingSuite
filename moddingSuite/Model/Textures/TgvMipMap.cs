using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using moddingSuite.ViewModel.Base;

namespace moddingSuite.Model.Textures
{
    public class TgvMipMap : ViewModelBase 
    {
        private uint _offset;
        private uint _size;
        private int _mipWidth;
        private byte[] _content;

        public TgvMipMap()
        {
        }

        public TgvMipMap(uint offset, uint size, ushort mipWidth)
        {
            Offset = offset;
            Size = size;
            MipWidth = mipWidth;
        }

        public uint Offset
        {
            get { return _offset; }
            set { _offset = value; OnPropertyChanged(() => Offset); }
        }

        public uint Size
        {
            get { return _size; }
            set { _size = value; OnPropertyChanged(() => Size); }
        }

        public int MipWidth
        {
            get { return _mipWidth; }
            set { _mipWidth = value; OnPropertyChanged(() => MipWidth); }
        }

        public byte[] Content
        {
            get { return _content; }
            set { _content = value; OnPropertyChanged(() => Content);}
        }
    }
}
