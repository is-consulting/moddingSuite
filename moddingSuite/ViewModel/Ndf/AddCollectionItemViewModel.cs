using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using moddingSuite.Model.Ndfbin;
using moddingSuite.Model.Ndfbin.Types;
using moddingSuite.ViewModel.Base;

namespace moddingSuite.ViewModel.Ndf
{
    public class AddCollectionItemViewModel : ViewModelBase
    {
        private readonly List<NdfType> _typeSelection = new List<NdfType>();
        private NdfType _type = NdfType.Unset;
        private CollectionItemValueHolder _wrapper;

        public AddCollectionItemViewModel(NdfBinary mgr, Window view)
        {
            Manager = mgr;
            View = view;

            OkCommand = new ActionCommand(OkCommandExecute, () => Type != NdfType.Unset);
            CancelCommand = new ActionCommand(CancelCommandExecute);

            _typeSelection.AddRange(NdfTypeManager.GetTypeSelection());
        }

        public ICommand OkCommand { get; protected set; }
        public ICommand CancelCommand { get; protected set; }
        public NdfBinary Manager { get; protected set; }

        protected Window View { get; set; }

        public NdfType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                GetValueForType();
                OnPropertyChanged(() => Type);
            }
        }

        public List<NdfType> TypeSelection
        {
            get { return _typeSelection; }
        }

        public CollectionItemValueHolder Wrapper
        {
            get { return _wrapper; }
            set
            {
                _wrapper = value;
                OnPropertyChanged(() => Wrapper);
            }
        }

        private void GetValueForType()
        {
            Wrapper = new CollectionItemValueHolder(NdfTypeManager.GetValue(new byte[NdfTypeManager.SizeofType(Type)], Type, Manager), Manager);
        }

        private void CancelCommandExecute(object obj)
        {
            View.DialogResult = false;
        }

        private void OkCommandExecute(object obj)
        {
            View.DialogResult = true;
            View.Close();
        }
    }
}