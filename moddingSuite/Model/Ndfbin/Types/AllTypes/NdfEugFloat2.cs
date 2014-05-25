using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfEugFloat2 : NdfFlatValueWrapper
    {
        private float _value2;

        public NdfEugFloat2(float value1, float value2)
            : base(NdfType.EugFloat2, value1)
        {
            Value2 = value2;
        }

        public float Value2
        {
            get { return _value2; }
            set
            {
                _value2 = value;
                OnPropertyChanged("Value2");
            }
        }

        public override byte[] GetBytes()
        {
            var value = new List<byte>();
            value.AddRange(BitConverter.GetBytes(Convert.ToSingle(Value)));
            value.AddRange(BitConverter.GetBytes(Convert.ToSingle(Value2)));
            return value.ToArray();
        }

        public override byte[] GetNdfText()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Format("Float pair: {0} : {1}", Value, Value2);
        }
    }
}
