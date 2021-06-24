using moddingSuite.BL.Utils;
using moddingSuite.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        private readonly static HashSet<char> s_allowedChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"
            .ToCharArray()
            .ToHashSet();

        public string HashView
        {
            get { return _hashView; }
            set
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

                HashView = StdUtils.ByteArrayToBigEndianHexByteString(_hash);

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
                    CalculateHash(this);

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

        public static void CalculateHash(TradEntry item)
        {
            var wordToHash = new StringBuilder();

            foreach (char t in item.Content)
                if (s_allowedChars.Contains(t))
                    wordToHash.Append(t);

            var word = wordToHash.ToString();

            item.Hash = StdUtils.CreateLocalisationHash(word, word.Length);
        }
    }
}