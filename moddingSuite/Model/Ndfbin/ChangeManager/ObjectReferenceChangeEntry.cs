using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.Model.Ndfbin.ChangeManager
{
    public class ObjectReferenceChangeEntry : ChangeEntryBase
    {
        private uint _classId;
        private uint _instanceId;

        public ObjectReferenceChangeEntry(NdfPropertyValue affectedPropertyValue, uint classId, uint instanceId)
            : base(affectedPropertyValue)
        {
            ClassId = classId;
            InstanceId = instanceId;
        }

        public uint ClassId
        {
            get { return _classId; }
            set
            {
                _classId = value;
                OnPropertyChanged("ClassId");
            }
        }

        public uint InstanceId
        {
            get { return _instanceId; }
            set
            {
                _instanceId = value;
                OnPropertyChanged("InstanceId");
            }
        }
    }
}
