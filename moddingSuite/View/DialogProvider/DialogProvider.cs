using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using moddingSuite.View.Common;
using moddingSuite.View.Edata;
using moddingSuite.View.Mesh;
using moddingSuite.View.Ndfbin;
using moddingSuite.View.Ndfbin.Viewer;
using moddingSuite.View.Scenario;
using moddingSuite.View.VersionManager;
using moddingSuite.ViewModel.About;
using moddingSuite.ViewModel.Base;
using moddingSuite.ViewModel.Edata;
using moddingSuite.ViewModel.Mesh;
using moddingSuite.ViewModel.Ndf;
using moddingSuite.ViewModel.Scenario;
using moddingSuite.ViewModel.Trad;
using moddingSuite.ViewModel.UnhandledException;
using moddingSuite.ViewModel.VersionManager;

namespace moddingSuite.View.DialogProvider
{
    public static class DialogProvider
    {
        private static IList<ViewInstance> _registeredViews = new List<ViewInstance>();

        private static IList<IViewMap> _maps = new List<IViewMap>();

        static DialogProvider()
        {
            Maps.Add(new ViewMap<EdataManagerView, EdataManagerViewModel>());

            Maps.Add(new ViewMap<NdfbinView, NdfEditorMainViewModel>());
            Maps.Add(new ViewMap<MeshView, MeshEditorViewModel>());
            Maps.Add(new ViewMap<ScenarioView, ScenarioEditorViewModel>());

            Maps.Add(new ViewMap<InstanceWindowView, NdfClassViewModel>());
            Maps.Add(new ViewMap<ListEditorWindow, ListEditorViewModel>());
            //Maps.Add(new ViewMap<DamageTableView,DamageTableViewModel>());

            Maps.Add(new ViewMap<TradFileView, TradFileViewModel>());

            Maps.Add(new ViewMap<AboutView, AboutViewModel>());

            Maps.Add(new ViewMap<ReferenceSearchResultView, ReferenceSearchResultViewModel>());
            Maps.Add(new ViewMap<ObjectCopyResultView, ObjectCopyResultViewModel>());

            Maps.Add(new ViewMap<VersionManagerView, VersionManagerViewModel>());

            Maps.Add(new ViewMap<UnhandledExceptionView, UnhandledExceptionViewModel>());
        }

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

        public static void ProvideView(ViewModelBase vm, ViewModelBase parentVm = null)
        {
            IViewMap map = Maps.SingleOrDefault(x => x.ViewModelType == vm.GetType());

            if (map == null)
                return;

            var viewInstance = Activator.CreateInstance(map.ViewType) as Window;

            if (viewInstance == null)
                throw new InvalidOperationException(string.Format("Can not create an instance of {0}", map.ViewType));


            if (parentVm != null)
            {
                ViewInstance parent = RegisteredViews.SingleOrDefault(x => x.ViewModel == parentVm);

                if (parent != null)
                {
                    Window parentView = parent.View;

                    if (Application.Current.Windows.OfType<Window>().Any(x => Equals(x, parentView)))
                        viewInstance.Owner = parentView;
                }
            }

            viewInstance.DataContext = vm;

            RegisteredViews.Add(new ViewInstance(viewInstance, vm));

            viewInstance.Show();
        }
    }
}