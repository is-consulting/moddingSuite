using System.Collections.ObjectModel;
using System.IO;
using moddingSuite.Compressing;
using moddingSuite.Model.Ndfbin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using moddingSuite.Model.Ndfbin.Types;
using moddingSuite.Model.Ndfbin.Types.AllTypes;

namespace moddingSuite.BL.Ndf
{
    public class NdfbinReader : INdfReader
    {
        public NdfBinary Read(byte[] data)
        {
            var ndf = new NdfBinary();

            using (var ms = new MemoryStream(data))
            {
                ndf.Header = ReadHeader(ms);

                if (ndf.Header.IsCompressedBody)
                {
                    using (var uncompStream = new MemoryStream())
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                        var headBuffer = new byte[ndf.Header.HeaderSize];
                        ms.Read(headBuffer, 0, headBuffer.Length);
                        uncompStream.Write(headBuffer, 0, headBuffer.Length);

                        ms.Seek((long)ndf.Header.HeaderSize, SeekOrigin.Begin);

                        var buffer = new byte[4];
                        ms.Read(buffer, 0, buffer.Length);
                        uint compressedblocklen = BitConverter.ToUInt32(buffer, 0);

                        var contentBuffer = new byte[(ulong)(data.Length) - ndf.Header.HeaderSize];
                        ms.Read(contentBuffer, 0, contentBuffer.Length);

                        var da = Compressor.Decomp(contentBuffer);
                        uncompStream.Write(da,0,da.Length);

                        data = uncompStream.ToArray();
                    }
                }
            }

            using (var ms = new MemoryStream(data))
            {
                ndf.Footer = ReadFooter(ms, ndf.Header);
                ndf.Classes = ReadClasses(ms, ndf);
                ReadProperties(ms, ndf);

                ndf.Strings = ReadStrings(ms, ndf);
                ndf.Trans = ReadTrans(ms, ndf);


                ndf.TopObjects = ReadUIntList(ms, ndf, "TOPO");
                ndf.Import = ReadUIntList(ms, ndf, "IMPR");
                ndf.Export = ReadUIntList(ms, ndf, "EXPR");

                ndf.Instances = ReadObjects(ms, ndf);
            }

            return ndf;
        }

        public byte[] GetUncompressedNdfbinary(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                var header = ReadHeader(ms);

                if (header.IsCompressedBody)
                {
                    using (var uncompStream = new MemoryStream())
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                        var headBuffer = new byte[header.HeaderSize];
                        ms.Read(headBuffer, 0, headBuffer.Length);
                        uncompStream.Write(headBuffer, 0, headBuffer.Length);

                        ms.Seek((long)header.HeaderSize, SeekOrigin.Begin);

                        var buffer = new byte[4];
                        ms.Read(buffer, 0, buffer.Length);
                        uint compressedblocklen = BitConverter.ToUInt32(buffer, 0);

                        var contentBuffer = new byte[(ulong)(data.Length) - header.HeaderSize];
                        ms.Read(contentBuffer, 0, contentBuffer.Length);
                        //Compressor.Decomp(contentBuffer, uncompStream);
                        var da = Compressor.Decomp(contentBuffer);

                        uncompStream.Write(da,0, da.Length);

                        data = uncompStream.ToArray();
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// Reads the header data of the compiled Ndf binary.
        /// </summary>
        /// <returns>A valid instance of the Headerfile.</returns>
        protected NdfHeader ReadHeader(Stream ms)
        {
            var header = new NdfHeader();

            var buffer = new byte[4];
            ms.Read(buffer, 0, buffer.Length);

            if (BitConverter.ToUInt32(buffer, 0) != 809981253)
                throw new InvalidDataException("No EUG0 found on top of this file!");

            ms.Read(buffer, 0, buffer.Length);

            if (BitConverter.ToUInt32(buffer, 0) != 0)
                throw new InvalidDataException("Bytes between EUG0 and CNDF have to be 0");

            ms.Read(buffer, 0, buffer.Length);

            if (BitConverter.ToUInt32(buffer, 0) != 1178881603)
                throw new InvalidDataException("No CNDF (Compiled NDF)!");

            ms.Read(buffer, 0, buffer.Length);
            header.IsCompressedBody = BitConverter.ToInt32(buffer, 0) == 128;

            buffer = new byte[8];

            ms.Read(buffer, 0, buffer.Length);
            header.FooterOffset = BitConverter.ToUInt64(buffer, 0);

            ms.Read(buffer, 0, buffer.Length);
            header.HeaderSize = BitConverter.ToUInt64(buffer, 0);

            ms.Read(buffer, 0, buffer.Length);
            header.FullFileSizeUncomp = BitConverter.ToUInt64(buffer, 0);

            return header;
        }

        /// <summary>
        /// Reads the footer data which is the Ndfbin Dictionary.
        /// </summary>
        /// <returns></returns>
        protected NdfFooter ReadFooter(Stream ms, NdfHeader head)
        {
            var footer = new NdfFooter();

            ms.Seek((long)head.FooterOffset, SeekOrigin.Begin);

            var dwdBuffer = new byte[4];
            var qwdbuffer = new byte[8];

            ms.Read(dwdBuffer, 0, dwdBuffer.Length);
            if (BitConverter.ToUInt32(dwdBuffer, 0) != 809717588)
                throw new InvalidDataException("Footer doesnt start with TOC0");


            ms.Read(dwdBuffer, 0, dwdBuffer.Length);
            uint footerEntryCount = BitConverter.ToUInt32(dwdBuffer, 0);

            for (int i = 0; i < footerEntryCount; i++)
            {
                var entry = new NdfFooterEntry();

                ms.Read(qwdbuffer, 0, qwdbuffer.Length);
                entry.Name = Encoding.ASCII.GetString(qwdbuffer).TrimEnd('\0'); ;

                ms.Read(qwdbuffer, 0, qwdbuffer.Length);
                entry.Offset = BitConverter.ToInt64(qwdbuffer, 0);

                ms.Read(qwdbuffer, 0, qwdbuffer.Length);
                entry.Size = BitConverter.ToInt64(qwdbuffer, 0);

                footer.Entries.Add(entry);
            }

            return footer;
        }

        /// <summary>
        /// Reads the Classes dictionary.
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        protected ObservableCollection<NdfClass> ReadClasses(Stream ms, NdfBinary owner)
        {
            var classes = new ObservableCollection<NdfClass>();

            NdfFooterEntry classEntry = owner.Footer.Entries.Single(x => x.Name == "CLAS");

            ms.Seek(classEntry.Offset, SeekOrigin.Begin);

            int i = 0;
            var buffer = new byte[4];

            while (ms.Position < classEntry.Offset + classEntry.Size)
            {
                var nclass = new NdfClass(owner)
                    {
#if DEBUG
                        Offset = ms.Position,
#endif
                        Id = i
                    };

                ms.Read(buffer, 0, buffer.Length);
                int strLen = BitConverter.ToInt32(buffer, 0);

                var strBuffer = new byte[strLen];
                ms.Read(strBuffer, 0, strBuffer.Length);

                nclass.Name = Encoding.GetEncoding("ISO-8859-1").GetString(strBuffer);

                i++;
                classes.Add(nclass);
            }

            return classes;
        }

        /// <summary>
        /// Reads the Properties dictionary and relates each one to its owning class.
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="owner"></param>
        protected void ReadProperties(Stream ms, NdfBinary owner)
        {
            NdfFooterEntry propEntry = owner.Footer.Entries.Single(x => x.Name == "PROP");
            ms.Seek(propEntry.Offset, SeekOrigin.Begin);

            int i = 0;
            var buffer = new byte[4];
            while (ms.Position < propEntry.Offset + propEntry.Size)
            {
                var property = new NdfProperty
                    {
#if DEBUG
                        Offset = ms.Position,
#endif
                        Id = i
                    };

                ms.Read(buffer, 0, buffer.Length);
                int strLen = BitConverter.ToInt32(buffer, 0);

                var strBuffer = new byte[strLen];
                ms.Read(strBuffer, 0, strBuffer.Length);

                property.Name = Encoding.GetEncoding("ISO-8859-1").GetString(strBuffer);

                ms.Read(buffer, 0, buffer.Length);

                NdfClass cls = owner.Classes.Single(x => x.Id == BitConverter.ToInt32(buffer, 0));
                property.Class = cls;

                cls.Properties.Add(property);

                i++;
            }
        }

        /// <summary>
        /// Reads the string list.
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        protected ObservableCollection<NdfStringReference> ReadStrings(Stream ms, NdfBinary owner)
        {
            var strings = new ObservableCollection<NdfStringReference>();

            NdfFooterEntry stringEntry = owner.Footer.Entries.Single(x => x.Name == "STRG");
            ms.Seek(stringEntry.Offset, SeekOrigin.Begin);

            int i = 0;
            var buffer = new byte[4];
            while (ms.Position < stringEntry.Offset + stringEntry.Size)
            {
                var nstring = new NdfStringReference { Id = i };

                ms.Read(buffer, 0, buffer.Length);
                int strLen = BitConverter.ToInt32(buffer, 0);

                var strBuffer = new byte[strLen];
                ms.Read(strBuffer, 0, strBuffer.Length);

                nstring.Value = Encoding.GetEncoding("ISO-8859-1").GetString(strBuffer);

                i++;
                strings.Add(nstring);
            }

            return strings;
        }

        /// <summary>
        /// Reads the trans list
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        protected ObservableCollection<NdfTranReference> ReadTrans(Stream ms, NdfBinary owner)
        {
            var trans = new ObservableCollection<NdfTranReference>();

            NdfFooterEntry stringEntry = owner.Footer.Entries.Single(x => x.Name == "TRAN");
            ms.Seek(stringEntry.Offset, SeekOrigin.Begin);

            int i = 0;
            var buffer = new byte[4];
            while (ms.Position < stringEntry.Offset + stringEntry.Size)
            {
                var ntran = new NdfTranReference { Id = i };

                ms.Read(buffer, 0, buffer.Length);
                int strLen = BitConverter.ToInt32(buffer, 0);

                var strBuffer = new byte[strLen];
                ms.Read(strBuffer, 0, strBuffer.Length);

                ntran.Value = Encoding.GetEncoding("ISO-8859-1").GetString(strBuffer);

                i++;
                trans.Add(ntran);
            }

            // TODO: Trans is actually more a tree than a list, this is still not fully implemented/reversed.

            return trans;
        }

        /// <summary>
        /// Reads the amount of instances this file contains.
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        protected uint ReadChunk(Stream ms, NdfBinary owner)
        {
            NdfFooterEntry chnk = owner.Footer.Entries.Single(x => x.Name == "CHNK");
            ms.Seek(chnk.Offset, SeekOrigin.Begin);

            var buffer = new byte[4];

            ms.Read(buffer, 0, buffer.Length);
            ms.Read(buffer, 0, buffer.Length);

            return BitConverter.ToUInt32(buffer, 0);
        }

        /// <summary>
        /// Reads a list of UInt32, this is needed for the topobjects, import and export tables.
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="owner"></param>
        /// <param name="lst"></param>
        /// <returns></returns>
        protected List<uint> ReadUIntList(Stream ms, NdfBinary owner, string lst)
        {
            var uintList = new List<uint>();

            NdfFooterEntry uintEntry = owner.Footer.Entries.Single(x => x.Name == lst);
            ms.Seek(uintEntry.Offset, SeekOrigin.Begin);

            var buffer = new byte[4];
            while (ms.Position < uintEntry.Offset + uintEntry.Size)
            {
                ms.Read(buffer, 0, buffer.Length);
                uintList.Add(BitConverter.ToUInt32(buffer, 0));
            }

            return uintList;
        }

        /// <summary>
        /// Reads the object instances.
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        protected List<NdfObject> ReadObjects(Stream ms, NdfBinary owner)
        {
            var objects = new List<NdfObject>();

            uint instanceCount = ReadChunk(ms, owner);

            NdfFooterEntry objEntry = owner.Footer.Entries.Single(x => x.Name == "OBJE");
            ms.Seek(objEntry.Offset, SeekOrigin.Begin);

            for (uint i = 0; i < instanceCount; i++)
            {
                long objOffset = ms.Position;

                NdfObject obj = ReadObject(ms, i, owner);
                obj.Offset = objOffset;

                objects.Add(obj);
            }

            return objects;
        }

        /// <summary>
        /// Reads one object instance.
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="index"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        protected NdfObject ReadObject(Stream ms, uint index, NdfBinary owner)
        {
            var instance = new NdfObject { Id = index };

            if (owner.TopObjects.Contains(index))
                instance.IsTopObject = true;

            var buffer = new byte[4];
            ms.Read(buffer, 0, buffer.Length);
            int classId = BitConverter.ToInt32(buffer, 0);

            if (owner.Classes.Count < classId)
                throw new InvalidDataException("Object without class found.");

            NdfClass cls = instance.Class = owner.Classes[classId];

            cls.Instances.Add(instance);

            // Read properties
            for (; ; )
            {
                ms.Read(buffer, 0, buffer.Length);
                uint propertyId = BitConverter.ToUInt32(buffer, 0);

                if (propertyId == 0xABABABAB)
                    break;

                var propVal = new NdfPropertyValue(instance);

                propVal.Property = cls.Properties.SingleOrDefault(x => x.Id == propertyId);

                if (propVal.Property != null)
                    instance.PropertyValues.Add(propVal);

                //throw new InvalidDataException("Found a value for a property which doens't exist in this class.");

                NdfValueWrapper res = ReadValue(ms, propVal, owner);

                propVal.Value = res;
            }

            owner.AddEmptyProperties(instance);

            return instance;
        }

        /// <summary>
        /// Reads the value of a Property inside a object instance.
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="prop"></param>
        /// <returns>A NdfValueWrapper Instance.</returns>
        protected NdfValueWrapper ReadValue(Stream ms, NdfPropertyValue prop, NdfBinary binary)
        {
            uint contBufferlen;
            NdfValueWrapper value;
            var buffer = new byte[4];

            ms.Read(buffer, 0, buffer.Length);
            NdfType type = NdfTypeManager.GetType(buffer);

            if (type == NdfType.Unknown)
                throw new InvalidDataException("Unknown datatypes are not supported!");

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
                case NdfType.Blob:
                case NdfType.ZipBlob:
                    ms.Read(buffer, 0, buffer.Length);
                    contBufferlen = BitConverter.ToUInt32(buffer, 0);

                    if (type == NdfType.ZipBlob)
                        if (ms.ReadByte() != 1)
                            throw new InvalidDataException("has to be checked.");
                    break;
                default:
                    contBufferlen = NdfTypeManager.SizeofType(type);
                    break;
            }

            if (type == NdfType.List || type == NdfType.MapList)
            {
                NdfCollection lstValue;

                if (type == NdfType.List)
                    lstValue = new NdfCollection(ms.Position);
                else
                    lstValue = new NdfMapList(ms.Position);

                for (int i = 0; i < contBufferlen; i++)
                {
                    CollectionItemValueHolder res;
                    if (type == NdfType.List)
                        res = new CollectionItemValueHolder(ReadValue(ms, prop, binary), binary,
                                                            prop.InstanceOffset);
                    else
                        res = new CollectionItemValueHolder(
                            new NdfMap(
                                new MapValueHolder(ReadValue(ms, prop, binary), binary, prop.InstanceOffset),
                                new MapValueHolder(ReadValue(ms, prop, binary), binary, prop.InstanceOffset),
                                ms.Position, binary),
                            binary, prop.InstanceOffset);

                    lstValue.Add(res);
                }

                value = lstValue;
            }
            else if (type == NdfType.Map)
            {
                value = new NdfMap(
                    new MapValueHolder(ReadValue(ms, prop, binary), binary, prop.InstanceOffset),
                    new MapValueHolder(ReadValue(ms, prop, binary), binary, prop.InstanceOffset),
                    ms.Position, binary);
            }
            else
            {
                var contBuffer = new byte[contBufferlen];
                ms.Read(contBuffer, 0, contBuffer.Length);

                if (prop != null)
                    prop.ValueData = contBuffer;

                value = NdfTypeManager.GetValue(contBuffer, type, binary, ms.Position - contBuffer.Length);
            }

            return value;
        }
    }
}
