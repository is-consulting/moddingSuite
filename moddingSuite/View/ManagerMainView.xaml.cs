using System.Windows;
using moddingSuite.ViewModel.Edata;

namespace moddingSuite.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ManagerMainViewModel();
        }
    }
}