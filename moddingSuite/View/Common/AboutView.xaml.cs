using System.Windows;
using System.Windows.Input;

namespace moddingSuite.View.Common
{
    /// <summary>
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView : Window
    {
        public AboutView()
        {
            InitializeComponent();
        }

        private void ButtonClick1(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ImageMouseUp1(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.gnu.org/licenses/gpl-3.0.en.html");
        }
    }
}
