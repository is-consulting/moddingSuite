using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace moddingSuite.ZoneEditor.Markers
{
    class CreaterMarker:Marker
    {
        public CreaterMarker()
            : base()
        {
            this.Size = new System.Drawing.Size(5, 5);
            Console.WriteLine("creater create");
        }
        public override void paint(object obj, PaintEventArgs e)
        {
            Console.WriteLine("paint creater");
            e.Graphics.FillRectangle(Brushes.Red, new Rectangle(0, 0, 5, 5));
        }
    }
}
