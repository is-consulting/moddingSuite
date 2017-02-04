using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using moddingSuite.BL;
using moddingSuite.Model.Settings;

namespace moddingSuite.View
{
    /// <summary>
    /// Interaktionslogik für SettingsView.xaml
    /// </summary>
    public partial class SettingsView : Window
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CanceButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void WorkSpaceBrowserButtonClick(object sender, RoutedEventArgs e)
        {
            var settings = DataContext as Settings;
            
            if (settings == null)
                return;

            var folderDlg = new FolderBrowserDialog
            {
                SelectedPath = settings.SavePath,
                //RootFolder = Environment.SpecialFolder.MyComputer,
                ShowNewFolderButton = true,
            };

            if (folderDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                settings.SavePath = folderDlg.SelectedPath;
        }

        private void GameSpaceButtonClick(object sender, RoutedEventArgs e)
        {
            var settings = DataContext as Settings;

            if (settings == null)
                return;

            var folderDlg = new FolderBrowserDialog
            {
                SelectedPath = settings.WargamePath,
                //RootFolder = Environment.SpecialFolder.MyComputer,
                ShowNewFolderButton = true,
            };

            if (folderDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                settings.WargamePath = folderDlg.SelectedPath;
        }
    }
}
