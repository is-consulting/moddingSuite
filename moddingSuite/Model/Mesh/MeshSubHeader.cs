using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.Model.Mesh
{
    public class MeshSubHeader
    {
        public uint MeshCount { get; set; }

        public MeshHeaderEntryWithCount Dictionary { get; set; }
        public MeshHeaderEntryWithCount VertexTypeNames { get; set; }
        public MeshHeaderEntryWithCount MeshMaterial { get; set; }
        public MeshHeaderEntryWithCount KeyedMeshSubPart { get; set; }
        public MeshHeaderEntryWithCount KeyedMeshSubPartVectors { get; set; }
        public MeshHeaderEntryWithCount MultiMaterialMeshes { get; set; }
        public MeshHeaderEntryWithCount SingleMaterialMeshes { get; set; }
        public MeshHeaderEntryWithCount Index1DBufferHeaders { get; set; }
        public MeshHeaderEntry Index1DBufferStreams { get; set; }
        public MeshHeaderEntryWithCount Vertex1DBufferHeaders { get; set; }
        public MeshHeaderEntry Vertex1DBufferStreams { get; set; }
    }
}
