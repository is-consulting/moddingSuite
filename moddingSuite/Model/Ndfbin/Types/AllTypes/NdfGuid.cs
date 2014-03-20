using System;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfGuid : NdfFlatValueWrapper
    {
        public NdfGuid(Guid value)
            : base(NdfType.Guid, value)
        {
        }

        public override byte[] GetBytes()
        {
            return Guid.Parse(Value.ToString()).ToByteArray();
        }

        public override byte[] GetNdfText()
        {
            throw new NotImplementedException();
        }
    }
}