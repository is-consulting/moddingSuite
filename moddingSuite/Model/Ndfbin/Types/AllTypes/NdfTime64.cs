using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfTime64 : NdfFlatValueWrapper
    {
        public NdfTime64(DateTime value, long pos)
            : base(NdfType.Time64, value, pos)
        {

        }

        public override byte[] GetBytes(out bool valid)
        {
            valid = true;

            try
            {
                var unixdt = new DateTime(1970, 1, 1);
                var msdt = (DateTime) Value;

                ulong res = (ulong)msdt.Subtract(unixdt).TotalSeconds;

                return BitConverter.GetBytes(res);
            }
            catch
            {
                valid = false;
                return new byte[0];
            }
        }

        public override byte[] GetNdfText()
        {
            throw new NotImplementedException();
        }
    }
}
