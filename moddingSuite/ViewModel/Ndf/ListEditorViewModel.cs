using moddingSuite.BL;
using moddingSuite.Model.Ndfbin;
using moddingSuite.Model.Ndfbin.Types;
using moddingSuite.Model.Ndfbin.Types.AllTypes;
using moddingSuite.View.DialogProvider;
using moddingSuite.View.Ndfbin.Viewer;
using moddingSuite.ViewModel.Base;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using static IronPython.Modules._ast;

namespace moddingSuite.ViewModel.Ndf
{
    public class ListEditorViewModel : ViewModelBase
    {
        private NdfCollection _collection;
        private NdfBinary _ndfbinManager;

        private bool _isInsertMode;

        public NdfBinary NdfbinManager
        {
            get { return _ndfbinManager; }
            set { _ndfbinManager = value; OnPropertyChanged(() => NdfbinManager); }
        }

        public NdfCollection Value
        {
            get { return _collection; }
            set { _collection = value; OnPropertyChanged(() => Value); }
        }

        public ICommand DetailsCommand { get; set; }
        public ICommand AddRowCommand { get; protected set; }
        public ICommand AddRowOfCommonTypeCommand { get; protected set; }
        public ICommand DeleteRowCommand { get; protected set; }

        public bool IsInsertMode
        {
            get { return _isInsertMode; }
            set { _isInsertMode = value; OnPropertyChanged(() => IsInsertMode); }
        }

        public ListEditorViewModel(NdfCollection collection, NdfBinary mgr)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (mgr == null)
                throw new ArgumentNullException("mgr");

            _collection = collection;
            _ndfbinManager = mgr;
            DetailsCommand = new ActionCommand(DetailsCommandExecute);

            AddRowCommand = new ActionCommand(AddRowExecute);
            AddRowOfCommonTypeCommand = new ActionCommand(AddRowOfCommonTypeExecute, AddRowOfCommonTypeCanExecute);
            DeleteRowCommand = new ActionCommand(DeleteRowExecute, DeleteRowCanExecute);
        }


        private bool AddRowOfCommonTypeCanExecute()
        {
            return Value != null && Value.Count > 0;
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

            Value.Remove(val);
        }
        
        private void AddRowOfCommonTypeExecute(object obj)
        {
            var cv = CollectionViewSource.GetDefaultView(Value);

            if (cv == null)
                return;
            
            if (cv.CurrentItem == null)
                return;

            NdfType type =
                Value.GroupBy(x => x.Value.Type).OrderByDescending(gp => gp.Count()).Select(x => x.First().Value.Type).
                    Single();
            var val = cv.CurrentItem as CollectionItemValueHolder;
            if (val == null)
                return;


            CollectionItemValueHolder wrapper = null;
            switch (type)
            {
                case NdfType.Map:
                    var map = val.Value as NdfMap;
                    MapValueHolder key = null;
                    MapValueHolder value = null;
                    var tempvalue = map.Value as MapValueHolder;
                    switch (map.Key.Value.Type)
                    {
                        case NdfType.List:
                            var newlist = new List<CollectionItemValueHolder>();
                            var list = map.Key.Value as NdfCollection;
                            newlist.AddRange(list.Select(entry => new CollectionItemValueHolder(CloneObject(entry.Value), NdfbinManager)));
                            key= new MapValueHolder(new NdfCollection(newlist),NdfbinManager);
                            break;

                        default:
                            key = new MapValueHolder(CloneObject(map.Key.Value), NdfbinManager);
                            break;

                    }
                    switch (tempvalue.Value.Type)
                    {
                        case NdfType.List:
                            var newlist = new List<CollectionItemValueHolder>();
                            var list = tempvalue.Value as NdfCollection;
                            newlist.AddRange(list.Select(entry => new CollectionItemValueHolder(CloneObject(entry), NdfbinManager)));
                            value = new MapValueHolder(new NdfCollection(newlist), NdfbinManager);
                            break;
                        
                        default:
                            value = new MapValueHolder(CloneObject(tempvalue.Value), NdfbinManager);
                            break;
                    }
                    
                    wrapper = new CollectionItemValueHolder(new NdfMap(key, value, NdfbinManager),NdfbinManager);
                    break;

                default:
                    wrapper = new CollectionItemValueHolder(CloneObject(val.Value), NdfbinManager);
                    break;
            }

            if (IsInsertMode)
            {
                Value.Insert(cv.CurrentPosition + 1, wrapper);
            }
            else
                Value.Add(wrapper);

            cv.MoveCurrentTo(wrapper);
        }

        private NdfValueWrapper CloneObject(object obj)
        {
            var value = obj as NdfValueWrapper;
            NdfValueWrapper clonedValue = null;
            switch (value.Type)
            {
                case NdfType.UInt32:
                    clonedValue = new NdfUInt32(BitConverter.ToUInt32(value.GetBytes(), 0));
                    break;

                case NdfType.Int32:
                    clonedValue = new NdfInt32(BitConverter.ToInt32(value.GetBytes(), 0));
                    break;

                case NdfType.Int16:
                    clonedValue = new NdfInt16(BitConverter.ToInt16(value.GetBytes(), 0));
                    break;

                case NdfType.UInt16:
                    clonedValue = new NdfUInt16(BitConverter.ToUInt16(value.GetBytes(), 0));
                    break;

                case NdfType.Float32:
                    clonedValue  = new NdfSingle(BitConverter.ToSingle(value.GetBytes(), 0));
                    break;

                case NdfType.LocalisationHash:
                    clonedValue = new NdfLocalisationHash(value.GetBytes());
                    break;

                case NdfType.TableString:
                    var tblstr = value as NdfString;
                    var strvl = tblstr.Value as NdfStringReference;
                    clonedValue = new NdfString(strvl);
                    break;

                case NdfType.ObjectReference:
                    var objref = value as NdfObjectReference;
                    clonedValue = new NdfObjectReference(objref.Class, objref.InstanceId);
                    break;

                default:
                    clonedValue = NdfTypeManager.GetValue(new byte[NdfTypeManager.SizeofType(value.Type)], value.Type, NdfbinManager);
                    break;
            }
            return clonedValue;
        }
        private void AddRowExecute(object obj)
        {
            ICollectionView cv = CollectionViewSource.GetDefaultView(Value);

            if (cv == null)
                return;

            var view = new AddCollectionItemView();
            var vm = new AddCollectionItemViewModel(NdfbinManager, view);

            view.DataContext = vm;

            bool? ret = view.ShowDialog();

            if (!ret.HasValue || !ret.Value)
                return;


            if (IsInsertMode)
            {
                if (cv.CurrentItem == null)
                    return;

                var val = cv.CurrentItem as CollectionItemValueHolder;

                if (val == null)
                    return;

                Value.Insert(cv.CurrentPosition + 1, vm.Wrapper);
            }
            else
                Value.Add(vm.Wrapper);

            cv.MoveCurrentTo(vm.Wrapper);
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

            var editor = new ListEditorViewModel(refe, NdfbinManager);

            DialogProvider.ProvideView(editor, this);
        }


    }
}
