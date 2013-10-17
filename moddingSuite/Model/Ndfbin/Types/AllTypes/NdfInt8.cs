using System;
using moddingSuite.BL;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfInt8 : NdfFlatValueWrapper
    {
        public NdfInt8(byte value, long offset)
            : base(NdfType.Int8, value, offset)
        {
        }

        public override byte[] GetBytes(out bool valid)
        {
            valid = true;

            try
            {
                return new byte[1] {(byte) Value};
            }
            catch (Exception e)
            {
                valid = false;
                return new byte[0];
            }
        }

        public override byte[] GetNdfText()
        {
            return NdfbinManager.NdfTextEncoding.GetBytes(Value.ToString());
        }
    }
}