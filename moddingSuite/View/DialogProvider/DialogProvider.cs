using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using moddingSuite.Model.Ndfbin;
using moddingSuite.View.Common;
using moddingSuite.View.Edata;
using moddingSuite.View.Ndfbin;
using moddingSuite.View.Ndfbin.Viewer;
using moddingSuite.View.VersionManager;
using moddingSuite.ViewModel;
using moddingSuite.ViewModel.About;
using moddingSuite.ViewModel.Base;
using moddingSuite.ViewModel.Edata;
using moddingSuite.ViewModel.Ndf;
using moddingSuite.ViewModel.Trad;
using moddingSuite.ViewModel.VersionManager;

namespace moddingSuite.View.DialogProvider
{
    public static class DialogProvider
    {
        private static IList<ViewInstance> _registeredViews = new List<ViewInstance>();

        private static IList<IViewMap> _maps = new List<IViewMap>();

        public static IList<ViewInstance> RegisteredViews
        {
            get { return _registeredViews; }
            set { _registeredViews = value; }
        }

        public static IList<IViewMap> Maps
        {
            get { return _maps; }
            set { _maps = value; }
        }

        static DialogProvider()
        {
            Maps.Add(new ViewMap<EdataManagerView, EdataManagerViewModel>());

            Maps.Add(new ViewMap<NdfbinView, NdfEditorMainViewModel>());
            Maps.Add(new ViewMap<InstanceWindowView, NdfClassViewModel>());
            Maps.Add(new ViewMap<ListEditorWindow, ListEditorViewModel>());


            Maps.Add(new ViewMap<TradFileView, TradFileViewModel>());

            Maps.Add(new ViewMap<AboutView, AboutViewModel>());

            Maps.Add(new ViewMap<VersionManagerView, VersionManagerViewModel>());
        }

        public static void ProvideView(ViewModelBase vm, ViewModelBase parentVm = null)
        {
            var map = Maps.SingleOrDefault(x => x.ViewModelType == vm.GetType());

            if (map == null)
                return;

            Window parentView = null;

            if (parentVm != null)
            {
                var parent = RegisteredViews.SingleOrDefault(x => x.ViewModel == parentVm);

                if (parent != null)
                    parentView = parent.View;
            }

            var viewInstance = Activator.CreateInstance(map.ViewType) as Window;

            if (viewInstance == null)
                throw new InvalidOperationException(string.Format("Can not create an instance of {0}", map.ViewType));

            viewInstance.Owner = parentView;
            viewInstance.DataContext = vm;

            RegisteredViews.Add(new ViewInstance(viewInstance, vm));

            viewInstance.Show();
        }
    }
}
