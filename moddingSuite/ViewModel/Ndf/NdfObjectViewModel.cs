using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using moddingSuite.Model.Ndfbin;
using moddingSuite.Model.Ndfbin.Types;
using moddingSuite.Model.Ndfbin.Types.AllTypes;
using moddingSuite.View.DialogProvider;
using moddingSuite.ViewModel.Base;

namespace moddingSuite.ViewModel.Ndf
{
    public class NdfObjectViewModel : ObjectWrapperViewModel<NdfObject>
    {
        private readonly ObservableCollection<NdfPropertyValue> _propertyValues =
            new ObservableCollection<NdfPropertyValue>();

        public NdfObjectViewModel(NdfObject obj, ViewModelBase parentVm)
            : base(obj, parentVm)
        {
            var propVals = new List<NdfPropertyValue>();

            propVals.AddRange(obj.PropertyValues);

            foreach (NdfPropertyValue source in propVals.OrderBy(x => x.Property.Id))
                _propertyValues.Add(source);

            DetailsCommand = new ActionCommand(DetailsCommandExecute);
            AddPropertyCommand = new ActionCommand(AddPropertyExecute, AddPropertyCanExecute);
            RemovePropertyCommand = new ActionCommand(RemovePropertyExecute, RemovePropertyCanExecute);
            CopyToInstancesCommand = new ActionCommand(CopyToInstancesExecute);
        }

        public uint Id
        {
            get { return Object.Id; }
            set
            {
                Object.Id = value;
                OnPropertyChanged("Name");
            }
        }

        public ObservableCollection<NdfPropertyValue> PropertyValues
        {
            get { return _propertyValues; }
        }

        public ICommand DetailsCommand { get; protected set; }
        public ICommand AddPropertyCommand { get; protected set; }
        public ICommand RemovePropertyCommand { get; protected set; }
        public ICommand CopyToInstancesCommand { get; protected set; }
        private void AddPropertyExecute(object obj)
        {
            ICollectionView cv = CollectionViewSource.GetDefaultView(PropertyValues);

            var item = cv.CurrentItem as NdfPropertyValue;

            if (item == null)
                return;

            var type = NdfType.Unset;

            foreach (NdfObject instance in Object.Class.Instances)
            {
                foreach (NdfPropertyValue propertyValue in instance.PropertyValues)
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
            ICollectionView cv = CollectionViewSource.GetDefaultView(PropertyValues);

            var item = cv.CurrentItem as NdfPropertyValue;

            if (item == null)
                return false;

            return item.Type == NdfType.Unset;
        }

        private void RemovePropertyExecute(object obj)
        {
            ICollectionView cv = CollectionViewSource.GetDefaultView(PropertyValues);

            var item = cv.CurrentItem as NdfPropertyValue;

            if (item == null || item.Type == NdfType.Unset || item.Type == NdfType.Unknown)
                return;

            MessageBoxResult result = MessageBox.Show("Do you want to set this property to null?", "Confirmation",
                                                      MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
                item.Value = NdfTypeManager.GetValue(new byte[0], NdfType.Unset, item.Manager);
        }

        private bool RemovePropertyCanExecute()
        {
            ICollectionView cv = CollectionViewSource.GetDefaultView(PropertyValues);

            var item = cv.CurrentItem as NdfPropertyValue;

            if (item == null)
                return false;

            return item.Type != NdfType.Unset;
        }
        private void CopyToInstancesExecute(object obj)
        {
            ICollectionView cv = CollectionViewSource.GetDefaultView(PropertyValues);

            var item = cv.CurrentItem as NdfPropertyValue;
            foreach (var instance in item.Instance.Class.Instances)
            {
                var property=instance.PropertyValues.First(x => x.Property == item.Property);
                property.BeginEdit();
                property.Value = item.Value;
                property.EndEdit();
                
            }
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

            var vm = new NdfClassViewModel(refe.Class, ParentVm);

            NdfObjectViewModel inst = vm.Instances.SingleOrDefault(x => x.Id == refe.InstanceId);

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
            
            if (isTable(refe))
            {
                var editor = new ListEditorViewModel(refe, Object.Class.Manager);
                DialogProvider.ProvideView(editor, ParentVm);
            }
            else
            {
                var editor = new ListEditorViewModel(refe, Object.Class.Manager);
                DialogProvider.ProvideView(editor, ParentVm);
            }
        }
        private bool isTable(NdfCollection collection)
        {
            var map = collection.First().Value as NdfMap;
            if (collection == null)
                return false;
            var valHolder = map.Value as MapValueHolder;
            return valHolder.Value is NdfCollection;

        }
    }
}