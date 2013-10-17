using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

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

            var file = Path.Combine(path, string.Format("logging_{0}.dat", DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ff")));

            Trace.Listeners.Add(new TextWriterTraceListener(file));
            Trace.AutoFlush = true;
        }

        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            //MessageBox.Show(e.Exception.ToString());

            Exception excep = e.Exception;

            while (excep != null)
            {
                Trace.TraceError("Unhandeled exception occoured: {0}", e.Exception.ToString());
                excep = excep.InnerException;
            }

        }
    }
}
