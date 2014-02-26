using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using moddingSuite.Compressing;
using moddingSuite.Model.Ndfbin;
using moddingSuite.Model.Ndfbin.ChangeManager;
using moddingSuite.Model.Ndfbin.Types;
using moddingSuite.Model.Ndfbin.Types.AllTypes;

namespace moddingSuite.BL
{
    public class NdfbinManager
    {
        public const string InstanceNamePrefix = "public";
        public static readonly Encoding NdfTextEncoding = Encoding.Unicode;

        public NdfbinManager(byte[] fileData)
        {
            FileData = fileData;
        }

        public byte[] FileData { get; protected set; }
        public NdfFooter Footer { get; protected set; }
        public NdfHeader Header { get; protected set; }
        public ObservableCollection<NdfClass> Classes { get; protected set; }
        public ObservableCollection<NdfStringReference> Strings { get; protected set; }
        public ObservableCollection<NdfTranReference> Trans { get; protected set; }

        public HashSet<uint> Topo { get; protected set; }
        public HashSet<uint> Impr { get; protected set; }
        public HashSet<uint> Expr { get; protected set; }

        public ChangeManager ChangeManager { get; protected set; }

        protected byte[] ContentData { get; set; }

        public bool HasChanges { get; set; }

        public bool HasUnkownTypes { get; set; }

        public List<NdfObject> AllInstances { get; protected set; }

        public bool HasUnkownType { get; set; }

        //protected List<byte[]> _unknownTypes = new List<byte[]>();
        //protected List<int> _unknownTypesCount = new List<int>();

        public byte[] GetContent()
        {
            if (Header == null)
                Header = ReadHeader();

            return ContentData;
        }

        public void Initialize()
        {
            ChangeManager = new ChangeManager();

            Header = ReadHeader();
            Footer = ReadFooter();
            ReadClasses();
            ReadProperties();

            ReadStrings();
            ReadTrans();

            Topo = ReadTopologyList("TOPO");
            Impr = ReadTopologyList("IMPR");
            Expr = ReadTopologyList("EXPR");

            AllInstances = ReadObjects();
        }

        protected HashSet<uint> ReadTopologyList(string lst)
        {
            var topo = new HashSet<uint>();

            NdfFooterEntry topoEntry = Footer.Entries.Single(x => x.Name == lst);

            //TODO: int cast is a bit too hacky here, solution needed
            using (var ms = new MemoryStream(ContentData, (int) topoEntry.Offset - 40, (int) topoEntry.Size))
            {
                var buffer = new byte[4];
                while (ms.Position < ms.Length)
                {
                    ms.Read(buffer, 0, buffer.Length);
                    topo.Add(BitConverter.ToUInt32(buffer, 0));
                }
            }

            return topo;
        }

        protected void ReadClasses()
        {
            var classes = new ObservableCollection<NdfClass>();

            NdfFooterEntry classEntry = Footer.Entries.Single(x => x.Name == "CLAS");

            //TODO: int cast is a bit too hacky here, solution needed
            using (var ms = new MemoryStream(ContentData, (int) classEntry.Offset - 40, (int) classEntry.Size))
            {
                int i = 0;
                var buffer = new byte[4];
                while (ms.Position < ms.Length)
                {
                    var nclass = new NdfClass(this) {Offset = ms.Position, Id = i};

                    ms.Read(buffer, 0, buffer.Length);
                    int strLen = BitConverter.ToInt32(buffer, 0);

                    var strBuffer = new byte[strLen];
                    ms.Read(strBuffer, 0, strBuffer.Length);

                    nclass.Name = Encoding.GetEncoding("ISO-8859-1").GetString(strBuffer);

                    i++;
                    classes.Add(nclass);
                }
            }

            Classes = classes;
        }

        protected void ReadProperties()
        {
            NdfFooterEntry propEntry = Footer.Entries.Single(x => x.Name == "PROP");

            //TODO: int cast is a bit too hacky here, solution needed
            using (var ms = new MemoryStream(ContentData, (int) propEntry.Offset - 40, (int) propEntry.Size))
            {
                int i = 0;
                var buffer = new byte[4];
                while (ms.Position < ms.Length)
                {
                    var property = new NdfProperty {Offset = ms.Position, Id = i};

                    ms.Read(buffer, 0, buffer.Length);
                    int strLen = BitConverter.ToInt32(buffer, 0);

                    var strBuffer = new byte[strLen];
                    ms.Read(strBuffer, 0, strBuffer.Length);

                    property.Name = Encoding.GetEncoding("ISO-8859-1").GetString(strBuffer);

                    ms.Read(buffer, 0, buffer.Length);

                    NdfClass cls = Classes.Single(x => x.Id == BitConverter.ToInt32(buffer, 0));
                    property.Class = cls;

                    cls.Properties.Add(property);

                    i++;
                }
            }
        }

        protected void ReadStrings()
        {
            var strings = new ObservableCollection<NdfStringReference>();

            NdfFooterEntry stringEntry = Footer.Entries.Single(x => x.Name == "STRG");

            //TODO: int cast is a bit too hacky here, solution needed
            using (var ms = new MemoryStream(ContentData, (int) stringEntry.Offset - 40, (int) stringEntry.Size))
            {
                int i = 0;
                var buffer = new byte[4];
                while (ms.Position < ms.Length)
                {
                    var nstring = new NdfStringReference {Offset = ms.Position, Id = i};

                    ms.Read(buffer, 0, buffer.Length);
                    int strLen = BitConverter.ToInt32(buffer, 0);

                    var strBuffer = new byte[strLen];
                    ms.Read(strBuffer, 0, strBuffer.Length);

                    nstring.Value = Encoding.GetEncoding("ISO-8859-1").GetString(strBuffer);

                    i++;
                    strings.Add(nstring);
                }
            }

            Strings = strings;
        }

        protected void ReadTrans()
        {
            var trans = new ObservableCollection<NdfTranReference>();

            NdfFooterEntry stringEntry = Footer.Entries.Single(x => x.Name == "TRAN");

            //TODO: int cast is a bit too hacky here, solution needed
            using (var ms = new MemoryStream(ContentData, (int) stringEntry.Offset - 40, (int) stringEntry.Size))
            {
                int i = 0;
                var buffer = new byte[4];
                while (ms.Position < ms.Length)
                {
                    var ntran = new NdfTranReference {Offset = ms.Position, Id = i};

                    ms.Read(buffer, 0, buffer.Length);
                    int strLen = BitConverter.ToInt32(buffer, 0);

                    var strBuffer = new byte[strLen];
                    ms.Read(strBuffer, 0, strBuffer.Length);

                    ntran.Value = Encoding.GetEncoding("ISO-8859-1").GetString(strBuffer);

                    i++;
                    trans.Add(ntran);
                }
            }

            Trans = trans;
        }

        protected uint ReadChunk()
        {
            NdfFooterEntry chnk = Footer.Entries.Single(x => x.Name == "OBJE");

            var buffer = new byte[4];

            using (var ms = new MemoryStream(ContentData, (int) chnk.Offset - 40, (int) chnk.Size))
            {
                ms.Read(buffer, 0, buffer.Length);
                ms.Read(buffer, 0, buffer.Length);

                return BitConverter.ToUInt32(buffer, 0);
            }
        }

        protected List<NdfObject> ReadObjects()
        {
            var objects = new List<NdfObject>();

            NdfFooterEntry objEntry = Footer.Entries.Single(x => x.Name == "OBJE");

            //TODO: int cast is a bit too hacky here, solution needed
            using (var ms = new MemoryStream(ContentData, (int) objEntry.Offset - 40, (int) objEntry.Size))
            {
                long[] instanceOffsets = GetInstanceOffsets(ms).ToArray();

                byte[] buffer;
                long size;

                for (uint i = 0; i < instanceOffsets.Length; i++)
                {
                    if (i == instanceOffsets.Length - 1)

                        if ((ms.Length - instanceOffsets[i] - 4) >= 0)
                            size = ms.Length - instanceOffsets[i] - 4;
                        else
                            size = ms.Length - instanceOffsets[i];
                    else
                        size = instanceOffsets[i + 1] - instanceOffsets[i] - 4;

                    long objOffset = ms.Position;

                    buffer = new byte[size];
                    ms.Read(buffer, 0, buffer.Length);
                    ms.Seek(4, SeekOrigin.Current);

                    if (buffer.Length > 0)
                    {
                        NdfObject obj = ParseObject(buffer, i, objOffset);
                        objects.Add(obj);
                    }
                }
            }

            return objects;
        }

        protected NdfObject ParseObject(byte[] data, uint index, long objOffset)
        {
            var instance = new NdfObject {Id = index, Data = data, Offset = objOffset};

            if (Topo.Contains(index))
                instance.IsTopObject = true;

            using (var ms = new MemoryStream(data))
            {
                var buffer = new byte[4];

                ms.Read(buffer, 0, buffer.Length);
                int classId = BitConverter.ToInt32(buffer, 0);

                NdfClass cls = instance.Class = Classes.SingleOrDefault(x => x.Id == classId);

                if (cls == null)
                    throw new InvalidDataException("Object without class found.");

                cls.Instances.Add(instance);

                NdfPropertyValue prop;
                bool triggerBreak;

                // Read properties
                while (ms.Position < ms.Length)
                {
                    prop = new NdfPropertyValue(instance);
                    instance.PropertyValues.Add(prop);

                    ms.Read(buffer, 0, buffer.Length);
                    prop.Property = cls.Properties.Single(x => x.Id == BitConverter.ToInt32(buffer, 0));

                    NdfValueWrapper res = ReadValue(ms, out triggerBreak, prop);

                    //prop.Type = res.Key;
                    prop.Value = res;

                    if (triggerBreak)
                        break;
                }

                AddEmptyProperties(instance);
            }

            return instance;
        }

        private static void AddEmptyProperties(NdfObject instance)
        {
            foreach (NdfProperty property in instance.Class.Properties)
                if (instance.PropertyValues.All(x => x.Property != property))
                    instance.PropertyValues.Add(new NdfPropertyValue(instance)
                                                    {Property = property, Value = new NdfNull(0)});
        }

        /// <summary>
        /// Reads the value of a Property inside a object instance.
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="triggerBreak"></param>
        /// <param name="prop"></param>
        /// <returns>A NdfValueWrapper Instance.</returns>
        protected NdfValueWrapper ReadValue(MemoryStream ms, out bool triggerBreak, NdfPropertyValue prop)
        {
            var buffer = new byte[4];
            uint contBufferlen;
            NdfValueWrapper value;
            triggerBreak = false;

            ms.Read(buffer, 0, buffer.Length);
            NdfType type = NdfTypeManager.GetType(buffer);

            if (type == NdfType.Reference)
            {
                ms.Read(buffer, 0, buffer.Length);
                type = NdfTypeManager.GetType(buffer);
            }

            switch (type)
            {
                case NdfType.WideString:
                case NdfType.List:
                case NdfType.MapList:
                    ms.Read(buffer, 0, buffer.Length);
                    contBufferlen = BitConverter.ToUInt32(buffer, 0);
                    break;
                default:
                    contBufferlen = NdfTypeManager.SizeofType(type);
                    break;
            }

            if (type == NdfType.Unknown)
            {
                //var t = _unknownTypes.SingleOrDefault(x => Utils.ByteArrayCompare(x, buffer));

                //if (t == default(byte[]))
                //{
                //    _unknownTypes.Add(buffer);
                //    _unknownTypesCount.Add(1);
                //}
                //else
                //    _unknownTypesCount[_unknownTypes.IndexOf(t)]++;

                triggerBreak = true;
                return new NdfUnkown(buffer, ms.Position);
            }

            if (type == NdfType.List || type == NdfType.MapList)
            {
                NdfCollection lstValue;

                if (type == NdfType.List)
                    lstValue = new NdfCollection(ms.Position);
                else
                    lstValue = new NdfMapList(ms.Position);

                CollectionItemValueHolder res;

                for (int i = 0; i < contBufferlen; i++)
                {
                    if (type == NdfType.List)
                        res = new CollectionItemValueHolder(ReadValue(ms, out triggerBreak, prop), this,
                                                            prop.InstanceOffset);
                    else
                        res = new CollectionItemValueHolder(
                            new NdfMap(
                                new MapValueHolder(ReadValue(ms, out triggerBreak, prop), this, prop.InstanceOffset),
                                new MapValueHolder(ReadValue(ms, out triggerBreak, prop), this, prop.InstanceOffset),
                                ms.Position, this),
                            this, prop.InstanceOffset);

                    lstValue.Add(res);

                    if (triggerBreak)
                        break;
                }

                value = lstValue;
            }
            else if (type == NdfType.Map)
            {
                value = new NdfMap(
                    new MapValueHolder(ReadValue(ms, out triggerBreak, prop), this, prop.InstanceOffset),
                    new MapValueHolder(ReadValue(ms, out triggerBreak, prop), this, prop.InstanceOffset),
                    ms.Position, this);
            }
            else
            {
                var contBuffer = new byte[contBufferlen];
                ms.Read(contBuffer, 0, contBuffer.Length);

                if (prop != null)
                    prop.ValueData = contBuffer;

                value = NdfTypeManager.GetValue(contBuffer, type, this, ms.Position - contBuffer.Length);
            }

            return value;
        }

        /// <summary>
        /// Gets the offset of every single instance.
        /// </summary>
        /// <param name="ms"></param>
        /// <returns>A collection with the offset for every object instance in the Ndfbin file.</returns>
        private IEnumerable<long> GetInstanceOffsets(MemoryStream ms)
        {
            const byte ab = 0xAB;

            var offsets = new List<long> {0};

            byte abCount = 0;
            byte[] buffer;

            while (ms.Position < ms.Length)
            {
                buffer = new byte[1];
                ms.Read(buffer, 0, buffer.Length);

                if (buffer[0] == ab)
                {
                    abCount++;
                    if (abCount == 4)
                    {
                        if (ms.Position >= ms.Length)
                            continue;
                        offsets.Add(ms.Position);
                        abCount = 0;
                    }
                }
                else if (abCount > 0)
                    abCount = 0;
            }

            ms.Position = 0;

            return offsets;
        }

        /// <summary>
        /// Reads the header data and sets the ContentData Property to the correct content.
        /// </summary>
        /// <returns>A valid instance of the Headerfile.</returns>
        protected NdfHeader ReadHeader()
        {
            var header = new NdfHeader();

            using (var ms = new MemoryStream(FileData))
            {
                ms.Seek(12, SeekOrigin.Begin);
                var buffer = new byte[4];

                ms.Read(buffer, 0, buffer.Length);
                header.IsCompressedBody = BitConverter.ToInt32(buffer, 0) == 128;

                ms.Read(buffer, 0, 4);
                header.FileSizeUncompressedMinusE0 = BitConverter.ToInt32(buffer, 0);

                ms.Seek(12, SeekOrigin.Current);

                ms.Read(buffer, 0, 4);
                header.FileSizeUncompressed = BitConverter.ToInt32(buffer, 0);

                ms.Seek(4, SeekOrigin.Current);

                if (header.IsCompressedBody)
                {
                    ms.Read(buffer, 0, 4);
                    header.UncompressedContentSize = BitConverter.ToInt32(buffer, 0);
                }

                buffer = new byte[FileData.Length - ms.Position];

                ms.Read(buffer, 0, buffer.Length);

                if (header.IsCompressedBody)
                    ContentData = Compressor.Decomp(buffer);
                else
                    ContentData = buffer;
            }

            return header;
        }

        /// <summary>
        /// Reads the footer data which is the Ndfbin Dictionary.
        /// </summary>
        /// <returns></returns>
        protected NdfFooter ReadFooter()
        {
            // Footer is 224 bytes
            const int footerLength = 224;
            var footer = new NdfFooter();

            using (var ms = new MemoryStream(ContentData, ContentData.Length - footerLength, footerLength))
            {
                var qwdbuffer = new byte[8];
                var dwdbufer = new byte[4];

                ms.Read(qwdbuffer, 0, qwdbuffer.Length);
                footer.Header = Encoding.ASCII.GetString(qwdbuffer);

                while (ms.Position < ms.Length)
                {
                    var entry = new NdfFooterEntry();

                    ms.Read(dwdbufer, 0, dwdbufer.Length);
                    entry.Name = Encoding.ASCII.GetString(dwdbufer);

                    ms.Seek(4, SeekOrigin.Current);

                    ms.Read(qwdbuffer, 0, qwdbuffer.Length);
                    entry.Offset = BitConverter.ToInt64(qwdbuffer, 0);

                    ms.Read(qwdbuffer, 0, qwdbuffer.Length);
                    entry.Size = BitConverter.ToInt64(qwdbuffer, 0);

                    footer.Entries.Add(entry);
                }
            }

            return footer;
        }

        public NdfObject CreateInstanceOf(NdfClass cls, bool isTopLevelInstance = true)
        {
            var newId = (uint) AllInstances.Count();

            var inst = new NdfObject {Class = cls, Id = newId};

            AddEmptyProperties(inst);

            AllInstances.Add(inst);

            if (isTopLevelInstance)
                Topo.Add(inst.Id);

            return inst;
        }

        public void DeleteInstance(NdfObject inst)
        {
            AllInstances.Remove(inst);
            NdfClass cls = inst.Class;
            cls.Instances.Remove(inst);

            if (Topo.Contains(inst.Id))
                Topo.Remove(inst.Id);
        }

        /// <summary>
        /// Build a valid NdfbinFile with the current ContentData.
        /// </summary>
        /// <param name="compress"></param>
        /// <returns></returns>
        public byte[] BuildNdfFile(bool compress)
        {
            byte[] contentData = RecompileContent();

            ContentData = contentData;

            //Utils.SaveDebug("listaddtest", contentData);

            var header = new byte[] {0x45, 0x55, 0x47, 0x30, 0x00, 0x00, 0x00, 0x00, 0x43, 0x4E, 0x44, 0x46};
            byte[] compressed = compress ? new byte[] {0x80, 0x00, 0x00, 0x00} : new byte[4];

            byte[] blockSize = BitConverter.GetBytes((long) contentData.Length + 40 - 0xE0);
            byte[] blockSizeE0 = BitConverter.GetBytes((long) (contentData.Length + 40));

            byte[] blockSize3 = BitConverter.GetBytes((contentData.Length));

            byte[] contBuffer = ContentData;

            using (var ms = new MemoryStream())
            {
                ms.Write(header, 0, header.Length);
                ms.Write(compressed, 0, compressed.Length);

                ms.Write(blockSize, 0, blockSize.Length);
                ms.Write(BitConverter.GetBytes((long) 40), 0, 8);
                ms.Write(blockSizeE0, 0, blockSizeE0.Length);

                if (compress)
                {
                    ms.Write(blockSize3, 0, blockSize3.Length);

                    contBuffer = Compressor.Comp(contentData);
                }

                ms.Write(contBuffer, 0, contBuffer.Length);

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Recompiling
        /// </summary>
        /// <returns></returns>
        protected byte[] RecompileObj()
        {
            var objectPart = new List<byte>();

            byte[] objSep = {0xAB, 0xAB, 0xAB, 0xAB};

            foreach (NdfObject instance in AllInstances)
            {
                objectPart.AddRange(BitConverter.GetBytes(instance.Class.Id));

                foreach (NdfPropertyValue propertyValue in instance.PropertyValues)
                {
                    bool valid;

                    if (propertyValue.Type == NdfType.Unset)
                        continue;

                    byte[] valueBytes = propertyValue.Value.GetBytes(out valid);

                    if (propertyValue.Value.Type == NdfType.Unset || !valid)
                        continue;

                    objectPart.AddRange(BitConverter.GetBytes(propertyValue.Property.Id));

                    if (propertyValue.Value.Type == NdfType.ObjectReference ||
                        propertyValue.Value.Type == NdfType.TransTableReference)
                        objectPart.AddRange(BitConverter.GetBytes((uint) NdfType.Reference));

                    objectPart.AddRange(BitConverter.GetBytes((uint) propertyValue.Value.Type));
                    objectPart.AddRange(valueBytes);
                }

                objectPart.AddRange(objSep);
            }

            return objectPart.ToArray();
        }

        protected byte[] RecompileTopo()
        {
            bool realComp = true;

            using (var ms = new MemoryStream())
            {
                if (realComp)
                {
                    List<NdfObject> topInsts = AllInstances.Where(x => x.IsTopObject).ToList();

                    var writeInsts = new HashSet<NdfObject>();

                    foreach (NdfObject inst in topInsts)
                    {
                        if (writeInsts.Contains(inst))
                            continue;

                        writeInsts.Add(inst);

                        int nextItemId = topInsts.IndexOf(inst) + 1;

                        if (topInsts.Count > nextItemId && topInsts[nextItemId].Class != inst.Class)
                        {
                            IEnumerable<NdfObject> othersOfSameClass =
                                topInsts.GetRange(nextItemId, topInsts.Count - nextItemId).Where(
                                    x => x.Class == inst.Class && !writeInsts.Contains(x));

                            foreach (NdfObject o in othersOfSameClass)
                                writeInsts.Add(o);
                        }
                    }

                    foreach (NdfObject instance in writeInsts)
                    {
                        byte[] buffer = BitConverter.GetBytes(instance.Id);
                        ms.Write(buffer, 0, buffer.Length);
                    }
                }
                else
                {
                    foreach (uint i in Topo)
                    {
                        byte[] buffer = BitConverter.GetBytes(i);
                        ms.Write(buffer, 0, buffer.Length);
                    }
                }

                return ms.ToArray();
            }
        }

        protected byte[] RecompileChnk()
        {
            return BitConverter.GetBytes(AllInstances.Count);
        }

        protected byte[] RecompileStrTable(IEnumerable<NdfStringReference> table)
        {
            var strBlock = new List<byte>();

            foreach (NdfStringReference stringReference in table)
            {
                strBlock.AddRange(BitConverter.GetBytes(stringReference.Value.Length));
                strBlock.AddRange(Encoding.GetEncoding("ISO-8859-1").GetBytes(stringReference.Value));
            }

            return strBlock.ToArray();
        }

        protected byte[] RecompileContent()
        {
            byte[] buffer;

            var footer = new NdfFooter();

            using (var ms = new MemoryStream())
            {
                buffer = RecompileObj();
                footer.AddEntry("OBJE", ms.Position + 40, buffer.Length);
                ms.Write(buffer, 0, buffer.Length);

                buffer = RecompileTopo();
                footer.AddEntry("TOPO", ms.Position + 40, buffer.Length);
                ms.Write(buffer, 0, buffer.Length);

                buffer = RecompileChnk();
                footer.AddEntry("CHNK", ms.Position + 40, buffer.Length);
                ms.Write(buffer, 0, buffer.Length);

                buffer = GetSingleNdfBlockData("CLAS");
                footer.AddEntry("CLAS", ms.Position + 40, buffer.Length);
                ms.Write(buffer, 0, buffer.Length);

                buffer = GetSingleNdfBlockData("PROP");
                footer.AddEntry("PROP", ms.Position + 40, buffer.Length);
                ms.Write(buffer, 0, buffer.Length);

                buffer = RecompileStrTable(Strings);
                footer.AddEntry("STRG", ms.Position + 40, buffer.Length);
                ms.Write(buffer, 0, buffer.Length);

                buffer = RecompileStrTable(Trans);
                footer.AddEntry("TRAN", ms.Position + 40, buffer.Length);
                ms.Write(buffer, 0, buffer.Length);

                buffer = GetSingleNdfBlockData("IMPR");
                footer.AddEntry("IMPR", ms.Position + 40, buffer.Length);
                ms.Write(buffer, 0, buffer.Length);

                buffer = GetSingleNdfBlockData("EXPR");
                footer.AddEntry("EXPR", ms.Position + 40, buffer.Length);
                ms.Write(buffer, 0, buffer.Length);

                buffer = footer.GetBytes();
                ms.Write(buffer, 0, buffer.Length);

                buffer = ms.ToArray();
            }

            Footer = footer;

            return buffer;
        }

        public byte[] GetSingleNdfBlockData(string blockName)
        {
            if (string.IsNullOrEmpty(blockName))
                throw new ArgumentException("blockName");

            using (var ms = new MemoryStream(ContentData))
            {
                NdfFooterEntry footerEntry = Footer.Entries.Single(x => x.Name == blockName);
                var buffer = new byte[footerEntry.Size];

                ms.Seek(footerEntry.Offset - 40, SeekOrigin.Begin);
                ms.Read(buffer, 0, buffer.Length);

                return buffer;
            }
        }

        public byte[] CreateNdfScript()
        {
            using (var ms = new MemoryStream())
            {
                byte[] buffer = NdfTextEncoding.GetBytes(string.Format("// Handwritten by enohka \n// For real\n\n\n"));

                ms.Write(buffer, 0, buffer.Length);

                foreach (NdfObject instance in AllInstances.Where(x => x.IsTopObject))
                {
                    buffer = instance.GetNdfText();
                    ms.Write(buffer, 0, buffer.Length);
                }

                return ms.ToArray();
            }
        }
    }
}