using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace moddingSuite.ZoneEditor.Markers
{
    class VertexMarker:Marker
    {
        MouseEventHandler dragEventHandler;
        public Brush Colour;
        public VertexMarker():base()
        {
            Colour = Brushes.Blue;
            this.Location = new System.Drawing.Point(30, 30);
            this.Name = "pictureBox1";
            this.Size = new System.Drawing.Size(10, 10);
            //this.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.TabIndex = 1;
            this.TabStop = false;

            this.MouseDown += new MouseEventHandler(OnMouseDown);
            this.MouseUp += new MouseEventHandler(OnMouseUp);
            dragEventHandler = new MouseEventHandler(OnDrag);

        }
        public override void paint(object obj, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Colour, new Rectangle(0, 0, 10, 10));
        }
        public void OnMouseDown(object obj, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            this.Parent.MouseMove += dragEventHandler;
            this.MouseMove += dragEventHandler;
            //Console.WriteLine(e.Location);
        }
        public void OnMouseUp(object obj, MouseEventArgs e)
        {
            //this.Parent.DoDragDrop(this, DragDropEffects.Move);
            if (e.Button != MouseButtons.Left) return;
            this.Parent.MouseMove -= dragEventHandler;
            this.MouseMove -= dragEventHandler;
            //Console.WriteLine(e.Location);
        }
        public void OnDrag(object obj, MouseEventArgs e)
        {

            Point p = e.Location;
            p.Offset(-Size.Width / 2 - 1, -Size.Height / 2 - 1);
            if (obj.Equals(this))
            {
                p.Offset(Location);
            }
            Parent.Invalidate();
            this.Location = p;
            position = getPosition();
        }
    }
}
