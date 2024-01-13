using System.Windows;

namespace moddingSuite.View.Ndfbin
{
    /// <summary>
    /// Interaction logic for NdfDetailView.xaml
    /// </summary>
    public partial class NdfbinView : Window
    {
        public NdfbinView()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            //selects the top class if there are classes to select to avoid having nothing selected
            if (classGrid.Items.Count > 0) 
            {
                classGrid.SelectedItem = classGrid.Items[0];
            }
            SetInstanceGridItem();
        }

        private void classGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SetInstanceGridItem();
        }
        private void SetInstanceGridItem()
        {
            // scrolls to currently selected instance
            if (instanceGrid.Items.Count == 0)
            {
                return;
            }
            instanceGrid.ScrollIntoView(instanceGrid.Items[instanceGrid.Items.Count - 1]);
            instanceGrid.UpdateLayout();
            instanceGrid.ScrollIntoView(instanceGrid.SelectedItem);
        }
    }
}
