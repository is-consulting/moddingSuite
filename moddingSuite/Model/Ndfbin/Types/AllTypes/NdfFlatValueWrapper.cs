using System.Diagnostics;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public abstract class NdfFlatValueWrapper : NdfValueWrapper
    {
        private object _value;

        protected NdfFlatValueWrapper(NdfType type, object value)
            : base(type)
        {
            Value = value;
        }

        public object Value
        {
            get { return _value; }
            set
            {
                if (value == null)
                {
                    Trace.TraceInformation("value is null, why that");
                }
                _value = value;
                OnPropertyChanged("Value");
            }
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}