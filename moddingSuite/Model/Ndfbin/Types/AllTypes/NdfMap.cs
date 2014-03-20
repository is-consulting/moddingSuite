using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using moddingSuite.BL;
using moddingSuite.BL.Ndf;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfMap : NdfFlatValueWrapper
    {
        private readonly List<NdfType> _typeSelection = new List<NdfType>();
        private MapValueHolder _key;

        private NdfType _keyType = NdfType.Unset;
        private NdfType _valueType = NdfType.Unset;

        public NdfMap(MapValueHolder key, MapValueHolder value, NdfBinary mgr)
            : base(NdfType.Map, value)
        {
            Key = key;
            Manager = mgr;

            _typeSelection.AddRange(NdfTypeManager.GetTypeSelection());
        }

        public NdfBinary Manager { get; protected set; }

        public NdfType KeyType
        {
            get { return _keyType; }
            set
            {
                _keyType = value;
                GetValueForType(true);
                OnPropertyChanged(() => KeyType);
            }
        }

        public NdfType ValueType
        {
            get { return _valueType; }
            set
            {
                _valueType = value;
                GetValueForType(false);
                OnPropertyChanged(() => ValueType);
            }
        }

        public List<NdfType> TypeSelection
        {
            get { return _typeSelection; }
        }

        public bool IsKeyNull
        {
            get { return Key.Value == null; }
        }

        public bool IsValueNull
        {
            get { return ((MapValueHolder) Value).Value == null; }
        }

        public MapValueHolder Key
        {
            get { return _key; }
            set
            {
                _key = value;
                OnPropertyChanged("Key");
            }
        }

        private void GetValueForType(bool keyOrValue)
        {
            if (keyOrValue)
                Key =
                    new MapValueHolder(
                        NdfTypeManager.GetValue(new byte[NdfTypeManager.SizeofType(KeyType)], KeyType, Manager), Manager);
            else
                Value =
                    new MapValueHolder(
                        NdfTypeManager.GetValue(new byte[NdfTypeManager.SizeofType(ValueType)], ValueType, Manager), Manager);

            OnPropertyChanged("IsKeyNull");
            OnPropertyChanged("IsValueNull");
        }

        public override byte[] GetBytes()
        {

            if (Key.Value == null || ((MapValueHolder) Value).Value == null)
                return new byte[0];

            var mapdata = new List<byte>();

            List<byte> key = Key.Value.GetBytes().ToList();
            List<byte> value = ((MapValueHolder) Value).Value.GetBytes().ToList();

            if (Key.Value.Type == NdfType.ObjectReference || Key.Value.Type == NdfType.TransTableReference)
                mapdata.AddRange(BitConverter.GetBytes((uint) NdfType.Reference));

            mapdata.AddRange(BitConverter.GetBytes((uint) Key.Value.Type));
            mapdata.AddRange(key);

            if (((MapValueHolder) Value).Value.Type == NdfType.ObjectReference ||
                ((MapValueHolder) Value).Value.Type == NdfType.TransTableReference)
                mapdata.AddRange(BitConverter.GetBytes((uint) NdfType.Reference));

            mapdata.AddRange(BitConverter.GetBytes((uint) ((MapValueHolder) Value).Value.Type));
            mapdata.AddRange(value);

            return mapdata.ToArray();
        }

        public override byte[] GetNdfText()
        {
            Encoding end = NdfTextWriter.NdfTextEncoding;
            var data = new List<byte>();

            data.AddRange(end.GetBytes("(\n"));


            data.AddRange((Key).Value.GetNdfText());

            data.AddRange(end.GetBytes(",\n"));


            data.AddRange(end.GetBytes(")\n"));

            return data.ToArray();
        }

        public override string ToString()
        {
            return string.Format("Map: {0} : {1}", Key.Value, ((MapValueHolder) Value).Value);
        }
    }
}