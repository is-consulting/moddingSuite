using moddingSuite.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace moddingSuite.View.DialogProvider
{
    public class ViewInstance
    {
        public Window View { get; protected set; }
        public ViewModelBase ViewModel { get; protected set; }

        public ViewInstance(Window view, ViewModelBase vm)
        {
            if (view == null)
                throw new ArgumentException("view");

            if (vm == null)
                throw new ArgumentException("vm");

            View = view;
            ViewModel = vm;
        }

    }
}
