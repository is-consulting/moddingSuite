using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using moddingSuite.ViewModel.Base;

namespace moddingSuite.Model.Ndfbin.ChangeManager
{
    public abstract class ChangeEntryBase : ViewModelBase
    {
        private NdfPropertyValue _affectedPropertyValue;

        public ChangeEntryBase(NdfPropertyValue affectedPropertyValue)
        {
            AffectedPropertyValue = affectedPropertyValue;
        }

        public NdfPropertyValue AffectedPropertyValue
        {
            get { return _affectedPropertyValue; }
            set
            {
                _affectedPropertyValue = value;
                OnPropertyChanged("AffectedPropertyValue");
            }
        }

    }
}
