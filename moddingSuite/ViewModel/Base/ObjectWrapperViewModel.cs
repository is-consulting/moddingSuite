using System;

namespace moddingSuite.ViewModel.Base
{
    public class ObjectWrapperViewModel<T> : ViewModelBase where T : ViewModelBase
    {
        public ObjectWrapperViewModel(T obj)
        {
            if (obj == null)
                throw new ArgumentException("obj");

            Object = obj;
        }

        public T Object { get; protected set; }
    }
}