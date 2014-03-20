using System;
using moddingSuite.BL;
using moddingSuite.BL.Ndf;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfUInt16 : NdfFlatValueWrapper
    {
        public NdfUInt16(ushort value)
            : base(NdfType.UInt16, value)
        {
        }

        public override byte[] GetBytes()
        {
            return BitConverter.GetBytes(Convert.ToUInt16(Value));
        }

        public override byte[] GetNdfText()
        {
            return NdfTextWriter.NdfTextEncoding.GetBytes(Value.ToString());
        }
    }
}