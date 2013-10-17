using System;
using moddingSuite.Util;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfUnkown : NdfFlatValueWrapper
    {
        public NdfUnkown(byte[] value, long offset)
            : base(NdfType.Unknown, value, offset)
        {
        }

        public override byte[] GetBytes(out bool valid)
        {
            valid = false;

            return new byte[0];
        }

        public override byte[] GetNdfText()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Format("{0} : {1}", OffSet, Utils.ByteArrayToBigEndianHeyByteString((byte[]) Value));
        }
    }
}