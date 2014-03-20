using System;
using moddingSuite.BL;
using moddingSuite.BL.Ndf;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfSingle : NdfFlatValueWrapper
    {
        public NdfSingle(float value)
            : base(NdfType.Float32, value)
        {
        }

        public new float Value
        {
            get { return (float)base.Value; }
            set
            {
                base.Value = value;
                OnPropertyChanged("Value");
            }
        }

        public override byte[] GetBytes()
        {
            return BitConverter.GetBytes(Convert.ToSingle(Value));
        }

        public override string ToString()
        {
            return string.Format("{0:0.####################}", Value);
        }

        public override byte[] GetNdfText()
        {
            return NdfTextWriter.NdfTextEncoding.GetBytes(Value.ToString());
        }
    }
}