using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using moddingSuite.Model.Ndfbin.ChangeManager;
using moddingSuite.Model.Ndfbin.Types;
using moddingSuite.Model.Ndfbin.Types.AllTypes;
using moddingSuite.Util;
using moddingSuite.View.DialogProvider;
using moddingSuite.ViewModel.Base;
using moddingSuite.ViewModel.Ndf;
using System.Diagnostics;

namespace moddingSuite.Model.Ndfbin
{
    public class NdfPropertyValue : ViewModelBase, IValueHolder, IEditableObject
    {
        private NdfObject _instance;
        private NdfProperty _property;
        private NdfValueWrapper _value;

        public NdfPropertyValue(NdfObject instance)
        {
            _instance = instance;

            DetailsCommand = new ActionCommand(DetailsCommandExecute);
        }

        public NdfType Type
        {
            get
            {
                if (Value == null)
                    return NdfType.Unset;

                return Value.Type;
            }
        }

        public NdfProperty Property
        {
            get { return _property; }
            set
            {
                _property = value;
                OnPropertyChanged("Property");
            }
        }

        public NdfObject Instance
        {
            get { return _instance; }
            set
            {
                _instance = value;
                OnPropertyChanged("Instance");
            }
        }

        #region IValueHolder Members

        public NdfValueWrapper Value
        {
            get { return _value; }
            set
            {
                _value = value;
                _value.ParentProperty = this;
                OnPropertyChanged("Value");
            }
        }

        public NdfBinary Manager
        {
            get { return Property.Class.Manager; }
        }

        #endregion

        public ICommand DetailsCommand { get; set; }

        public void DetailsCommandExecute(object obj)
        {
            var item = obj as IEnumerable<DataGridCellInfo>;

            if (item == null)
                return;

            var prop = item.First().Item as IValueHolder;

            FollowDetails(prop);
        }

        private void FollowDetails(IValueHolder prop)
        {
            if (prop == null || prop.Value == null)
                return;

            switch (prop.Value.Type)
            {
                case NdfType.MapList:
                case NdfType.List:
                    FollowList(prop);
                    break;
                case NdfType.ObjectReference:
                    FollowObjectReference(prop);
                    break;
                case NdfType.Map:
                    var map = prop.Value as NdfMap;

                    if (map != null)
                    {
                        FollowDetails(map.Key);
                        FollowDetails(map.Value as IValueHolder);
                    }

                    break;
                default:
                    return;
            }
        }

        private void FollowObjectReference(IValueHolder prop)
        {
            var refe = prop.Value as NdfObjectReference;

            if (refe == null)
                return;

            var vm = new NdfClassViewModel(refe.Class, null);

            NdfObjectViewModel inst = vm.Instances.SingleOrDefault(x => x.Id == refe.InstanceId);

            if (inst == null)
                return;

            vm.InstancesCollectionView.MoveCurrentTo(inst);

            DialogProvider.ProvideView(vm);
        }

        private void FollowList(IValueHolder prop)
        {
            var refe = prop.Value as NdfCollection;

            if (refe == null)
                return;

            
            var editor = new ListEditorViewModel(refe, Manager);
            DialogProvider.ProvideView(editor);
            
            
        }


        private byte[] _oldVal;
        private bool _dirty;

        public void BeginEdit()
        {
            if (_dirty)
                return;

            _oldVal = Value.GetBytes();

            _dirty = true;
        }

        public void CancelEdit()
        {
            _dirty = false;
        }

        public void EndEdit()
        {
            if (!_dirty)
                return;

            var newVal = Value.GetBytes();

            if (newVal != null && _oldVal != null && Utils.ByteArrayCompare(newVal, _oldVal))
                return;

            ChangeEntryBase change = null;

            switch (Value.Type)
            {
                case NdfType.Map:
                    var map = Value as NdfMap;
                    change = new MapChangeEntry(this, map.Key, map.Value as MapValueHolder);

                    break;

                case NdfType.ObjectReference:
                    var refe = Value as NdfObjectReference;
                    change = new ObjectReferenceChangeEntry(this, refe.Class.Id, refe.InstanceId);

                    break;

                default:
                    change = new FlatChangeEntry(this, Value);

                    break;
            }

            Manager.ChangeManager.AddChange(change);

            _dirty = false;
        }
    }
}