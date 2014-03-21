using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using moddingSuite.Model.Ndfbin;
using moddingSuite.View.DialogProvider;
using moddingSuite.ViewModel.Base;

namespace moddingSuite.ViewModel.Ndf
{
    public class ObjectCopyResultViewModel : ViewModelBase
    {
        public ObservableCollection<NdfObject> NewInstances { get; set; }

        public ICommand DetailsCommand { get; set; }

        public NdfEditorMainViewModel Editor { get; set; }

        public ObjectCopyResultViewModel(List<NdfObject> results, NdfEditorMainViewModel editor)
        {
            NewInstances = new ObservableCollection<NdfObject>(results);

            Editor = editor;

            DetailsCommand = new ActionCommand(DetailsExecute);
        }

        private void DetailsExecute(object obj)
        {
            var instance = obj as NdfObject;

            if (instance == null)
                return;

            var vm = new NdfClassViewModel(instance.Class, this);

            NdfObjectViewModel inst = vm.Instances.SingleOrDefault(x => x.Id == instance.Id);

            if (inst == null)
                return;

            vm.InstancesCollectionView.MoveCurrentTo(inst);

            DialogProvider.ProvideView(vm, Editor);
        }
    }
}
