using System;
using System.Drawing;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfColor32 : NdfFlatValueWrapper
    {
        public NdfColor32(Color value)
            : base(NdfType.Color32, value)
        {
        }

        public override byte[] GetBytes()
        {
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