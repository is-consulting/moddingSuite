using moddingSuite.Model.Ndfbin;
using moddingSuite.Model.Ndfbin.Types.AllTypes;
using moddingSuite.ZoneEditor.Markers;
using moddingSuite.ZoneEditor.ScenarioItems.PropertyPanels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZoneEditor;

namespace moddingSuite.ZoneEditor.ScenarioItems
{
    public class Icon:ScenarioItem
    {
        VertexMarker position;
        Image image;
        private IconType _type;
        private int _priority;
        public int priority
        {
            get { return _priority; }
            set
            {
                _priority = value;
                ((IconProperty)propertypanel).update();
                updateImage();
                if (position.Parent != null)
                    position.Parent.Refresh();
            }
        }
        public IconType type
        {
            get { return _type; }
            set {
                _type = value;
                ((IconProperty)propertypanel).update();
                updateImage();
                if (position.Parent!=null)
                position.Parent.Refresh();
            }
        }
        public Icon(Point p,int i,IconType t,int prio=1)
        {
            
            position = new VertexMarker();
            position.Colour = Brushes.Green;
            position.setPosition(p);
            propertypanel = new IconProperty(this);
            type = t;
            Name = string.Format("Start Position {0}", i);
            setSelected(false);
            priority = prio;
        }
        public override void attachTo(System.Windows.Forms.Control c)
        {
            c.Controls.Add(position);
            c.Paint += paintEvent;
        }
        public override void detachFrom(System.Windows.Forms.Control c)
        {
            c.Controls.Remove(position);
            c.Paint -= paintEvent;
        }
        private void updateImage()
        {
            string typeString="";
            switch (type)
            {
                case IconType.CV:
                    typeString = "CV.png";
                    break;
                case IconType.FOB:
                    typeString = "FOB.png";
                    break;
            }
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var imgStream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".ZoneEditor.Images." + typeString);
            image = new Bitmap(imgStream);
        }
        protected override void paint(object o,PaintEventArgs e)
        {
            //PanAndZoom.Transform(e);
            //var p=position.getPosition();
            e.Graphics.ResetTransform();
            var p = PanAndZoom.fromGlobalToLocal(position.getPosition());
            var size=20;
            
            e.Graphics.TranslateTransform(p.X,p.Y) ;
            
            e.Graphics.DrawImage(image, new Rectangle(-size/2,-size/2,size,size));
        }
        public override void setSelected(bool selected)
        {
            position.Visible = selected;
        }
        public override void buildNdf(NdfBinary data,ref int i)
        {
            //return;

            string name = "";
            string ranking = "";
            switch (type)
            {
                case IconType.CV:
                    name = "TGameDesignAddOn_StartingCommandUnit";
                    ranking = "StartingCommandUnits";

                    break;
                case IconType.FOB:
                    name = "TGameDesignAddOn_StartingFOB";
                    ranking = "StartingFOB";
                    break;
            }
            var spawnPoint = createNdfObject(data, name);
            /*var nameProperty = getProperty(spawnPoint, "Name");
            nameProperty.Value = getAutoName(data, i++);
            var rankingProperty = getProperty(spawnPoint, "Ranking");
            rankingProperty.Value = getString(data, ranking);
            var guidProperty = getProperty(spawnPoint, "GUID");
            rankingProperty.Value = new NdfGuid(Guid.NewGuid());*/
            var allocationProperty = getProperty(spawnPoint, "AllocationPriority");
            //NOT RIGHT
            allocationProperty.Value = new NdfInt32(priority);

            var designItem = createNdfObject(data, "TGameDesignItem");
            var list = data.Classes.First().Instances.First().PropertyValues.First().Value as NdfCollection;
            var ci = new CollectionItemValueHolder(new NdfObjectReference(designItem.Class, designItem.Id), data);
            list.Add(ci);

            var positionProperty = getProperty(designItem, "Position");
            var hp = position.getPosition();
            var p = Geometry.Geometry.convertPoint(hp);
            positionProperty.Value = new NdfVector(p);

            var rotationProperty = getProperty(designItem, "Rotation");

            rotationProperty.Value = new NdfSingle(0f);

            var addOnProperty = getProperty(designItem, "AddOn");
            addOnProperty.Value = new NdfObjectReference(spawnPoint.Class, spawnPoint.Id);
        }
    }
}
