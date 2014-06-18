using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Editor
{
    class Marker:Control
    {
        MouseEventHandler dragEventHandler ;
        Point position;
        public Marker()
        {
            this.Paint += new PaintEventHandler(paint);
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
        private void paint(object obj, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Blue, new Rectangle(0, 0, 10, 10));
        }
        public void OnMouseDown(object obj,MouseEventArgs e)
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
            
            Point p=e.Location;
            p.Offset(-Size.Width/2-1,-Size.Height/2-1);
            if (obj.Equals(this))
            {
                p.Offset(Location);
            }
            Parent.Invalidate();
            this.Location = p;
            position = getPosition();
        }
        public void setPosition(Point point)
        {
            
            position = point;
            UpdateMarker();
        }
        public Point getPosition()
        {

            var loc = Location;
            loc.Offset(Size.Width / 2+1, Size.Height / 2+1);
            return PanAndZoom.fromLocalToGlobal(loc);
        }
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            MouseEventArgs mouse = e as MouseEventArgs;

            if (mouse.Button == MouseButtons.Left)
            {
                
            }
        }


        internal void UpdateMarker()
        {
            var loc = PanAndZoom.fromGlobalToLocal(position);
            loc.Offset(-Size.Width / 2, -Size.Height / 2);
            Location = loc;
        }
    }
}
