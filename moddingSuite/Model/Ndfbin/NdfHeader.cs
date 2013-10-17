using moddingSuite.ViewModel.Base;

namespace moddingSuite.Model.Ndfbin
{
    /// <summary>
    /// struct cndfHeader {
    ///	char header[12];
    ///	DWORD always128;
    ///	DWORD blockSizef;
    ///	DWORD chunk1;
    ///	DWORD len4;
    ///	DWORD chunk2;
    ///	DWORD blockSizePlusE0;
    ///	DWORD chunk3;
    /// only if compressed
    ///	DWORD blockSizePlusE0MinusLen4;
    ///};
    /// </summary>
    public class NdfHeader : ViewModelBase
    {
        private int _blockSize;
        private int _blockSizeE0;

        // Only if compressed = true;

        private int _blockSizeWithoutHeader;
        private bool _isCompressedBody;

        public int FileSizeUncompressed
        {
            get { return _blockSize; }
            set
            {
                _blockSize = value;
                OnPropertyChanged(() => FileSizeUncompressed);
            }
        }

        public int FileSizeUncompressedMinusE0
        {
            get { return _blockSizeE0; }
            set
            {
                _blockSizeE0 = value;
                OnPropertyChanged(() => FileSizeUncompressedMinusE0);
            }
        }

        public int UncompressedContentSize
        {
            get { return _blockSizeWithoutHeader; }
            set
            {
                _blockSizeWithoutHeader = value;
                OnPropertyChanged(() => UncompressedContentSize);
            }
        }

        public bool IsCompressedBody
        {
            get { return _isCompressedBody; }
            set
            {
                _isCompressedBody = value;
                OnPropertyChanged(() => IsCompressedBody);
            }
        }
    }
}