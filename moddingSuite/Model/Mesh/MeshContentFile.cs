using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace moddingSuite.Model.Mesh
{
    public class MeshContentFile
    {
        public uint FileEntrySize { get; set; }
        public Point3D MinBoundingBox { get; set; }
        public Point3D MaxBoundingBox { get; set; }
        public uint Flags { get; set; }
        public ushort MultiMaterialMeshIndex { get; set; }
        public ushort HierarchicalAseModelSkeletonIndex { get; set; }

        public string Name { get; set; }
        public string Path { get; set; }

        public MeshContentFile()
        {
            
        }

    }
}
