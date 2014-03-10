using System;
using moddingSuite.BL;
using moddingSuite.BL.Ndf;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfSingle : NdfFlatValueWrapper
    {
        public NdfSingle(float value, long offset)
            : base(NdfType.Float32, value, offset)
        {
        }

        public new float Value
        {
            get { return (float) base.Value; }
            set
            {
                base.Value = value;
                OnPropertyChanged("Value");
            }
        }

        public override byte[] GetBytes(out bool valid)
        {
            valid = true;

            try
            {
                return BitConverter.GetBytes(Convert.ToSingle(Value));
            }
            catch (Exception)
            {
                valid = false;
                return new byte[0];
            }
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