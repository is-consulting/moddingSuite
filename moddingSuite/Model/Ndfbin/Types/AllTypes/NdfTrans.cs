using System;
using moddingSuite.BL;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfTrans : NdfFlatValueWrapper
    {
        public NdfTrans(NdfTranReference value, long offset)
            : base(NdfType.TransTableReference, value, offset)
        {
        }

        public override byte[] GetBytes(out bool valid)
        {
            valid = true;

            return BitConverter.GetBytes(((NdfTranReference) Value).Id);
        }


        public override byte[] GetNdfText()
        {
            return NdfbinManager.NdfTextEncoding.GetBytes(string.Format("\"{0}\"", ((NdfStringReference) Value).Value));
        }
    }
}