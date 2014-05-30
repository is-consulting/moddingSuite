using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using moddingSuite.BL.Compressing;
using moddingSuite.Model.Ndfbin;
using moddingSuite.Model.Ndfbin.Types;

namespace moddingSuite.BL.Ndf
{
    public class NdfbinWriter : INdfWriter
    {
        public const uint EugenMagic = 809981253;
        public const uint NdfBinMagic = 1178881603;
        public const ulong NdfbinHeaderSize = 40;

        /// <summary>
        /// Writes a the bytecode of a Ndf file into outStream.
        /// </summary>
        /// <param name="outStream">The Stream in wich the data has to be written in.</param>
        /// <param name="ndf">The ndf file which has to be compiled.</param>
        /// <param name="compressed">Sets wether the bytecode has to be compressed or not.</param>
        public void Write(Stream outStream, NdfBinary ndf, bool compressed)
        {
            uint compressedFlag = compressed ? 128 : 0u;

            outStream.Write(BitConverter.GetBytes(EugenMagic), 0, 4);
            outStream.Write(BitConverter.GetBytes((uint)0), 0, 4);
            outStream.Write(BitConverter.GetBytes(NdfBinMagic), 0, 4);
            outStream.Write(BitConverter.GetBytes(compressedFlag), 0, 4);

            var data = GetCompiledContent(ndf);

            outStream.Write(BitConverter.GetBytes(ndf.Footer.Offset), 0, 8);
            outStream.Write(BitConverter.GetBytes(NdfbinHeaderSize), 0, 8);
            outStream.Write(BitConverter.GetBytes(NdfbinHeaderSize + (ulong)data.Length), 0, 8);

            if (compressed)
            {
                outStream.Write(BitConverter.GetBytes(data.Length), 0, 4);
                //Compressor.Comp(data, outStream);

                var da = Compressor.Comp(data);

                outStream.Write(da,0,da.Length);
            }
            else
                outStream.Write(data, 0, data.Length);
        }

        public byte[] Write(NdfBinary ndf, bool compressed)
        {
            using (var ms = new MemoryStream())
            {
                Write(ms, ndf, compressed);

                return ms.ToArray();
            }
        }

        protected byte[] GetCompiledContent(NdfBinary ndf)
        {
            var footer = new NdfFooter();

            const long headerSize = (long)NdfbinHeaderSize;

            using (var contentStream = new MemoryStream())
            {
                byte[] buffer = RecompileObj(ndf);
                footer.AddEntry("OBJE", contentStream.Position + headerSize, buffer.Length);
                contentStream.Write(buffer, 0, buffer.Length);

                buffer = RecompileTopo(ndf);
                footer.AddEntry("TOPO", contentStream.Position + headerSize, buffer.Length);
                contentStream.Write(buffer, 0, buffer.Length);

                buffer = RecompileChnk(ndf);
                footer.AddEntry("CHNK", contentStream.Position + headerSize, buffer.Length);
                contentStream.Write(buffer, 0, buffer.Length);

                buffer = RecompileClas(ndf);
                footer.AddEntry("CLAS", contentStream.Position + headerSize, buffer.Length);
                contentStream.Write(buffer, 0, buffer.Length);

                buffer = RecompileProp(ndf);
                footer.AddEntry("PROP", contentStream.Position + headerSize, buffer.Length);
                contentStream.Write(buffer, 0, buffer.Length);

                buffer = RecompileStrTable(ndf.Strings);
                footer.AddEntry("STRG", contentStream.Position + headerSize, buffer.Length);
                contentStream.Write(buffer, 0, buffer.Length);

                buffer = RecompileStrTable(ndf.Trans);
                footer.AddEntry("TRAN", contentStream.Position + headerSize, buffer.Length);
                contentStream.Write(buffer, 0, buffer.Length);

                buffer = RecompileUIntList(ndf.Import);
                footer.AddEntry("IMPR", contentStream.Position + headerSize, buffer.Length);
                contentStream.Write(buffer, 0, buffer.Length);

                buffer = RecompileUIntList(ndf.Export);
                footer.AddEntry("EXPR", contentStream.Position + headerSize, buffer.Length);
                contentStream.Write(buffer, 0, buffer.Length);

                buffer = footer.GetBytes();

                footer.Offset = (ulong)contentStream.Position + NdfbinHeaderSize;
                contentStream.Write(buffer, 0, buffer.Length);

                ndf.Footer = footer;

                return contentStream.ToArray();
            }
        }

        protected byte[] RecompileObj(NdfBinary ndf)
        {
            var objectPart = new List<byte>();

            byte[] objSep = { 0xAB, 0xAB, 0xAB, 0xAB };

            foreach (NdfObject instance in ndf.Instances)
            {
                objectPart.AddRange(BitConverter.GetBytes(instance.Class.Id));

                foreach (NdfPropertyValue propertyValue in instance.PropertyValues)
                {
                    if (propertyValue.Type == NdfType.Unset)
                        continue;

                    byte[] valueBytes = propertyValue.Value.GetBytes();

                    if (propertyValue.Value.Type == NdfType.Unset)
                        continue;

                    objectPart.AddRange(BitConverter.GetBytes(propertyValue.Property.Id));

                    if (propertyValue.Value.Type == NdfType.ObjectReference ||
                        propertyValue.Value.Type == NdfType.TransTableReference)
                        objectPart.AddRange(BitConverter.GetBytes((uint)NdfType.Reference));

                    objectPart.AddRange(BitConverter.GetBytes((uint)propertyValue.Value.Type));
                    objectPart.AddRange(valueBytes);
                }

                objectPart.AddRange(objSep);
            }

            return objectPart.ToArray();
        }

        protected byte[] RecompileTopo(NdfBinary ndf)
        {
            using (var ms = new MemoryStream())
            {
                List<NdfObject> topInsts = ndf.Instances.Where(x => x.IsTopObject).ToList();


                //var writeInsts = new HashSet<NdfObject>();

                //foreach (NdfObject inst in topInsts)
                //{
                //    if (writeInsts.Contains(inst))
                //        continue;

                //    writeInsts.Add(inst);

                //    int nextItemId = topInsts.IndexOf(inst) + 1;

                //    if (topInsts.Count > nextItemId && topInsts[nextItemId].Class != inst.Class)
                //    {
                //        IEnumerable<NdfObject> othersOfSameClass =
                //            topInsts.GetRange(nextItemId, topInsts.Count - nextItemId).Where(
                //                x => x.Class == inst.Class && !writeInsts.Contains(x));

                //        foreach (NdfObject o in othersOfSameClass)
                //            writeInsts.Add(o);
                //    }
                //}

                var test = topInsts.OrderBy(x => x.Class.Id);

                foreach (NdfObject instance in test)
                {
                    byte[] buffer = BitConverter.GetBytes(instance.Id);
                    ms.Write(buffer, 0, buffer.Length);
                }

                return ms.ToArray();
            }
        }

        protected byte[] RecompileChnk(NdfBinary ndf)
        {
            var chnk = new List<byte>();

            chnk.AddRange(BitConverter.GetBytes((uint)0));
            chnk.AddRange(BitConverter.GetBytes(ndf.Instances.Count));

            return chnk.ToArray();
        }

        protected byte[] RecompileClas(NdfBinary ndf)
        {
            var clasData = new List<byte>();

            foreach (var clas in ndf.Classes.OrderBy(x => x.Id))
            {
                var nameData = Encoding.GetEncoding("ISO-8859-1").GetBytes(clas.Name);
                clasData.AddRange(BitConverter.GetBytes(nameData.Length));
                clasData.AddRange(nameData);
            }

            return clasData.ToArray();
        }

        protected byte[] RecompileProp(NdfBinary ndf)
        {
            var propData = new List<byte>();

            var props = new List<NdfProperty>();

            foreach (var clas in ndf.Classes)
                props.AddRange(clas.Properties);

            foreach (var prop in props.OrderBy(x => x.Id))
            {
                var nameData = Encoding.GetEncoding("ISO-8859-1").GetBytes(prop.Name);
                propData.AddRange(BitConverter.GetBytes(nameData.Length));
                propData.AddRange(nameData);
                propData.AddRange(BitConverter.GetBytes(prop.Class.Id));
            }

            return propData.ToArray();
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

        protected byte[] RecompileUIntList(IEnumerable<uint>  lst)
        {
            var data = new List<byte>();

            foreach (var u in lst)
                data.AddRange(BitConverter.GetBytes(u));

            return data.ToArray();
        }
    }
}
