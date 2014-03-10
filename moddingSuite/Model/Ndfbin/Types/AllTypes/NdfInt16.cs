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
        public NdfInt16(short value, long offset)
            : base(NdfType.Int16, value, offset)
        {
        }

        public override byte[] GetBytes(out bool valid)
        {
            valid = true;

            try
            {
                return BitConverter.GetBytes(Convert.ToUInt16(Value));
            }
            catch (Exception)
            {
                valid = false;
                return new byte[0];
            }
        }

        public override byte[] GetNdfText()
        {
            return NdfTextWriter.NdfTextEncoding.GetBytes(Value.ToString());
        }
    }
}
