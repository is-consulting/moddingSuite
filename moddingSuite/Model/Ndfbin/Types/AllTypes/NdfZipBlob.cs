using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfZipBlob : NdfFlatValueWrapper
    {
        public NdfZipBlob(byte[] value, long offset) : base(NdfType.ZipBlob, value, offset)
        {
            
        }

        public override byte[] GetBytes(out bool valid)
        {
            valid = true;

            var val = new List<byte>();

            val.AddRange(BitConverter.GetBytes((uint)((byte[])Value).Length));
            val.Add(1);
            val.AddRange((byte[])Value);

            return val.ToArray();
        }

        public override byte[] GetNdfText()
        {
            throw new NotImplementedException();
        }
    }
}
