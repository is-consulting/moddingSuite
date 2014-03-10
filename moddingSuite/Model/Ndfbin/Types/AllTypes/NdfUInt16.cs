using System;
using moddingSuite.BL;
using moddingSuite.BL.Ndf;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfUInt16 : NdfFlatValueWrapper
    {
        public NdfUInt16(ushort value, long offset)
            : base(NdfType.UInt16, value, offset)
        {
        }

        public override byte[] GetBytes(out bool valid)
        {
            valid = true;

            try
            {
                return BitConverter.GetBytes(Convert.ToUInt16(Value));
            }
            catch (Exception)
            {
                valid = false;
                return new byte[0];
            }
        }

        public override byte[] GetNdfText()
        {
            return NdfTextWriter.NdfTextEncoding.GetBytes(Value.ToString());
        }
    }
}