using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using moddingSuite.BL;

namespace moddingSuite.Model.Edata
{
    public class EdataPackage : EdataManager
    {
        private EdataDir _root;
        private Dictionary<EdataContentFile, byte[]> _loadedFiles = new Dictionary<EdataContentFile, byte[]>();

        public EdataPackage(string path)
            : base(path)
        {

        }

        public EdataDir Root
        {
            get { return _root; }
            set
            {
                _root = value;
                OnPropertyChanged("Root");
            }
        }

        public Dictionary<EdataContentFile, byte[]> LoadedFiles
        {
            get { return _loadedFiles; }
        }

        public byte[] GetRawData(EdataContentFile ofFile, bool cacheResult = true)
        {
            byte[] rawData;

            if (LoadedFiles.ContainsKey(ofFile))
            {
                rawData = LoadedFiles[ofFile];
                ofFile.Checksum = MD5.Create().ComputeHash(rawData);
            }
            else
            {
                rawData = base.GetRawData(ofFile);
                if (cacheResult)
                    LoadedFiles.Add(ofFile, rawData);
            }

            return rawData;
        }

        public override void ReplaceFile(EdataContentFile oldFile, byte[] newContent)
        {
            if (LoadedFiles.ContainsKey(oldFile))
                LoadedFiles[oldFile] = newContent;
            else
                LoadedFiles.Add(oldFile, newContent);
        }

    }
}
