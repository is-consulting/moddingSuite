using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using moddingSuite.BL;
using moddingSuite.BL.Ndf;
using moddingSuite.Model.Ndfbin.Types;
using moddingSuite.ViewModel.Base;

namespace moddingSuite.Model.Ndfbin
{
    public class NdfObject : ViewModelBase, INdfScriptSerializable
    {
        private readonly ObservableCollection<NdfPropertyValue> _propertyValues =
            new ObservableCollection<NdfPropertyValue>();

        private NdfClass _class;
        private byte[] _data;
        private uint _id;
        private bool _isTopObject;
        public string Name
        {
            get { return string.Format("{0}",Id); }
        }
        public NdfClass Class
        {
            get { return _class; }
            set
            {
                _class = value;
                OnPropertyChanged("Class");
            }
        }

        public byte[] Data
        {
            get { return _data; }
            set
            {
                _data = value;
                OnPropertyChanged("Data");
            }
        }

        public ObservableCollection<NdfPropertyValue> PropertyValues
        {
            get { return _propertyValues; }
        }

        public uint Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("Name");
            }
        }

        public bool IsTopObject
        {
            get { return _isTopObject; }
            set
            {
                _isTopObject = value;
                OnPropertyChanged("IsTopObject");
            }
        }

        public long Offset { get; set; }

        #region INdfScriptSerializable Members

        public byte[] GetNdfText()
        {
            Encoding enc = NdfTextWriter.NdfTextEncoding;

            using (var ms = new MemoryStream())
            {
                byte[] buffer =
                    enc.GetBytes(string.Format("{0}_{1} is {2}\n", NdfTextWriter.InstanceNamePrefix, Id, Class.Name));

                ms.Write(buffer, 0, buffer.Length);
                ms.Write(enc.GetBytes("(\n"), 0, 1);

                var propBuff = new List<byte>();

                foreach (NdfPropertyValue propVal in PropertyValues)
                {
                    if (propVal.Type == NdfType.Unset)
                        continue;

                    propBuff.AddRange(enc.GetBytes(string.Format("{0} =", propVal.Property.Name)));
                    propBuff.AddRange(propVal.Value.GetNdfText());
                    propBuff.AddRange(enc.GetBytes("\n"));

                    buffer = propBuff.ToArray();
                    propBuff.Clear();

                    ms.Write(buffer, 0, buffer.Length);
                }

                ms.Write(enc.GetBytes(")\n"), 0, 1);

                return ms.ToArray();
            }
        }

        #endregion
    }
}