﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using moddingSuite.Model.Trad;

namespace moddingSuite.BL
{
    public enum StringType
    {
        Default,
        Utf32
    }

    public class TradManager
    {
        private ObservableCollection<TradEntry> _entries = new ObservableCollection<TradEntry>();
        private readonly StringType _stringType;
        private readonly Encoding _encoder;

        private const ulong GlyphHash = (ulong)0x1 << 63;
        private int _totalRead;

        public StringType StringType => _stringType;
        public TradManager(byte[] data, StringType stringType)
        {
            _stringType = stringType;

            if (_stringType == StringType.Utf32)
            {
                /* Linux file support */
                _encoder = Encoding.UTF32;
            } else
            {
                _encoder = Encoding.Unicode;
            }
            _totalRead = 0;
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
                    ((TradEntry)tradEntry).UserCreated = true;
        }

        private void ParseTradFile(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                uint entryCount = ReadHeader(ms);
                Entries = ReadDictionary(entryCount, ms);

                GetEntryContents(ms);
                Debug.Assert(ms.Capacity == _totalRead);
            }
        }

        private void GetEntryContents(MemoryStream ms)
        {
            int charSize = _stringType == StringType.Utf32 ? 4 : 2;

            foreach (TradEntry entry in Entries)
            {
                ms.Seek(entry.OffsetCont, SeekOrigin.Begin);

                var buffer = new byte[entry.ContLen * charSize];

                _totalRead += ms.Read(buffer, 0, buffer.Length);
                entry.Content = _encoder.GetString(buffer);
            }
        }

        private ObservableCollection<TradEntry> ReadDictionary(uint entryCount, MemoryStream ms)
        {
            var entries = new ObservableCollection<TradEntry>();

            var buffer = new byte[4];

            for (int i = 0; i < entryCount; i++)
            {
                var entry = new TradEntry { OffsetDic = (uint)ms.Position };

                var hashBuffer = new byte[8];

                _totalRead += ms.Read(hashBuffer, 0, hashBuffer.Length);
                entry.Hash = hashBuffer;

                _totalRead += ms.Read(buffer, 0, buffer.Length);
                entry.OffsetCont = BitConverter.ToUInt32(buffer, 0);

                _totalRead += ms.Read(buffer, 0, buffer.Length);
                entry.ContLen = BitConverter.ToUInt32(buffer, 0);

                entries.Add(entry);
            }

            return entries;
        }

        private uint ReadHeader(MemoryStream ms)
        {
            var buffer = new byte[4];

            _totalRead += ms.Read(buffer, 0, buffer.Length);

            if (Encoding.ASCII.GetString(buffer) != "TRAD")
                throw new ArgumentException("No valid Eugen Systems TRAD (*.dic) file.");

            _totalRead += ms.Read(buffer, 0, buffer.Length);

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


                var glyphEntry = Entries.FirstOrDefault(x => BitConverter.ToUInt64(x.Hash, 0).Equals(GlyphHash));

                if (glyphEntry == null)
                {
                    glyphEntry = new TradEntry() {Hash = BitConverter.GetBytes(GlyphHash)};
                    Entries.Add(glyphEntry);
                }

                var orderedEntried = Entries.OrderBy(x => BitConverter.ToUInt64(x.Hash, 0)).ToList();
                orderedEntried.Remove(glyphEntry);
                glyphEntry.Content = BuildGlyphContent(orderedEntried);
                orderedEntried.Add(glyphEntry);

                // Write dictionary
                foreach (TradEntry entry in orderedEntried)
                {
                    entry.OffsetDic = (uint)ms.Position;

                    // Hash
                    ms.Write(entry.Hash, 0, entry.Hash.Length);

                    // Content offset : we dont know it yet
                    ms.Seek(4, SeekOrigin.Current);

                    // Content length
                    buffer = BitConverter.GetBytes(entry.Content.Length);
                    ms.Write(buffer, 0, buffer.Length);
                }

                foreach (TradEntry entry in orderedEntried)
                {
                    entry.OffsetCont = (uint)ms.Position;
                    buffer = _encoder.GetBytes(entry.Content);
                    ms.Write(buffer, 0, buffer.Length);
                }

                foreach (TradEntry entry in orderedEntried)
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

        protected string BuildGlyphContent(List<TradEntry> lst)
        {
            var glyphOccurences = new Dictionary<char, int>();

            foreach (TradEntry e in lst)
                foreach (var chr in e.Content)

                    if (glyphOccurences.ContainsKey(chr))
                        glyphOccurences[chr]++;
                    else
                        glyphOccurences.Add(chr, 1);

            StringBuilder contentBuilder = new StringBuilder();

            var tmp = glyphOccurences.OrderByDescending(x => x.Value);

            foreach (var occurence in glyphOccurences.OrderByDescending(x => x.Value))
                contentBuilder.Append(occurence.Key);

            return contentBuilder.ToString();
        }
    }
}