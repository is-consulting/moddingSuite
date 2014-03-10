using System;
using moddingSuite.BL;
using moddingSuite.BL.Ndf;

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
            return NdfTextWriter.NdfTextEncoding.GetBytes(string.Format("\"{0}\"", ((NdfStringReference)Value).Value));
        }
    }
}