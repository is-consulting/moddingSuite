using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfEugInt2 : NdfFlatValueWrapper
    {
        private int _value2;

        public NdfEugInt2(int value1, int value2, long offset)
            : base(NdfType.EugInt2, value1, offset)
        {
            Value2 = value2;
        }

        public int Value2
        {
            get { return _value2; }
            set
            {
                _value2 = value;
                OnPropertyChanged("Value2");
            }
        }

        public override byte[] GetBytes(out bool valid)
        {
            valid = true;

            try
            {
                var value = new List<byte>();
                value.AddRange(BitConverter.GetBytes(Convert.ToInt32(Value)));
                value.AddRange(BitConverter.GetBytes(Convert.ToInt32(Value2)));
                return value.ToArray();
            }
            catch (Exception)
            {
                valid = false;
                return new byte[0];
            }
        }

        public override byte[] GetNdfText()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Format("Int pair: {0} : {1}", Value, Value2);
        }
    }
}
