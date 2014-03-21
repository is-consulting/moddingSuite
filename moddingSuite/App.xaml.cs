using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using moddingSuite.View.DialogProvider;
using moddingSuite.ViewModel.Edata;
using moddingSuite.ViewModel.UnhandledException;
using moddingSuite.ViewModel.VersionManager;

namespace moddingSuite
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            if (path != null)
            {
                var file = Path.Combine(path, string.Format("logging_{0}.dat", DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ff")));

                Trace.Listeners.Add(new TextWriterTraceListener(file));
                Trace.AutoFlush = true;
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainVm = new EdataManagerViewModel();
            DialogProvider.ProvideView(mainVm);

            //var versionVm = new VersionManagerViewModel(mainVm);
            //DialogProvider.ProvideView(versionVm);
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            var vm = new UnhandledExceptionViewModel(e.Exception);
            DialogProvider.ProvideView(vm);

            Exception excep = e.Exception;

            while (excep != null)
            {
                Trace.TraceError("Unhandeled exception occoured: {0}", e.Exception.ToString());
                excep = excep.InnerException;
            }
        }
    }
}
