using System;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfGuid : NdfFlatValueWrapper
    {
        public NdfGuid(Guid value, long offset)
            : base(NdfType.Guid, value, offset)
        {
        }

        public override byte[] GetBytes(out bool valid)
        {
            valid = true;

            return Guid.Parse(Value.ToString()).ToByteArray();
        }

        public override byte[] GetNdfText()
        {
            throw new NotImplementedException();
        }
    }
}