using moddingSuite.ZoneEditor.Markers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Editor
{
    public partial class ZoneEditor : Form
    {
        public Point LeftClickPoint;
        Outline outline;
        Image image;
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

        

        public ZoneEditor(ZoneEditorData ze)
        {
            InitializeComponent();
            buildImage("portWonsan.png");
            this.Name = "ZoneDrawer";
            this.Text = "ZoneDrawer";

            /*this.qqToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_Click);
            tropicThunToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_Click);
            nuclearWinterIsComingToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_Click);
            pungjingValleyToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_Click);
            apocalypseImminentToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_Click);
            wonsanHarborToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_Click);
            paddyFieldToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_Click);
            hopAndGloryToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_Click);
            toughJungleToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_Click);
            chosinReservoirToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_Click);
            jungleLAWToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_Click);
            standoffInBarentsToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_Click);
            bloodyRidgeToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_Click);
            */


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
            /*var ms = new ContextMenuStrip();
            ms.Items.Add("Add...");
            ms.Items[0].Enabled = false;
            ms.Items.Add("Zone");
            ms.Items.Add("Spawn");
            var t = new ToolStripDropDownItem();
            t.
            t.Items.Add("Land");
            t.Items.Add("Air");
            t.Items.Add("Sea");
            ms.Items.Add(t);
            ms.Items.Add("Starting position");
            pictureBox1.ContextMenuStrip=ms;*/
            //pictureBox1.ContextMenuStrip = contextMenuStrip1;
            //contextMenuStrip1.
            /*contextMenuStrip1.Opening += new CancelEventHandler(delegate(object x, CancelEventArgs c)
            {
                //Console.WriteLine("aaa");
                c.Cancel = true;
            });*/
            //contextMenuStrip1.
            //contextMenuStrip1.ItemClicked += new ToolStripItemClickedEventHandler(menuStrip2_ItemClicked);
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
        public void addOutline(Outline o)
        {
            o.attachTo(pictureBox1);
            listBox1.Items.Add(o);
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
            var imgStream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".ZoneEditor.portWonsan.png");
            Console.WriteLine(assembly.GetName().Name);
            image = new Bitmap(imgStream);

            //this.pictureBox1.Size = new System.Drawing.Size(image.Width, image.Height);
            //this.pictureBox1.Image = image;
            //this.pictureBox1.MouseClick += new MouseEventHandler(pictureBox1_Click);
            //this.ClientSize = new System.Drawing.Size(image.Width, image.Height + this.pictureBox1.Location.Y);

            //this.groupBox1.Location = new System.Drawing.Point(0, image.Height + this.pictureBox1.Location.Y);
            //this.ClientSize = new System.Drawing.Size(image.Width, image.Height + this.pictureBox1.Location.Y + this.groupBox1.Size.Height);
            //this.groupBox1.Size = new System.Drawing.Size(image.Width, groupBox1.Size.Height);
        }

        private void ToolStripMenuItem_Click(object sender, EventArgs a)
        {
            var item = sender as ToolStripMenuItem;
            String imageString = "";
            if (item.Name.Equals("hopAndGloryToolStripMenuItem"))
            {
                imageString = "1.png";
            }
            if (item.Name.Equals("apocalypseImminentToolStripMenuItem"))
            {
                imageString = "2.png";
            }
            if (item.Name.Equals("nuclearWinterIsComingToolStripMenuItem"))
            {
                imageString = "3.png";
            }
            if (item.Name.Equals("cliffHangerToolStripMenuItem"))
            {
                imageString = "4.png";
            }
            if (item.Name.Equals("floodsToolStripMenuItem"))
            {
                imageString = "5.png";
            }
            if (item.Name.Equals("anotherD-DayInParadiseToolStripMenuItem"))
            {
                imageString = "6.png";
            }
            if (item.Name.Equals("pungjingValleyToolStripMenuItem"))
            {
                imageString = "7.png";
            }
            if (item.Name.Equals("wonsanHarborToolStripMenuItem"))
            {
                imageString = "8.png";
            }
            if (item.Name.Equals("wonsanHarborToolStripMenuItem"))
            {
                imageString = "8.png";
            }
            if (item.Name.Equals("paddyFieldToolStripMenuItem"))
            {
                imageString = "9.png";
            }
            if (item.Name.Equals("gunboatDiplomacyToolStripMenuItem"))
            {
                imageString = "10.png";
            }
            if (item.Name.Equals("backToInchonToolStripMenuItem"))
            {
                imageString = "11.png";
            }
            if (item.Name.Equals("jungleLAWToolStripMenuItem"))
            {
                imageString = "12.png";
            }
            if (item.Name.Equals("tropicThunToolStripMenuItem"))
            {
                imageString = "13.png";
            }
            if (item.Name.Equals("chosinReservoirToolStripMenuItem"))
            {
                imageString = "14.png";
            }
            if (item.Name.Equals("bloodyRidgeToolStripMenuItem"))
            {
                imageString = "15.png";
            }
            if (item.Name.Equals("koreaRocksToolStripMenuItem"))
            {
                imageString = "16.png";
            }
            if (item.Name.Equals("AMazeInJapanToolStripMenuItem"))
            {
                imageString = "17.png";
            }
            if (item.Name.Equals("38thParallelToolStripMenuItem"))
            {
                imageString = "18.png";
            }
            if (item.Name.Equals("toughJungleToolStripMenuItem"))
            {
                imageString = "22.png";
            }



            if (imageString.Length > 0)
            {
                buildImage(imageString);
            }
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
            
        }
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }
    }
    
}
