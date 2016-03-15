using moddingSuite.Util;
using moddingSuite.ViewModel.Base;
using moddingSuite.ViewModel.Trad;

namespace moddingSuite.Model.Trad
{
    public class TradEntry : ViewModelBase
    {
        private uint _contLen;
        private string _content;
        private byte[] _hash;
        private string _hashView;
        private uint _offsetCont;
        private uint _offsetDic;

        private bool _userCreated = false;

        public string HashView
        {
            get { return _hashView; }
            protected set
            {
                _hashView = value;
                OnPropertyChanged(() => HashView);
            }
        }

        public byte[] Hash
        {
            get { return _hash; }
            set
            {
                _hash = value;

                HashView = Utils.ByteArrayToBigEndianHexByteString(_hash);

                OnPropertyChanged(() => Hash);
            }
        }

        public uint OffsetDic
        {
            get { return _offsetDic; }
            set
            {
                _offsetDic = value;
                OnPropertyChanged(() => OffsetDic);
            }
        }

        public uint OffsetCont
        {
            get { return _offsetCont; }
            set
            {
                _offsetCont = value;
                OnPropertyChanged(() => OffsetCont);
            }
        }

        public uint ContLen
        {
            get { return _contLen; }
            set
            {
                _contLen = value;
                OnPropertyChanged(() => ContLen);
            }
        }

        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;

                if (UserCreated)
                    TradFileViewModel.CalculateHash(this);

                OnPropertyChanged(() => Content);
            }
        }

        public bool UserCreated
        {
            get { return _userCreated; }
            set
            {
                _userCreated = value;
                OnPropertyChanged(() => UserCreated);
            }
        }
    }
}