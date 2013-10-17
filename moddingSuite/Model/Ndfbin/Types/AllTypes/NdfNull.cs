using System;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfNull : NdfValueWrapper
    {
        public NdfNull(long offset)
            : base(NdfType.Unset, offset)
        {
        }

        public override string ToString()
        {
            return "null";
        }

        public override byte[] GetBytes(out bool valid)
        {
            valid = false;

            return new byte[0];
        }


        public override byte[] GetNdfText()
        {
            throw new NotImplementedException();
        }
    }
}