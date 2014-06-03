using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using moddingSuite.Model.Common;

namespace moddingSuite.Model.Scenario
{
    public class Area
    {
        private AreaContent _content;
        private Point3D _attachmentPoint;
        private string _name;
        private int _id;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public Point3D AttachmentPoint
        {
            get { return _attachmentPoint; }
            set { _attachmentPoint = value; }
        }

        public AreaContent Content
        {
            get { return _content; }
            set { _content = value; }
        }
    }
}
