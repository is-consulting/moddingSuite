using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using moddingSuite.Model.Trad;

namespace moddingSuite.BL
{
    public class TradManager
    {
        private ObservableCollection<TradEntry> _entries = new ObservableCollection<TradEntry>();

        public TradManager(byte[] data)
        {
            ParseTradFile(data);

            _entries.CollectionChanged += EntriesCollectionChanged;
        }

        public ObservableCollection<TradEntry> Entries
        {
            get { return _entries; }
            private set { _entries = value; }
        }

        private void EntriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                foreach (object tradEntry in e.NewItems)
                    ((TradEntry) tradEntry).UserCreated = true;
        }

        protected void ParseTradFile(byte[] data)
        {
            uint entryCount;

            using (var ms = new MemoryStream(data))
            {
                entryCount = ReadHeader(ms);
                Entries = ReadDictionary(entryCount, ms);

                GetEntryContents(ms);
            }
        }

        protected void GetEntryContents(MemoryStream ms)
        {
            byte[] buffer;

            foreach (TradEntry entry in Entries)
            {
                ms.Seek(entry.OffsetCont, SeekOrigin.Begin);

                buffer = new byte[entry.ContLen*2];

                ms.Read(buffer, 0, buffer.Length);

                entry.Content = Encoding.Unicode.GetString(buffer);
            }
        }

        protected ObservableCollection<TradEntry> ReadDictionary(uint entryCount, MemoryStream ms)
        {
            var entries = new ObservableCollection<TradEntry>();

            byte[] hashBuffer;
            var buffer = new byte[4];

            for (int i = 0; i < entryCount; i++)
            {
                var entry = new TradEntry {OffsetDic = (uint) ms.Position};

                hashBuffer = new byte[8];

                ms.Read(hashBuffer, 0, hashBuffer.Length);
                entry.Hash = hashBuffer;

                ms.Read(buffer, 0, buffer.Length);
                entry.OffsetCont = BitConverter.ToUInt32(buffer, 0);

                ms.Read(buffer, 0, buffer.Length);
                entry.ContLen = BitConverter.ToUInt32(buffer, 0);

                entries.Add(entry);
            }

            return entries;
        }

        protected uint ReadHeader(MemoryStream ms)
        {
            var buffer = new byte[4];

            ms.Read(buffer, 0, buffer.Length);

            if (Encoding.ASCII.GetString(buffer) != "TRAD")
                throw new ArgumentException("No valid Eugen Systems TRAD (*.dic) file.");

            ms.Read(buffer, 0, buffer.Length);

            return BitConverter.ToUInt32(buffer, 0);
        }

        public byte[] BuildTradFile()
        {
            using (var ms = new MemoryStream())
            {
                byte[] buffer = Encoding.ASCII.GetBytes("TRAD");
                ms.Write(buffer, 0, buffer.Length);

                buffer = BitConverter.GetBytes(Entries.Count);
                ms.Write(buffer, 0, buffer.Length);

                // Write dictionary
                foreach (TradEntry entry in Entries)
                {
                    entry.OffsetDic = (uint) ms.Position;

                    // Hash
                    ms.Write(entry.Hash, 0, entry.Hash.Length);

                    // Content offset : we dont know it yet
                    ms.Seek(4, SeekOrigin.Current);

                    // Content length
                    buffer = BitConverter.GetBytes(entry.Content.Length);
                    ms.Write(buffer, 0, buffer.Length);
                }

                foreach (TradEntry entry in Entries)
                {
                    entry.OffsetCont = (uint) ms.Position;
                    buffer = Encoding.Unicode.GetBytes(entry.Content);
                    ms.Write(buffer, 0, buffer.Length);
                }

                foreach (TradEntry entry in Entries)
                {
                    ms.Seek(entry.OffsetDic, SeekOrigin.Begin);

                    ms.Seek(8, SeekOrigin.Current);

                    buffer = BitConverter.GetBytes(entry.OffsetCont);

                    ms.Write(buffer, 0, buffer.Length);
                }

                //Util.Utils.SaveDebug("dicttest.diccmp",ms.ToArray());

                return ms.ToArray();
            }
        }
    }
}