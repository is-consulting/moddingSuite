using System;
using moddingSuite.Util;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfLocalisationHash : NdfFlatValueWrapper
    {
        public NdfLocalisationHash(byte[] value, long offset)
            : base(NdfType.LocalisationHash, value, offset)
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
            return Utils.ByteArrayToBigEndianHeyByteString(Value);
        }

        public override byte[] GetNdfText()
        {
            throw new NotImplementedException();
        }
    }
}