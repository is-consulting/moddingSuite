using System;

namespace moddingSuite.ViewModel.Base
{
    public abstract class ObjectWrapperViewModel<T> : ViewModelBase 
        where T : ViewModelBase
    {
        private ViewModelBase _parentVm;

        protected ObjectWrapperViewModel(T obj, ViewModelBase parentVm)
        {
            if (obj == null)
                throw new ArgumentException("obj");
            //if (parentVm == null)
            //    throw new ArgumentException("parentVm");

            Object = obj;
            ParentVm = parentVm;
        }

        public T Object { get; protected set; }

        public ViewModelBase ParentVm
        {
            get { return _parentVm; }
            set { _parentVm = value; OnPropertyChanged("ParentVm"); }
        }
    }
}