using System;
using moddingSuite.BL;
using moddingSuite.BL.Ndf;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfTrans : NdfFlatValueWrapper
    {
        public NdfTrans(NdfTranReference value)
            : base(NdfType.TransTableReference, value)
        {
        }

        public override byte[] GetBytes()
        {
            return BitConverter.GetBytes(((NdfTranReference)Value).Id);
        }


        public override byte[] GetNdfText()
        {
            return NdfTextWriter.NdfTextEncoding.GetBytes(string.Format("\"{0}\"", ((NdfStringReference)Value).Value));
        }
    }
}