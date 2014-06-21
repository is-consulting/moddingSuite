using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace moddingSuite.ZoneEditor.ScenarioItems.PropertyPanels
{
    public partial class IconProperty : UserControl
    {
        

        Icon icon;
        public IconProperty(Icon s)
        {
            icon = s;
            InitializeComponent();
        }
        public void update()
        {
            comboBox1.SelectedIndex = (int)icon.type;
            textBox1.Text = string.Format("{0}", icon.priority);
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            icon.type = (IconType)comboBox1.SelectedIndex;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int temp;
            if (int.TryParse(textBox1.Text, out temp))
            {
                icon.priority = temp;
            }
            textBox1.Text = string.Format("{0}", icon.priority);
        }
    }
}
