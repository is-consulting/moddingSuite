using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using IronPython.Runtime.Operations;
using moddingSuite.ViewModel.Ndf;
using moddingSuite.Model.Ndfbin;
using moddingSuite.Model.Ndfbin.Types;
using moddingSuite.Model.Ndfbin.Types.AllTypes;
using moddingSuite.View.DialogProvider;
using moddingSuite.ViewModel.Base;
using moddingSuite.ViewModel.Filter;
using System.Drawing.Design;
using System;
using IronPython.Runtime;

namespace moddingSuite.ViewModel.Ndf
{
    public class NdfObjectViewModel : ObjectWrapperViewModel<NdfObject>
    {
        //private ObservableCollection<PropertyFilterExpression> _propertyFilterExpressions = new ObservableCollection<PropertyFilterExpression>();
        public NdfObjectViewModel(NdfObject obj, ViewModelBase parentVm)
            : base(obj, parentVm)
        {
            var propVals = new List<NdfPropertyValue>();

            propVals.AddRange(obj.PropertyValues);

            foreach (var source in propVals.OrderBy(x => x.Property.Id))
                PropertyValues.Add(source);

            DetailsCommand = new ActionCommand(DetailsCommandExecute);
            AddPropertyCommand = new ActionCommand(AddPropertyExecute, AddPropertyCanExecute);
            RemovePropertyCommand = new ActionCommand(RemovePropertyExecute, RemovePropertyCanExecute);
            CopyToInstancesCommand = new ActionCommand(CopyToInstancesExecute);
        }
        //public ObservableCollection<PropertyFilterExpression> PropertyFilterExpressions2
        //{
           // get { return _propertyFilterExpressions; }
        //}
        public uint Id
        {
            get { return Object.Id; }
            set
            {
                Object.Id = value;
                OnPropertyChanged("Name");
            }
        }

        public ObservableCollection<NdfPropertyValue> PropertyValues { get; } =
            new ObservableCollection<NdfPropertyValue>();

        public ICommand DetailsCommand { get; protected set; }
        public ICommand AddPropertyCommand { get; protected set; }
        public ICommand RemovePropertyCommand { get; protected set; }
        public ICommand CopyToInstancesCommand { get; protected set; }

        /// <summary>
        /// Easy property indexing by name for scripts.
        /// </summary>
        public NdfValueWrapper this[string property]
        {
            get => GetPropertyValueByName(property)?.Value;
            set
            {
                NdfPropertyValue prop = GetPropertyValueByName(property);
                if (prop == null)
                    throw new KeyNotFoundException("unknown property");

                prop.BeginEdit();
                prop.Value = value;
                prop.EndEdit();
            }
        }

        private NdfPropertyValue GetPropertyValueByName(string name) => PropertyValues.FirstOrDefault(pv => pv.Property.Name == name);

        private void AddPropertyExecute(object obj)
        {
            var cv = CollectionViewSource.GetDefaultView(PropertyValues);
            
            if (obj == null)
            { 
            obj = cv.CurrentItem as NdfPropertyValue;
            }

            var item = obj as NdfPropertyValue;


            if (item == null)
                return;

            var type = NdfType.Unset;

            foreach (var instance in Object.Class.Instances)
            {
                foreach (var propertyValue in instance.PropertyValues)
                {
                    if (propertyValue.Property.Id == item.Property.Id)
                        if (propertyValue.Type != NdfType.Unset)
                            type = propertyValue.Type;
                }
            }

            if (type == NdfType.Unset || type == NdfType.Unknown)
                return;

            item.Value = NdfTypeManager.GetValue(new byte[NdfTypeManager.SizeofType(type)], type, item.Manager);
        }

        private bool AddPropertyCanExecute()
        {
            var cv = CollectionViewSource.GetDefaultView(PropertyValues);

            var item = cv.CurrentItem as NdfPropertyValue;

            if (item == null)
                return false;

            return item.Type == NdfType.Unset;
        }

        private void RemovePropertyExecute(object obj)
        {
            var cv = CollectionViewSource.GetDefaultView(PropertyValues);

            var item = cv.CurrentItem as NdfPropertyValue;

            if (item == null || item.Type == NdfType.Unset || item.Type == NdfType.Unknown)
                return;

            var result = MessageBox.Show("Do you want to set this property to null?", "Confirmation",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
                item.Value = NdfTypeManager.GetValue(new byte[0], NdfType.Unset, item.Manager);
        }

        private bool RemovePropertyCanExecute()
        {
            var cv = CollectionViewSource.GetDefaultView(PropertyValues);

            var item = cv.CurrentItem as NdfPropertyValue;

            if (item == null)
                return false;

            return item.Type != NdfType.Unset;
        }

        private void CopyToInstancesExecute(object obj)
        {
            var cv = CollectionViewSource.GetDefaultView(PropertyValues);
            
            var result = MessageBox.Show("Do you want to copy this instance value to ALL Filtered other instances? If unsure, press no", "Confirmation",
                MessageBoxButton.YesNo, MessageBoxImage.Question,defaultResult: MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
            {
                var item = cv.CurrentItem as NdfPropertyValue;

                //finds filtered instances list in steps to typecast correctly
                var ParentVmFinder = this.ParentVm as NdfEditorMainViewModel;
                var CCVFinder = ParentVmFinder.ClassesCollectionView.CurrentItem as NdfClassViewModel;
                var ICV = CCVFinder.InstancesCollectionView as ListCollectionView;
                foreach (NdfObjectViewModel instance in ICV)
                {
                    
                    var property = instance.PropertyValues.First(x => x.Property == item.Property);


                    if (property.Type== NdfType.Unset)
                           AddPropertyExecute(property);
                    
                     
                    property.BeginEdit();
                    
                    property.Value = GetCopiedValue(item);
                    property.EndEdit();
                }
            }
        }

        private NdfValueWrapper GetCopiedValue(IValueHolder toCopy)
        {
            NdfValueWrapper copiedValue = null;

            switch (toCopy.Value.Type)
            {
                case NdfType.ObjectReference:
                    var origInst = toCopy.Value as NdfObjectReference;

                        copiedValue = NdfTypeManager.GetValue(toCopy.Value.GetBytes(), toCopy.Value.Type, toCopy.Manager);

                    break;
                case NdfType.List:
                    var copiedItems = new List<CollectionItemValueHolder>();
                    var collection = toCopy.Value as NdfCollection;
                    if (collection != null)
                    {
                        copiedItems.AddRange(collection.Select(entry => new CollectionItemValueHolder(GetCopiedValue(entry), toCopy.Manager)));
                    }

                    copiedValue = new NdfCollection(copiedItems);
                    break;
                case NdfType.MapList:
                    // creates Maplist type copy as NdfMaplist:Collection, written by Reros
                    var copiedMapItems = new NdfMapList();
                    var collectionMap = toCopy.Value as NdfCollection;
                    if (collectionMap != null)
                    {
                        foreach (var item in collectionMap)
                        {
                            copiedMapItems.Add(new CollectionItemValueHolder(GetCopiedValue(item), toCopy.Manager));
                        }
                    }
                    copiedValue = copiedMapItems;
                    break;

                case NdfType.Map:
                    var map = toCopy.Value as NdfMap;
                    if (map != null)
                        copiedValue = new NdfMap(new MapValueHolder(GetCopiedValue(map.Key), toCopy.Manager),
                            new MapValueHolder(GetCopiedValue(map.Value as IValueHolder), toCopy.Manager), toCopy.Manager);
                    break;

                default:
                    copiedValue = NdfTypeManager.GetValue(toCopy.Value.GetBytes(), toCopy.Value.Type, toCopy.Manager);
                    break;
            }

            return copiedValue;
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
            if (prop?.Value == null)
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

            var vm = new NdfClassViewModel(refe.Class, ParentVm);

            var inst = vm.Instances.SingleOrDefault(x => x.Id == refe.InstanceId);

            if (inst == null)
                return;

            vm.InstancesCollectionView.MoveCurrentTo(inst);

            DialogProvider.ProvideView(vm, ParentVm);
        }

        private void FollowList(IValueHolder prop)
        {
            var refe = prop.Value as NdfCollection;

            if (refe == null)
                return;

            //if (IsTable(refe))
            //{
            var editor = new ListEditorViewModel(refe, Object.Class.Manager);
            DialogProvider.ProvideView(editor, ParentVm);
            //}
            //else
            //{
            //var editor = new ListEditorViewModel(refe, Object.Class.Manager);
            //DialogProvider.ProvideView(editor, ParentVm);
            //}
        }

        private bool IsTable(NdfCollection collection)
        {
            var map = collection.First().Value as NdfMap;

            if (collection == null)
                return false;

            var valHolder = map.Value as MapValueHolder;
            return valHolder.Value is NdfCollection;
        }
    }
}