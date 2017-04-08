using moddingSuite.ViewModel.Ndf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace moddingSuite.View.Ndfbin.Viewer
{
    /// <summary>
    /// Interaction logic for ArmourDamageTableWindow.xaml
    /// </summary>
    public partial class ArmourDamageTableWindowView : Window
    {
        public ArmourDamageTableWindowView()
        {
            InitializeComponent();
        }
        void DataGrid_LoadingRow(object w, DataGridRowEventArgs e)
        {
            var headers = ((ArmourDamageViewModel)DataContext).RowHeaders;
            if (e.Row.GetIndex() < headers.Count)
            {
                e.Row.Header = headers[e.Row.GetIndex()];
            }
        }
    }
}
