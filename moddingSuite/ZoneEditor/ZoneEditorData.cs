using moddingSuite.Model.Scenario;
using moddingSuite.Geometry;
using moddingSuite.ViewModel.Ndf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using moddingSuite.Model.Ndfbin.Types.AllTypes;
using moddingSuite.ZoneEditor;
using moddingSuite.ZoneEditor.ScenarioItems;
using System.Windows.Media.Media3D;
using moddingSuite.Model.Ndfbin;


namespace ZoneEditor
{
    public class ZoneEditorData
    {
        int spawnNumber = 1;
        int startPosNumber = 1;
        int zoneNumber = 0;
        //List<Outline> zoneOutlines = new List<Outline>();
        List<ScenarioItem> scenarioItems = new List<ScenarioItem>();
        List<Zone> zones = new List<Zone>();
        public ScenarioItem selectedItem;
        Editor editor;
        ScenarioFile scenarioFile;
        NdfBinary data;

        public ZoneEditorData(ScenarioFile sf, string path)
        {

            scenarioFile = sf;
            editor = new Editor(this, path);

            data = sf.NdfBinary;
            foreach (var area in sf.ZoneData.AreaManagers[1])
            {
                //var nodes=Geometry.getOutline(area.Content);
                //var zone = new Outline(nodes);
                //zoneOutlines.Add(zone);
                zoneNumber++;
                var zone = new Zone(editor, area);
                scenarioItems.Add(zone);
                zones.Add(zone);
                editor.addScenarioItem(zone);
                Console.WriteLine("name:");
                Console.WriteLine(area.Name);
                Console.WriteLine("en name");
                /*Console.WriteLine("zone\n");
                foreach (var c in area.Content.ClippedAreas)
                {
                    Console.Write("vertices=[");
            var scen = area.Content;
            foreach (var v in scen.Vertices.GetRange(c.StartVertex,c.VertexCount))
            {
                Console.WriteLine("{0:G},{1:G},{2:G};", (int)v.X, (int)v.Y, (int)v.Center);
            }
            Console.WriteLine("]");

            Console.Write("tri=[");
            foreach (var v in scen.Triangles.GetRange(c.StartTriangle,c.TriangleCount))
            {
                Console.WriteLine("{0},{1},{2};", (int)v.Point1, (int)v.Point2, (int)v.Point3);
            }
            Console.WriteLine("]");
                }*/

            }

            doZoneProperties();
            Application.EnableVisualStyles();
            Application.Run(editor);
            //Application.SetCompatibleTextRenderingDefault(false);

        }
        public ScenarioItem setSelectedItem(string s)
        {
            if (s == null) return null;
            if (selectedItem != null) selectedItem.setSelected(false);
            selectedItem = scenarioItems.Find(x => x.ToString().Equals(s));
            selectedItem.setSelected(true);
            return selectedItem;
        }
        public void Save()
        {
            Console.WriteLine("saving");
            //Zones
            scenarioFile.ZoneData.AreaManagers[1].Clear();
            var i = 0;
            foreach (var zone in zones)
            {
                var area = zone.getArea();
                area.Id = i++;
                scenarioFile.ZoneData.AreaManagers[1].Add(area);
            }

            //delete old Markups
            purgeData();
            //Markups
            var j = 1;
            scenarioItems.ForEach(x => x.buildNdf(data, ref j));
            //data.Classes.First().Object.Manager.CreateInstanceOf
        }
        private void purgeData()
        {
            string[] toBePurged ={"TGameDesignItem",
                                    "TGameDesignAddOn_CommandPoints",
                                    "TGameDesignAddOn_StartingPoint",
                                    "TGameDesignAddOn_ReinforcementLocation",
                                    "TGameDesignAddOn_MaritimeCorridor",
                                    "TGameDesignAddOn_AerialCorridor",
                                    "TGameDesignAddOn_StartingCommandUnit",
                                    "TGameDesignAddOn_StartingFOB"
                                 };
            foreach (var str in toBePurged)
            {
                if (!data.Classes.Any(x => x.Name.Equals(str))) continue;
                var viewModel = data.Classes.Single(x => x.Name.Equals(str));
                //foreach (var inst in viewModel.Instances)
                while (viewModel.Instances.Count > 0)
                {
                    var inst = viewModel.Instances.Last();
                    if (inst == null)
                        return;

                    viewModel.Manager.DeleteInstance(inst);

                    viewModel.Instances.Remove(inst);
                }
            }
            var list = data.Classes.First().Instances.First().PropertyValues.First().Value as NdfCollection;
            list.Clear();

        }
        private void doZoneProperties()
        {
            var list = data.Classes.First().Instances.First().PropertyValues.First().Value as NdfCollection;
            foreach (var item in list)
            {
                var reference = item.Value as NdfObjectReference;
                if (reference.Instance == null) continue;
                var designItem = reference.Instance;
                var position = designItem.PropertyValues.First(x => x.Property.Name.Equals("Position")).Value as NdfVector;
                var rotation = designItem.PropertyValues.First(x => x.Property.Name.Equals("Rotation")).Value as NdfSingle;
                var addonReference = designItem.PropertyValues.First(x => x.Property.Name.Equals("AddOn")).Value as NdfObjectReference;

                var scale = designItem.PropertyValues.FirstOrDefault(x => x.Property.Name.Equals("Scale"));

                float s = 1;

                if (scale != null)
                {
                    var sc = scale.Value as NdfVector;
                    if (sc != null)
                        s = (float)((Point3D)sc.Value).X;
                }

                var addon = addonReference.Instance;

                var q = (Point3D)position.Value;
                var p = new AreaVertex();
                p.X = (float)q.X;
                p.Y = (float)q.Y;
                Zone zone;

                zone = zones.FirstOrDefault(x =>
                    Geometry.isInside(p, x.getRawOutline())
                    );


                if (addon.Class.Name.Equals("TGameDesignAddOn_CommandPoints") && zone != null)
                {
                    var pos = addon.PropertyValues.First(x => x.Property.Name.Equals("Points")).Value as NdfInt32;
                    if (pos == null)
                    {
                        zone.value = 0;
                    }
                    else
                    {
                        zone.value = (int)pos.Value;
                    }
                }


                if (addon.Class.Name.Equals("TGameDesignAddOn_StartingPoint") && zone != null)
                {



                    var pos = addon.PropertyValues.First(x => x.Property.Name.Equals("AllianceNum")).Value as NdfInt32;
                    if (pos == null)
                    {
                        zone.possession = (Possession)0;
                    }
                    else
                    {
                        zone.possession = (Possession)pos.Value;
                    }



                }
                if (addon.Class.Name.Equals("TGameDesignAddOn_ReinforcementLocation") && zone != null)
                {

                    var spawn = new Spawn(Geometry.convertPoint(q), (float)rotation.Value, s, spawnNumber++, SpawnType.Land);
                    editor.addScenarioItem(spawn);
                    scenarioItems.Add(spawn);
                }
                if (addon.Class.Name.Equals("TGameDesignAddOn_MaritimeCorridor") && zone != null)
                {

                    var spawn = new Spawn(Geometry.convertPoint(q), (float)rotation.Value, s, spawnNumber++, SpawnType.Sea);
                    editor.addScenarioItem(spawn);
                    scenarioItems.Add(spawn);
                }
                if (addon.Class.Name.Equals("TGameDesignAddOn_AerialCorridor") && zone != null)
                {

                    var spawn = new Spawn(Geometry.convertPoint(q), (float)rotation.Value, s, spawnNumber++, SpawnType.Air);
                    editor.addScenarioItem(spawn);
                    scenarioItems.Add(spawn);
                }
                if (addon.Class.Name.Equals("TGameDesignAddOn_StartingCommandUnit") && zone != null)
                {

                    var prop = addon.PropertyValues.First(x => x.Property.Name.Equals("AllocationPriority"));
                    int prio = 0;
                    if (!(prop.Value is NdfNull))
                    {
                        prio = (int)((NdfInt32)prop.Value).Value;
                    }
                    var startPos = new Icon(Geometry.convertPoint(q), startPosNumber++, IconType.CV, prio);
                    editor.addScenarioItem(startPos);
                    scenarioItems.Add(startPos);
                }
                if (addon.Class.Name.Equals("TGameDesignAddOn_StartingFOB") && zone != null)
                {
                    var prop = addon.PropertyValues.First(x => x.Property.Name.Equals("AllocationPriority"));
                    int prio = 0;
                    if (!(prop.Value is NdfNull))
                    {
                        prio = (int)((NdfInt32)prop.Value).Value;
                    }
                    var startPos = new Icon(Geometry.convertPoint(q), startPosNumber++, IconType.FOB, prio);
                    editor.addScenarioItem(startPos);
                    scenarioItems.Add(startPos);
                }





                //Console.WriteLine(rotation);
            }
        }
        public EventHandler AddZone
        {
            get { return new EventHandler(addZone); }
        }
        public EventHandler AddLandSpawn
        {
            get { return new EventHandler(addLandSpawn); }
        }
        public EventHandler AddAirSpawn
        {
            get { return new EventHandler(addAirSpawn); }
        }
        public EventHandler AddSeaSpawn
        {
            get { return new EventHandler(addSeaSpawn); }
        }
        public EventHandler AddCV
        {
            get { return new EventHandler(addCV); }
        }
        public EventHandler AddFOB
        {
            get { return new EventHandler(addFOB); }
        }
        private void addZone(object obj, EventArgs e)
        {
            var zone = new Zone(editor, editor.LeftClickPoint, zoneNumber++);
            scenarioItems.Add(zone);
            zones.Add(zone);
            editor.addScenarioItem(zone, true);

        }
        private void addLandSpawn(object obj, EventArgs e)
        {
            var spawn = new Spawn(PanAndZoom.fromLocalToGlobal(editor.LeftClickPoint), spawnNumber, SpawnType.Land);
            scenarioItems.Add(spawn);
            editor.addScenarioItem(spawn, true);
            //Console.WriteLine("add land spawn");
        }
        private void addAirSpawn(object obj, EventArgs e)
        {
            var spawn = new Spawn(PanAndZoom.fromLocalToGlobal(editor.LeftClickPoint), spawnNumber, SpawnType.Air);
            scenarioItems.Add(spawn);
            editor.addScenarioItem(spawn, true);

            // Console.WriteLine("add air spawn");
        }
        private void addSeaSpawn(object obj, EventArgs e)
        {
            var spawn = new Spawn(PanAndZoom.fromLocalToGlobal(editor.LeftClickPoint), spawnNumber++, SpawnType.Sea);
            scenarioItems.Add(spawn);
            editor.addScenarioItem(spawn, true);

        }
        private void addCV(object obj, EventArgs e)
        {
            var icon = new Icon(PanAndZoom.fromLocalToGlobal(editor.LeftClickPoint), startPosNumber++, IconType.CV);
            scenarioItems.Add(icon);
            editor.addScenarioItem(icon, true);

        }
        private void addFOB(object obj, EventArgs e)
        {
            var icon = new Icon(PanAndZoom.fromLocalToGlobal(editor.LeftClickPoint), startPosNumber++, IconType.CV);
            scenarioItems.Add(icon);
            editor.addScenarioItem(icon, true);
        }
        public void deleteItem(object o, EventArgs e)
        {
            scenarioItems.Remove(selectedItem);
            if (selectedItem is Zone)
            {
                zones.Remove((Zone)selectedItem);
            }
            editor.deleteItem(selectedItem);
        }

    }
}
