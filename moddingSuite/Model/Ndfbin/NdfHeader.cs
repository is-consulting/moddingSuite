using moddingSuite.ViewModel.Base;
using System;

namespace moddingSuite.Model.Ndfbin
{
    /// <summary>
    ///struct cndfHeader {
    /// 
    /// blob headerMagic[12];       // EUG0 00 00 00 00 CNDF
    /// DWORD IsCompressed;  		// 128 == true; 0 == false;
    /// QWORD Footeroffset;  		// With full header
    /// QWORD HeaderSize;    		// value == 40
    /// QWORD FullFileSizeUncomp;	// full filesize with header (When Compressed its the uncompressed size)
    ///
    ///};
    /// </summary>
    public class NdfHeader : ViewModelBase
    {
        private bool _isCompressedBody;

        public bool IsCompressedBody
        {
            get { return _isCompressedBody; }
            set
            {
                _isCompressedBody = value;
                OnPropertyChanged(() => IsCompressedBody);
            }
        }
        public ulong FooterOffset { get; set; }
        public ulong HeaderSize { get; set; }
        public ulong FullFileSizeUncomp { get; set; }
    }
}