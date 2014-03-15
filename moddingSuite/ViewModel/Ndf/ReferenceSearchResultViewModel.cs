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
    public class ReferenceSearchResultViewModel : ViewModelBase
    {
        public ObservableCollection<NdfPropertyValue> Results { get; set; }

        public ICommand DetailsCommand { get; set; }

        public NdfEditorMainViewModel Editor { get; set; }

        public ReferenceSearchResultViewModel(List<NdfPropertyValue> results, NdfEditorMainViewModel editor)
        {
            Results = new ObservableCollection<NdfPropertyValue>(results);

            Editor = editor;

            DetailsCommand = new ActionCommand(DetailsExecute);
        }

        private void DetailsExecute(object obj)
        {
            var propVal = obj as NdfPropertyValue;

            if (propVal == null)
                return;

            var vm = new NdfClassViewModel(propVal.Instance.Class, this);

            DialogProvider.ProvideView(vm, Editor);
        }
    }
}
