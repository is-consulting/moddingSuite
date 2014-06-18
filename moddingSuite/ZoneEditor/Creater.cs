using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Editor
{
    class Creater:Control
    {
        Point position;
        public Creater()
        {
            this.Paint += new PaintEventHandler(paint);
            this.Location = new System.Drawing.Point(30, 30);
            this.Name = "pictureBox1";
            this.Size = new System.Drawing.Size(5, 5);
            //this.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.TabIndex = 1;
            this.TabStop = false;

            //this.MouseDown += new MouseEventHandler(OnMouseDown);
            //this.MouseUp += new MouseEventHandler(OnMouseUp);
            //dragEventHandler = new MouseEventHandler(OnDrag);
            
            
        }
        private void paint(object obj, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Red, new Rectangle(0, 0, 5, 5));
        }
        public void setPosition(Point point)
        {
            
            position = point;
            UpdateCreater();
            /*p.Offset(-Size.Width / 2, -Size.Height / 2);
            var k = new Point((int)(p.X * Form1.zoom), (int)(p.Y * Form1.zoom));
            p.Offset((int)(Form1.imgx), (int)(Form1.imgy));*/

            //this.Location = p;
        }
        public Point getPosition()
        {
            var loc = Location;
            loc.Offset(Size.Width / 2, Size.Height / 2);
            return PanAndZoom.fromLocalToGlobal(loc);
        }

        internal void UpdateCreater()
        {
            
            var loc = PanAndZoom.fromGlobalToLocal(position);
            loc.Offset(-Size.Width / 2, -Size.Height / 2);
            Location = loc;
        }
    }
}
