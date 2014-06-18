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
    public partial class ZoneProperty : UserControl
    {
        Zone zone;
        public ZoneProperty(Zone z)
        {
            InitializeComponent();
            zone = z;
            
        }
        public void update()
        {
            comboBox1.SelectedIndex = (int)zone.possession;
            textBox1.Text = string.Format("{0}", zone.value);
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            zone.possession = (Possession)comboBox1.SelectedIndex;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int temp;
            if(int.TryParse(textBox1.Text, out temp)){
                zone.value=temp;
            }
            textBox1.Text = string.Format("{0}", zone.value);
            
        }
    }
}
