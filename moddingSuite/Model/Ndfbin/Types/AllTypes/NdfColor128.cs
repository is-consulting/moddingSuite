using System;
using moddingSuite.Util;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfColor128 : NdfFlatValueWrapper
    {
        public NdfColor128(byte[] value, long offset) : base(NdfType.Color128, value, offset)
        {
        }

        public new byte[] Value
        {
            get { return (byte[]) base.Value; }
            set
            {
                base.Value = value;
                OnPropertyChanged(() => Value);
            }
        }

        public override byte[] GetBytes(out bool valid)
        {
            valid = true;

            return Value;
        }

        public override string ToString()
        {
            return string.Format("Vec4: {0}", Utils.ByteArrayToBigEndianHexByteString(Value));
        }


        public override byte[] GetNdfText()
        {
            throw new NotImplementedException();
        }
    }
}