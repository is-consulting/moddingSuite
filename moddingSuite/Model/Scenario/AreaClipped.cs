using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.Model.Scenario
{
    public class AreaClipped
    {
        private int _startTriangle;
        private int _triangleCount;

        private int _startVertex;
        private int _vertexCount;

        public int StartTriangle
        {
            get { return _startTriangle; }
            set { _startTriangle = value; }
        }

        public int TriangleCount
        {
            get { return _triangleCount; }
            set { _triangleCount = value; }
        }

        public int VertexCount
        {
            get { return _vertexCount; }
            set { _vertexCount = value; }
        }

        public int StartVertex
        {
            get { return _startVertex; }
            set { _startVertex = value; }
        }
    }
}
