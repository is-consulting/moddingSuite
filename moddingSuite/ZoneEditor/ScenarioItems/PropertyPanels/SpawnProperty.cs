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
    public partial class SpawnProperty : UserControl
    {
        Spawn spawn;
        public SpawnProperty(Spawn s)
        {
            spawn = s;
            InitializeComponent();
        }
        public void update()
        {
            comboBox1.SelectedIndex = (int)spawn.type;
            //textBox1.Text = string.Format("{0}", zone.value);
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            spawn.type = (SpawnType)comboBox1.SelectedIndex;
        }
    }
}
