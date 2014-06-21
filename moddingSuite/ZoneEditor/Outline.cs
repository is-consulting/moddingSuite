using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using moddingSuite.ZoneEditor.Markers;
using moddingSuite.ZoneEditor.ScenarioItems;
using moddingSuite.Model.Scenario;
using moddingSuite.Geometry;
namespace ZoneEditor
{
    public class Outline
    {
        
        List<Point> nodes = new List<Point>();
        List<VertexMarker> markers = new List<VertexMarker>();
        List<CreaterMarker> creaters = new List<CreaterMarker>();
        Control parent;
        public Possession possession;
        private PaintEventHandler paintEvent;
        public Outline(List<Point> nodes)
        {
            
            this.nodes = nodes;
            foreach (var n in nodes)
            {
                var marker = new VertexMarker();
                marker.setPosition(n);
                marker.MouseClick += new MouseEventHandler(deleteMarker);
                markers.Add(marker);

                marker.BringToFront();

                var c = new CreaterMarker();
                c.MouseClick += new MouseEventHandler(createMarker);
                //parent.Controls.Add(c);
                creaters.Add(c);
            }
            paintEvent = new PaintEventHandler(paint);
        }
        public Outline(Point center){
            
            var sideLength = 50;
            center.Offset(-sideLength / 2, -sideLength/2);
            nodes.Add(PanAndZoom.fromLocalToGlobal(center));


            center.Offset(0, sideLength);
            nodes.Add(PanAndZoom.fromLocalToGlobal(center));

            center.Offset(sideLength, 0);
            nodes.Add(PanAndZoom.fromLocalToGlobal(center));

            center.Offset(0, -sideLength);
            nodes.Add(PanAndZoom.fromLocalToGlobal(center));
            
            //parent.Controls.Add(this);
            //BringToFront();


            
            foreach(var n in nodes){
                var marker = new VertexMarker();
                marker.setPosition(n);
                marker.MouseClick += new MouseEventHandler(deleteMarker);
                markers.Add(marker);

                marker.BringToFront();

                var c = new CreaterMarker();
                c.MouseClick += new MouseEventHandler(createMarker);
                //parent.Controls.Add(c);
                creaters.Add(c);
            }
            paintEvent = new PaintEventHandler(paint);
            
        }
        public void attachTo(Control c)
        {
            this.parent = c;
            parent.Paint += paintEvent;
            c.Controls.AddRange(markers.ToArray());
            c.Controls.AddRange(creaters.ToArray());
        }
        public void detachFrom(Control c)
        {
            
            parent.Paint -= paintEvent;
            foreach (var m in markers)
            {
                c.Controls.Remove(m);
            }
            foreach (var cr in creaters)
            {
                c.Controls.Remove(cr);
            }
            this.parent = null;
        }
        public void paint(object sen, PaintEventArgs e)
        {
            
            var pos=markers.Select(x => x.getPosition()).ToList();
            pos.Add(pos.First());

            for (int i = 0; i < pos.Count-1; i++)
            {
                var p1=pos.ElementAt(i);
                var p2=pos.ElementAt(i+1);
                Point p = new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
                creaters.ElementAt(i).setPosition(p);
            }
                //e.Graphics.DrawArc(Pens.Red, new Rectangle(20, 20, 400, 200), 10, 170);
                //e.Graphics.DrawLines(Pens.AliceBlue, x.ToArray());
                //Console.WriteLine(pos.ToList());
            Color c=new Color();
            switch (possession)
            {
                case Possession.Redfor:
                    c = Color.FromArgb(80, 255, 0, 0);
                    break;
                case Possession.Bluefor:
                    c = Color.FromArgb(80, 0, 0, 255);
                    break;
                case Possession.Neutral:
                    c = Color.FromArgb(80, 255, 255, 255);
                    break;
                    
            }
            Brush b = new SolidBrush(c);
            PanAndZoom.Transform(e);
            e.Graphics.FillPolygon(b, pos.ToArray());
            
            
        }
        public void deleteMarker(object obj,MouseEventArgs e){
            
            if (e.Button != MouseButtons.Right || markers.Count <= 3) return;
            VertexMarker m = (VertexMarker)obj;
            parent.Controls.Remove(m);

            var i = markers.IndexOf(m);
            markers.RemoveAt(i);

            if (creaters.Count==i-1)
            {
                
            }

            parent.Controls.Remove(creaters.ElementAt(i));
            creaters.RemoveAt(i);
            parent.Invalidate();

        }
        public void createMarker(object obj, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            var creater = (CreaterMarker)obj;


            var marker = new VertexMarker();
            marker.setPosition(creater.getPosition());
            marker.MouseClick += new MouseEventHandler(deleteMarker);
            markers.Insert(creaters.IndexOf(creater)+1,marker);
            parent.Controls.Add(marker);
            marker.BringToFront();

            var c = new CreaterMarker();
            c.MouseClick += new MouseEventHandler(createMarker);
            parent.Controls.Add(c);
            creaters.Insert(creaters.IndexOf(creater)+1, c);

            parent.Invalidate();

        }
        public void setSelected(bool selected)
        {
            markers.ForEach(x => x.Visible = selected);
            creaters.ForEach(x => x.Visible = selected);
        }
        public List<AreaVertex> getOutline()
        {
            var list = new List<AreaVertex>();
            foreach (var marker in markers)
            {
                var p = Geometry.convertPoint(marker.getPosition());
                var av=new AreaVertex();
                av.X = (float)p.X;
                av.Y = (float)p.Y;
                av.Z = (float)Geometry.groundLevel;
                av.W = (float)Geometry.groundLevel;
                list.Add(av);
            }
            return list;
        }
    }
    
}
