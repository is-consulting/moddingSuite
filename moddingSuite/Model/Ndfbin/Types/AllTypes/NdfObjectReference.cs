using System;
using System.Collections.Generic;
using System.Linq;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfObjectReference : NdfValueWrapper
    {
        private readonly bool _isDead;
        private NdfClass _class;
        private uint _instanceId;

        public NdfObjectReference(NdfClass cls, uint instance, bool isDead = false)
            : base(NdfType.ObjectReference)
        {
            Class = cls;
            InstanceId = instance;

            _isDead = isDead;
        }

        public NdfClass Class
        {
            get { return _class; }
            protected set
            {
                _class = value;
                OnPropertyChanged("Class");
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

        public NdfObject Instance
        {
            get
            {
                if (Class == null) return null;

                return Class.Instances.FirstOrDefault(o => o.Id == InstanceId);
            }
            set
            {
                if (!Class.Instances.Contains(value))
                    InstanceId = Class.Instances.First().Id;
                else
                    InstanceId = value.Id;

                OnPropertyChanged("Instance");
                OnPropertyChanged("InstanceId");
            }
        }

        public override string ToString()
        {
            if (Class == null)
                return string.Format("Class does not exist : {0}", InstanceId);

            return string.Format("{0} : {1} ({2}) - {3}", Class.Id, InstanceId, Instance.IsTopObject, Class.Name);
        }

        public override byte[] GetBytes()
        {
            var refereceData = new List<byte>();

            refereceData.AddRange(BitConverter.GetBytes(InstanceId));

            refereceData.AddRange(_isDead ? new byte[] { 0xFF, 0xFF, 0xFF, 0xFF } : BitConverter.GetBytes(Class.Id));

            return refereceData.ToArray();
        }

        public override byte[] GetNdfText()
        {
            throw new NotImplementedException();
        }
    }
}