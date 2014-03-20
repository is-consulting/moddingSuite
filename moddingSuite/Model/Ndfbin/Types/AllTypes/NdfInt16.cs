using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using moddingSuite.BL;
using moddingSuite.BL.Ndf;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfInt16 : NdfFlatValueWrapper
    {
        public NdfInt16(short value)
            : base(NdfType.Int16, value)
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
