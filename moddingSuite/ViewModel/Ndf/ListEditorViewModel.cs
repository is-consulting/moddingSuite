using moddingSuite.BL;
using moddingSuite.Model.Ndfbin;
using moddingSuite.Model.Ndfbin.Types;
using moddingSuite.Model.Ndfbin.Types.AllTypes;
using moddingSuite.View.DialogProvider;
using moddingSuite.View.Ndfbin.Viewer;
using moddingSuite.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace moddingSuite.ViewModel.Ndf
{
    public class ListEditorViewModel : ViewModelBase
    {
        private NdfCollection _collection;
        private NdfbinManager _ndfbinManager;

        public NdfbinManager NdfbinManager
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

        public ListEditorViewModel(NdfCollection collection, NdfbinManager mgr)
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
            if (Value == null)
                return;

            NdfType type =
                Value.GroupBy(x => x.Value.Type).OrderByDescending(gp => gp.Count()).Select(x => x.First().Value.Type).
                    Single();

            var wrapper =
                new CollectionItemValueHolder(
                    NdfTypeManager.GetValue(new byte[NdfTypeManager.SizeofType(type)], type, NdfbinManager, 0), NdfbinManager, 0);

            Value.Add(wrapper);
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

            if (ret.HasValue && ret.Value)
                Value.Add(vm.Wrapper);
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
