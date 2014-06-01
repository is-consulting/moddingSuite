using System.Windows.Media.Media3D;

namespace moddingSuite.Model.Common
{
    public struct MeshTriangularFace
    {
        public int Point1;
        public int Point2;
        public int Point3;

        public MeshTriangularFace(int p1, int p2, int p3)
        {
            Point1 = p1;
            Point2 = p2;
            Point3 = p3;
        }
    }
}