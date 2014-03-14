using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using moddingSuite.Model.Mesh;
using moddingSuite.View.DialogProvider;
using moddingSuite.ViewModel.Base;
using moddingSuite.ViewModel.Ndf;

namespace moddingSuite.ViewModel.Mesh
{
    public class MeshEditorViewModel : ViewModelBase
    {
        private MeshFile _meshFile;
        private ICollectionView _multiMaterialMeshesCollectionView;
        private string _multiMaterialMeshesFilterExpression = string.Empty;

        public ICommand EditTextureBindingsCommand { get; protected  set; }

        public ICollectionView MultiMaterialMeshesCollectionView
        {
            get
            {
                if (_multiMaterialMeshesCollectionView == null)
                {
                    BuildMultiMaterialMeshesCollectionView();
                }

                return _multiMaterialMeshesCollectionView;
            }
        }

        public MeshFile MeshFile
        {
            get { return _meshFile; }
            set { _meshFile = value; OnPropertyChanged("MeshFile"); }
        }

        public string MultiMaterialMeshesFilterExpression
        {
            get { return _multiMaterialMeshesFilterExpression; }
            set
            {
                _multiMaterialMeshesFilterExpression = value;
                OnPropertyChanged(() => MultiMaterialMeshesFilterExpression);
                MultiMaterialMeshesCollectionView.Refresh();
            }
        }

        public MeshEditorViewModel(MeshFile file)
        {
            MeshFile = file;

            EditTextureBindingsCommand = new ActionCommand(EditTextureBindingsExecute);
        }

        private void EditTextureBindingsExecute(object obj)
        {
            var ndfEditor = new NdfEditorMainViewModel(MeshFile.TextureBindings);

            DialogProvider.ProvideView(ndfEditor, this);
        }

        private void BuildMultiMaterialMeshesCollectionView()
        {
            _multiMaterialMeshesCollectionView = CollectionViewSource.GetDefaultView(MeshFile.MultiMaterialMeshFiles);
            _multiMaterialMeshesCollectionView.Filter = FilterMultiMaterialMeshes;

            OnPropertyChanged(() => MultiMaterialMeshesCollectionView);
        }

        private bool FilterMultiMaterialMeshes(object obj)
        {
            var file = obj as MeshContentFile;

            if (file == null || MultiMaterialMeshesFilterExpression == string.Empty || MultiMaterialMeshesFilterExpression.Length < 3)
            {
                return true;
            }

            return file.Path.Contains(MultiMaterialMeshesFilterExpression);
        }

    }
}
