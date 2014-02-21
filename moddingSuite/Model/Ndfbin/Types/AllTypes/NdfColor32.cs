using System;
using System.Windows.Media;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfColor32 : NdfFlatValueWrapper
    {
        public NdfColor32(Color value, long offset)
            : base(NdfType.Color32, value, offset)
        {
        }

        public override byte[] GetBytes(out bool valid)
        {
            valid = true;

            var col = (Color) Value;

            var colorArray = new[] { col.R, col.G, col.B, col.A};

            return colorArray;
        }

        public override byte[] GetNdfText()
        {
            throw new NotImplementedException();
        }
    }
}