using System;
using System.Collections.Generic;
using System.Text;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfWideString : NdfFlatValueWrapper
    {
        public NdfWideString(string value)
            : base(NdfType.WideString, value)
        {
        }

        public override byte[] GetBytes()
        {
            var data = new List<byte>();

            var val = (string) Value;

            byte[] valBytes = Encoding.Unicode.GetBytes(val);

            data.AddRange(BitConverter.GetBytes(valBytes.Length));
            data.AddRange(valBytes);

            return data.ToArray();
        }

        public override byte[] GetNdfText()
        {
            throw new NotImplementedException();
        }
    }
}