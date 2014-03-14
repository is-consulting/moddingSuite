using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.Model.Mesh
{
    public class MeshFile
    {
        public MeshHeader Header { get; set; }
        public MeshSubHeader SubHeader { get; set; }

        public ObservableCollection<MeshContentFile> MultiMaterialMeshFiles { get; set; }
    }
}
