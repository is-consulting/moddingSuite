using System;
using moddingSuite.BL;
using moddingSuite.BL.Ndf;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfBoolean : NdfFlatValueWrapper
    {
        public NdfBoolean(bool value)
            : base(NdfType.Boolean, value)
        {
        }

        public override byte[] GetBytes()
        {
            return BitConverter.GetBytes(Convert.ToBoolean(Value));
        }

        public override byte[] GetNdfText()
        {
            return NdfTextWriter.NdfTextEncoding.GetBytes(Value.ToString());
        }
    }
}