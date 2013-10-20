using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using moddingSuite.ViewModel.Base;

namespace moddingSuite.View.DialogProvider
{
    public class ViewMap<TView, TViewModel> : IViewMap
        where TView : Window
        where TViewModel : ViewModelBase
    {
        public Type ViewType { get; protected set; }
        public Type ViewModelType { get; protected set; }

        public ViewMap()
        {
            ViewType = typeof(TView);
            ViewModelType = typeof(TViewModel);
        } 
    }
}
