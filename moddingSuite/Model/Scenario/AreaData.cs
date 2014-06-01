using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using moddingSuite.Model.Common;

namespace moddingSuite.Model.Scenario
{
    public class AreaData
    {
        private List<Point3D> _vertices = new List<Point3D>();
        private List<MeshTriangularFace> _faces = new List<MeshTriangularFace>();

        private string _id;

        public List<Point3D> Vertices
        {
            get { return _vertices; }
            set { _vertices = value; }
        }

        public List<MeshTriangularFace> Faces
        {
            get { return _faces; }
            set { _faces = value; }
        }

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }
    }
}
