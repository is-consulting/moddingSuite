using System;
using moddingSuite.BL;
using moddingSuite.BL.Ndf;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfInt32 : NdfFlatValueWrapper
    {
        public NdfInt32(int value)
            : base(NdfType.Int32, value)
        {
        }

        public override byte[] GetBytes()
        {
            return BitConverter.GetBytes(Convert.ToInt32(Value));
        }

        public override byte[] GetNdfText()
        {
            return NdfTextWriter.NdfTextEncoding.GetBytes(Value.ToString());
        }
    }
}