using ZoneEditor;
using moddingSuite.ZoneEditor.Markers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using moddingSuite.ZoneEditor.ScenarioItems;
using System.Windows.Controls;

namespace moddingSuite.ZoneEditor
{
    public partial class Editor : Form
    {
        public Point LeftClickPoint;
        Outline outline;
        System.Drawing.Image image;
        Redraw redraw
        {
            get
            {
                return new Redraw(delegate()
                {
                    
                    foreach (var c in pictureBox1.Controls)
                    {
                        if (c is Marker)
                        {
                            var a = c as Marker;
                            a.UpdateMarker();
                        }
                        
                    }
                    pictureBox1.Select();
                    pictureBox1.Refresh();
                });
            }
        }
        ZoneEditorData zoneData;
        

        public Editor(ZoneEditorData ze)
        {
            InitializeComponent();
            zoneData = ze;
            buildImage("portWonsan.png");
            this.Name = "ZoneDrawer";
            this.Text = "ZoneDrawer";
            
            


            Graphics g = this.CreateGraphics();
            var zoom = ((float)pictureBox1.Width / (float)image.Width) *
                    (image.HorizontalResolution / g.DpiX);
            PanAndZoom.setZoom(zoom);
            
            pictureBox1.Paint += new PaintEventHandler(OnPaint);
            pictureBox1.MouseDown += PanAndZoom.MouseDown;
            pictureBox1.MouseMove += PanAndZoom.MouseMove;
            
            pictureBox1.MouseUp += PanAndZoom.MouseUp;
            pictureBox1.MouseClick += new MouseEventHandler(pictureBox1_Click);
            pictureBox1.MouseWheel += PanAndZoom.MouseWheel;
            
            pictureBox1.Select();
            //contextMenuStrip1
            PanAndZoom.redraw = redraw;
            contextMenuStrip1.Items[1].Click += ze.AddZone;
            var spawns=contextMenuStrip1.Items[2] as ToolStripMenuItem;
            spawns.DropDown.Items[0].Click += ze.AddLandSpawn;
            spawns.DropDown.Items[1].Click += ze.AddAirSpawn;
            spawns.DropDown.Items[2].Click += ze.AddSeaSpawn;
            var positions = contextMenuStrip1.Items[3] as ToolStripMenuItem;
            positions.DropDown.Items[0].Click += ze.AddCV;
            positions.DropDown.Items[1].Click += ze.AddFOB;


            
            
            
            //outline = new Outline(pictureBox1);
            //pictureBox1.Paint += new PaintEventHandler(outline.paint);
            


        }
        public void addScenarioItem(ScenarioItem item)
        {
            item.attachTo(pictureBox1);
            listBox1.Items.Add(item.ToString());
            
            redraw();
        }
        private void stripClicked(object obj, EventArgs e)
        {
            Console.WriteLine("hello");
        }
        private void pictureBox1_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                LeftClickPoint = e.Location;
                contextMenuStrip1.Show(pictureBox1,e.Location);
            }
            //var scalingFactor = 5100;
            //textBox1.Text = String.Format("{0}", e.X * scalingFactor);
            //textBox2.Text = String.Format("{0}", e.Y * scalingFactor);
            //Console.WriteLine(e.Location);
        }

        private void menuStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            Console.WriteLine(e.ClickedItem.Name);
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
        private void buildImage(string imageString)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var imgStream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".ZoneEditor.Images.portWonsan.png");
            Console.WriteLine(assembly.GetName().Name);
            image = new Bitmap(imgStream);

            }

        
        private void OnPaint(object sender, PaintEventArgs e)
        {
            PanAndZoom.Transform(e);
            e.Graphics.DrawImage(image, 0, 0);
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
        private void splitContainer2_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (zoneData.selectedItem!=null)
            {
                panel1.Controls.Remove(zoneData.selectedItem.propertypanel);
            }
            zoneData.setSelectedItem((string)listBox1.SelectedItem);
            panel1.Controls.Add(zoneData.selectedItem.propertypanel);
            
        }
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void splitContainer2_Panel2_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
