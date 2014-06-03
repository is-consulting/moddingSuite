using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.Model.Scenario
{
    public class AreaFile
    {
        private List<AreaColletion>  _areaManagers = new List<AreaColletion>();

        public List<AreaColletion> AreaManagers
        {
            get { return _areaManagers; }
            set { _areaManagers = value; }
        }
    }
}
