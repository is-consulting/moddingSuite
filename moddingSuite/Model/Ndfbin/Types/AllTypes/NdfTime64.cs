using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfTime64 : NdfFlatValueWrapper
    {
        public NdfTime64(DateTime value)
            : base(NdfType.Time64, value)
        {
        }

        public override byte[] GetBytes()
        {
            var unixdt = new DateTime(1970, 1, 1);
            var msdt = (DateTime)Value;

            ulong res = (ulong)msdt.Subtract(unixdt).TotalSeconds;

            return BitConverter.GetBytes(res);
        }

        public override byte[] GetNdfText()
        {
            throw new NotImplementedException();
        }
    }
}
