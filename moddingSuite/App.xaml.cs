using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using moddingSuite.BL;
using moddingSuite.View;
using moddingSuite.View.DialogProvider;
using moddingSuite.View.Edata;
using moddingSuite.ViewModel.Edata;
using moddingSuite.ViewModel.UnhandledException;

namespace moddingSuite
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            if (path != null)
            {
                var file = Path.Combine(path, $"logging_{DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ff")}.dat");

                Trace.Listeners.Add(new TextWriterTraceListener(file));
                Trace.AutoFlush = true;
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var startApplication = false;

            var settings = SettingsManager.Load();
            var mgr = new EdataManagerView();


            if (settings.InitialSettings)
            {
                var settingsView = new SettingsView();
                settingsView.DataContext = settings;

                var result = settingsView.ShowDialog();
                if (result.GetValueOrDefault(false))
                {
                    if (Directory.Exists(settings.SavePath) && Directory.Exists(settings.WargamePath))
                        settings.InitialSettings = false;

                    SettingsManager.Save(settings);
                    startApplication = true;
                }
            }
            else
            {
                startApplication = true;
            }

            if (startApplication)
            {
                var mainVm = new EdataManagerViewModel();
                mgr.DataContext = mainVm;
                mgr.Show();
            }
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            var vm = new UnhandledExceptionViewModel(e.Exception);
            DialogProvider.ProvideView(vm);

            var excep = e.Exception;

            while (excep != null)
            {
                Trace.TraceError("Unhandeled exception occoured: {0}", e.Exception);
                excep = excep.InnerException;
            }
        }
    }
}