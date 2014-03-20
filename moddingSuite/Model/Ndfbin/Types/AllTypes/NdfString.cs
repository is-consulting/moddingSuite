using System;
using moddingSuite.BL;
using moddingSuite.BL.Ndf;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfString : NdfFlatValueWrapper
    {
        public NdfString(NdfStringReference value)
            : base(NdfType.TableString, value)
        {
        }

        public override byte[] GetBytes()
        {
            return BitConverter.GetBytes(((NdfStringReference)Value).Id);
        }

        public override byte[] GetNdfText()
        {
            return NdfTextWriter.NdfTextEncoding.GetBytes(string.Format("\"{0}\"", ((NdfStringReference)Value).Value));
        }
    }
}