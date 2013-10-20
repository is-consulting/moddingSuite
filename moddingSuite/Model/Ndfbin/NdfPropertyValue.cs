using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using moddingSuite.BL;
using moddingSuite.Model.Ndfbin.ChangeManager;
using moddingSuite.Model.Ndfbin.Types;
using moddingSuite.Model.Ndfbin.Types.AllTypes;
using moddingSuite.Util;
using moddingSuite.View.DialogProvider;
using moddingSuite.View.Ndfbin.Viewer;
using moddingSuite.ViewModel.Base;
using moddingSuite.ViewModel.Ndf;

namespace moddingSuite.Model.Ndfbin
{
    public class NdfPropertyValue : ViewModelBase, IValueHolder
    {
        private NdfObject _instance;
        private byte[] _oldVal = new byte[0];
        private NdfProperty _property;
        private NdfValueWrapper _value;
        private byte[] _valueData;

        public NdfPropertyValue(NdfObject instance)
        {
            _instance = instance;

            DetailsCommand = new ActionCommand(DetailsCommandExecute);
            AddRowCommand = new ActionCommand(AddRowExecute);
            AddRowOfCommonTypeCommand = new ActionCommand(AddRowOfCommonTypeExecute, AddRowOfCommonTypeCanExecute);
            DeleteRowCommand = new ActionCommand(DeleteRowExecute, DeleteRowCanExecute);
        }

        public ICommand DetailsCommand { get; set; }
        public ICommand AddRowCommand { get; protected set; }
        public ICommand AddRowOfCommonTypeCommand { get; protected set; }
        public ICommand DeleteRowCommand { get; protected set; }

        public NdfType Type
        {
            get
            {
                if (Value == null)
                    return NdfType.Unset;

                return Value.Type;
            }
        }

        public byte[] ValueData
        {
            get { return _valueData; }
            set
            {
                _valueData = value;
                OnPropertyChanged("ValueData");
                OnPropertyChanged("BinValue");
            }
        }

        public string BinValue
        {
            get { return Utils.ByteArrayToBigEndianHeyByteString(ValueData); }
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
                OnPropertyChanged("Value");
            }
        }

        public NdfbinManager Manager
        {
            get { return Property.Class.Manager; }
        }

        public long InstanceOffset
        {
            get { return Instance.Offset; }
        }

        #endregion

        private bool AddRowOfCommonTypeCanExecute()
        {
            var col = Value as NdfCollection;

            return col != null && col.Count > 0;
        }

        private bool DeleteRowCanExecute()
        {
            ICollectionView cv = CollectionViewSource.GetDefaultView(Value);

            return cv != null && cv.CurrentItem != null;
        }

        private void DeleteRowExecute(object obj)
        {
            ICollectionView cv = CollectionViewSource.GetDefaultView(Value);

            if (cv == null || cv.CurrentItem == null)
                return;

            var val = cv.CurrentItem as CollectionItemValueHolder;

            if (val == null)
                return;

            ((NdfCollection)Value).Remove(cv.CurrentItem);
        }

        private void AddRowOfCommonTypeExecute(object obj)
        {
            var col = Value as NdfCollection;

            if (col == null)
                return;

            NdfType type =
                col.GroupBy(x => x.Value.Type).OrderByDescending(gp => gp.Count()).Select(x => x.First().Value.Type).
                    Single();

            var wrapper =
                new CollectionItemValueHolder(
                    NdfTypeManager.GetValue(new byte[NdfTypeManager.SizeofType(type)], type, Manager, 0), Manager, 0);

            col.Add(wrapper);
        }

        private void AddRowExecute(object obj)
        {
            ICollectionView cv = CollectionViewSource.GetDefaultView(Value);

            if (cv == null)
                return;

            var view = new AddCollectionItemView();
            var vm = new AddCollectionItemViewModel(Manager, view);

            view.DataContext = vm;

            bool? ret = view.ShowDialog();

            if (ret.HasValue && ret.Value)
                ((NdfCollection)Value).Add(vm.Wrapper);
        }

        public override void BeginEdit()
        {
            bool valid;
            _oldVal = Value.GetBytes(out valid);

            base.BeginEdit();
        }

        public override void EndEdit()
        {
            bool valid;

            byte[] newValue = Value.GetBytes(out valid);

            if (valid && !Utils.ByteArrayCompare(newValue, _oldVal))
            {
                Property.Class.Manager.ChangeManager.Changes.Add(new ChangeEntry
                                                                     {
                                                                         ChangedValue = this,
                                                                         NewValue = newValue,
                                                                         OldValue = _oldVal
                                                                     });

                _oldVal = newValue;
            }

            base.EndEdit();
        }




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

            DialogProvider.ProvideView(prop as ViewModelBase);
            var view = new ListEditorWindow { DataContext = prop };
            view.Show();
        }
    }
}