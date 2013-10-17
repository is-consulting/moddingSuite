using System;
using moddingSuite.BL;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfString : NdfFlatValueWrapper
    {
        public NdfString(NdfStringReference value, long offset)
            : base(NdfType.TableString, value, offset)
        {
        }

        public override byte[] GetBytes(out bool valid)
        {
            valid = true;

            return BitConverter.GetBytes(((NdfStringReference) Value).Id);
        }

        public override byte[] GetNdfText()
        {
            return NdfbinManager.NdfTextEncoding.GetBytes(string.Format("\"{0}\"", ((NdfStringReference) Value).Value));
        }
    }
}