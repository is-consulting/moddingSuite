using moddingSuite.BL;
using moddingSuite.Model.Ndfbin.ChangeManager;
using moddingSuite.Model.Ndfbin.Types.AllTypes;
using moddingSuite.Util;
using moddingSuite.ViewModel.Base;

namespace moddingSuite.Model.Ndfbin
{
    public class CollectionItemValueHolder : ViewModelBase, IValueHolder
    {
        //private byte[] _oldVal = new byte[0];
        private NdfValueWrapper _value;

        public CollectionItemValueHolder(NdfValueWrapper value, NdfBinary manager)
        {
            Value = value;
            Manager = manager;
        }

        #region IValueHolder Members

        public virtual NdfBinary Manager { get; private set; }

        public virtual NdfValueWrapper Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }

        #endregion

        public override void BeginEdit()
        {
            //bool valid;
            //_oldVal = Value.GetBytes(out valid);

            base.BeginEdit();
        }

        public override void EndEdit()
        {
            //bool valid;

            //byte[] newValue = Value.GetBytes(out valid);

            //if (valid && !Utils.ByteArrayCompare(newValue, _oldVal))
            //{
            //    Manager.ChangeManager.Changes.Add(new ChangeEntry
            //                                          {
            //                                              ChangedValue = this,
            //                                              NewValue = newValue,
            //                                              OldValue = _oldVal
            //                                          });

            //    OnPropertyChanged(() => Value);

            //    _oldVal = newValue;
            //}

            base.EndEdit();
        }
    }
}