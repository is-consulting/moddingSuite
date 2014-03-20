using System;
using moddingSuite.BL;
using moddingSuite.BL.Ndf;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfUInt32 : NdfFlatValueWrapper
    {
        public NdfUInt32(uint value)
            : base(NdfType.UInt32, value)
        {
        }

        public override byte[] GetBytes()
        {
            return BitConverter.GetBytes(Convert.ToUInt32(Value));
        }

        public override byte[] GetNdfText()
        {
            return NdfTextWriter.NdfTextEncoding.GetBytes(Value.ToString());
        }
    }
}