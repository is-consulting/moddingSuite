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
    public class Spawn:ScenarioItem
    {
        VertexMarker head;
        VertexMarker source;
        int arrowLength = 250000;
        private SpawnType _type;
        public SpawnType type{
            get{return _type;}
            set{
                _type=value;
                try { head.Parent.Refresh(); }
                catch (NullReferenceException e) { }

                ((SpawnProperty) propertypanel).update();
            }
        }
            
        public Spawn(Point p,float rot,float scale,int i,SpawnType t)
        {
            if (scale == null) scale = 1;
            propertypanel = new SpawnProperty(this);
            type = t;
            head = new VertexMarker();
            head.Colour = Brushes.Yellow;
            head.setPosition(p);

            source = new VertexMarker();
            source.Colour = Brushes.Yellow;
            p.Offset(-(int)(scale * arrowLength * Math.Cos(rot) / Geometry.Geometry.scaleFactor), -(int)(scale * arrowLength * Math.Sin(rot) / Geometry.Geometry.scaleFactor));
            source.setPosition(p);
            Name = string.Format("Spawn {0}", i);
            
            setSelected(false);

        }
        public override void attachTo(System.Windows.Forms.Control c)
        {
            c.Controls.Add(head);
            c.Controls.Add(source);
            c.Paint += new PaintEventHandler(paint);
        }
        public void paint(object sen, PaintEventArgs e)
        {
            PanAndZoom.Transform(e);
            Pen p = new Pen(Brushes.White, 10);
            switch (type){
                case SpawnType.Land:
                    p=new Pen(Brushes.White,10);
                    break;
                case SpawnType.Air:
                    p = new Pen(Brushes.Blue, 5);
                    break;
                case SpawnType.Sea:
                    p = new Pen(Brushes.White, 5);
                    break;
            }    
                
            e.Graphics.DrawLine(p, source.getPosition(), head.getPosition());
        }
        public override void setSelected(bool selected)
        {
            head.Visible = selected;
            source.Visible = selected;
        }
        public override void buildNdf(ViewModel.Ndf.NdfEditorMainViewModel data, ref int i)
        {
            string name = "";
            string ranking = "";
            switch (type)
            {
                case SpawnType.Land:
                    name="TGameDesignAddOn_ReinforcementLocation";
                    ranking = "ReinforcementLocations";

                    break;
                case SpawnType.Sea:
                    name="TGameDesignAddOn_MaritimeCorridor";
                    ranking = "MaritimeCorridors";
                    break;
                case SpawnType.Air:
                    name="TGameDesignAddOn_AerialCorridor";
                    ranking = "AerialCorridors";
                    break;

            }
            var spawnPoint = createNdfObject(data, name);
            /*var nameProperty = getProperty(spawnPoint, "Name");
            nameProperty.Value = getAutoName(data, i++);
            var rankingProperty = getProperty(spawnPoint, "Ranking");
            rankingProperty.Value = getString(data, ranking);
            var guidProperty = getProperty(spawnPoint, "GUID");
            rankingProperty.Value = new NdfGuid(Guid.NewGuid());
            var allianceProperty = getProperty(spawnPoint, "NumAlliance");
            allianceProperty.Value = new NdfInt32(1);*/

            var designItem = createNdfObject(data, "TGameDesignItem");
            var list = data.Classes.First().Instances.First().PropertyValues.First().Value as NdfCollection;
            var ci = new CollectionItemValueHolder(new NdfObjectReference(designItem.Class, designItem.Id), data.NdfBinary);
            list.Add(ci);

            var positionProperty = getProperty(designItem, "Position");
            var hp=head.getPosition();
            var p = Geometry.Geometry.convertPoint(hp);
            positionProperty.Value = new NdfVector(p);

            var rotationProperty = getProperty(designItem, "Rotation");
            var sp = source.getPosition();
            var rot = Math.Atan2(hp.Y - sp.Y, hp.X - sp.X);
            rotationProperty.Value = new NdfSingle((float)rot);

            var scaleProperty = getProperty(designItem, "Scale");
            var length = Math.Sqrt((hp.Y - sp.Y) * (hp.Y - sp.Y) + (hp.X - sp.X) * (hp.X - sp.X));
            float xScale = (float)(length * Geometry.Geometry.scaleFactor) / arrowLength;
            scaleProperty.Value = new NdfVector(new System.Windows.Media.Media3D.Point3D(xScale, 1, 1));

            var addOnProperty = getProperty(designItem, "AddOn");
            addOnProperty.Value = new NdfObjectReference(spawnPoint.Class, spawnPoint.Id);
        }
    }
}
