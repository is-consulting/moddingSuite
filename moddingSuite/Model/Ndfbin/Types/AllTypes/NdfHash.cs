using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using moddingSuite.Util;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfHash : NdfFlatValueWrapper
    {
        public NdfHash(byte[] value, long offset)
            : base(NdfType.Hash, value, offset)
        {
            
        }

        public override byte[] GetBytes(out bool valid)
        {
            valid = true;

            return Value;
        }

        public new byte[] Value
        {
            get { return (byte[])base.Value; }
            set
            {
                base.Value = value;
                OnPropertyChanged(() => Value);
            }
        }

        public override byte[] GetNdfText()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Utils.ByteArrayToBigEndianHexByteString(Value);
        }
    }
}
