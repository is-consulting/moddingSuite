using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.Model.Ndfbin.ChangeManager
{
    public class MapChangeEntry : ChangeEntryBase
    {
        private MapValueHolder _newKey;
        private MapValueHolder _newValue;

        public MapChangeEntry(NdfPropertyValue affectedPropertyValue, MapValueHolder newKey, MapValueHolder newValue)
            : base(affectedPropertyValue)
        {
            NewKey = newKey;
            NewValue = newValue;
 
        }

        public MapValueHolder NewKey
        {
            get { return _newKey; }
            set
            {
                _newKey = value;
                OnPropertyChanged("NewKey");
            }
        }

        public MapValueHolder NewValue
        {
            get { return _newValue; }
            set
            {
                _newValue = value;
                OnPropertyChanged("NewValue");
            }
        }


        int min(int l, int r)
        {
            return l < r ? l : r;
        }

        float min(float l, float r)
        {
            return l < r ? l : r;
        }

    }
}
