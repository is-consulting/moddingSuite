using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using moddingSuite.BL;
using moddingSuite.BL.Compressing;
using moddingSuite.ViewModel.Base;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfBlob : NdfFlatValueWrapper
    {
        public ICommand ExportRawCommand { get; set; }
        public ICommand ExportKdTreeCommand { get; set; }
        public NdfBlob(byte[] value)
            : base(NdfType.Blob, value)
        {
            ExportRawCommand = new ActionCommand(ExportRawExecute);
            ExportKdTreeCommand = new ActionCommand(ExportKdTreeExecute);
            //deblockify();
        }
        private List<byte[]> chunks = new List<byte[]>();
        uint mysteryNumber;
        byte[] mysteryChunk;
        const uint mysteryChunkLenth = 70;
        private void ExportKdTreeExecute(object obj)
        {
            /*foreach (var chunk in chunks)
            {
                using (var ms = new MemoryStream(chunk)){
                    var buffer = new byte[4];
                    ms.Read(buffer, 0, buffer.Length);
                    uint uncompLength = BitConverter.ToUInt32(buffer, 0);
                    var subBlockBuffer=new byte[uncompLength];
                    Compressor.Decomp(subBlockBuffer);
                }
            {

            
            //deblockify();*/
            Settings.Settings settings = SettingsManager.Load();
            var logFile=new StreamWriter(new FileStream(Path.Combine(settings.SavePath, "exportLogFile.txt"), FileMode.OpenOrCreate));
            logFile.WriteLine("kdf values:");
            foreach(var propValue in this.ParentProperty.Instance.PropertyValues){
                logFile.WriteLine("{0}:\t{1}", propValue.Property.Name, propValue.Value);
                //propValue.Property.Name
            }
            
            var data = GetBytes();
            logFile.WriteLine("blob export data, start byte/end byte/uncompressed length:");
            using (var ms = new MemoryStream(data))
            {
                
                var buffer = new byte[4];

                ms.Read(buffer, 0, buffer.Length);
                uint fileLength = BitConverter.ToUInt32(buffer, 0);
                //printToFile(ms, "VertexPositionBuffer", logFile);
                
                readVertexes(ms);
                
                printToFile(ms, "VertexNormaleBuffer", logFile);
                //ms.Seek(8, SeekOrigin.Current);
                uncompressedPrintToFile(ms, "3rdPartUnComp", 8, logFile);
                printToFile(ms, "IndexBuffer", logFile);
                //ms.Seek(4, SeekOrigin.Current);
                uncompressedPrintToFile(ms, "5thPartUnComp", 4, logFile);
                printToFile(ms, "TriangleIndexBuffer", logFile);
                var off = ms.Position % 8;
                if (off == 0) off = 8;
                //ms.Seek(72-off, SeekOrigin.Current);
                uncompressedPrintToFile(ms, "7thPartUncomp", (int)(72 - off), logFile);
                printToFile(ms, "Subtrees", logFile);
                
            }
            logFile.Flush();
        }
        /*private void deblockify()
        {
            var data = GetBytes();
            using (var ms = new MemoryStream(data))
            {
                
                var buffer = new byte[4];
                ms.Read(buffer, 0, buffer.Length);
                uint fileLength = BitConverter.ToUInt32(buffer, 0);
                chunks.Add(getNextChunk(ms));
                chunks.Add(getNextChunk(ms));
                ms.Read(buffer, 0, buffer.Length);
                ms.Read(buffer, 0, buffer.Length);
                mysteryNumber = BitConverter.ToUInt32(buffer, 0);
                chunks.Add(getNextChunk(ms));
                ms.Read(buffer, 0, buffer.Length);
                chunks.Add(getNextChunk(ms));
                mysteryChunk = new byte[mysteryChunkLenth];
                ms.Read(mysteryChunk, 0, mysteryChunk.Length);
                chunks.Add(getNextChunk(ms));
            }

        }*/
        /*private byte[] getNextChunk(MemoryStream ms)
        {
            var buffer = new byte[4];
            ms.Read(buffer, 0, buffer.Length);
            uint chunkLength = BitConverter.ToUInt32(buffer, 0);
            byte[] bytes = new byte[chunkLength];
            ms.Read(bytes, 0, bytes.Length);
            return bytes;
        }*/
        private void uncompressedPrintToFile(MemoryStream ms, string name, int length,StreamWriter logFile=null)
        {
            Settings.Settings settings = SettingsManager.Load();
            using (var fs = new FileStream(Path.Combine(settings.SavePath, name), FileMode.OpenOrCreate))
            {
                var buffer = new byte[length];
                var start = ms.Position;
                ms.Read(buffer, 0, length);
                var end = ms.Position;
                fs.Write(buffer, 0, length);
                fs.Flush();
                if (logFile != null) logFile.WriteLine("{0}: {1}/{2}/{3}", name, start, end, length);
            }
        }
        private void printToFile(MemoryStream ms,  string name,StreamWriter logFile=null)
        {
            //Console.WriteLine(ms.Position);
            Settings.Settings settings = SettingsManager.Load();
            var buffer = new byte[4];
            var start = ms.Position;
            ms.Read(buffer, 0, buffer.Length);
            uint blockLenComp = BitConverter.ToUInt32(buffer, 0);

            ms.Read(buffer, 0, buffer.Length);
            uint blockLenUnComp = BitConverter.ToUInt32(buffer, 0);

            var subBlockBuffer = new byte[blockLenComp - 4];

            ms.Read(subBlockBuffer, 0, subBlockBuffer.Length);
            var end = ms.Position;
            ms.Seek(0, SeekOrigin.Current);
            var subBlockData = Compressor.Decomp(subBlockBuffer);
            var uncompLength = subBlockData.Length;
            using (var fs = new FileStream(Path.Combine(settings.SavePath, name), FileMode.OpenOrCreate))
            {
                fs.Write(subBlockData, 0, subBlockData.Length);
                fs.Flush();
            }
            if(logFile!=null)logFile.WriteLine("{0}: {1}/{2}/{3}",name, start, end, uncompLength);
            //ms.Seek(2, SeekOrigin.Current);
        }
        private void ExportRawExecute(object obj)
        {
            Settings.Settings settings = SettingsManager.Load();

            var buffer = GetBytes();

            using (var fs = new FileStream(Path.Combine(settings.SavePath, string.Format("blobdump_{0}", DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ff"))), FileMode.OpenOrCreate))
            {
                fs.Write(buffer, 0, buffer.Length);
                fs.Flush();
            }
        }

        public override byte[] GetBytes()
        {
            var val = new List<byte>();
            val.AddRange(BitConverter.GetBytes((uint)((byte[])Value).Length));
            val.AddRange((byte[])Value);

            return val.ToArray();
        }

        public override byte[] GetNdfText()
        {
            throw new NotImplementedException();
        }
        /*private float getHalfFloat(byte[] bytes){
            
        }*/
        public void readVertexes(MemoryStream ms)
        {
            Settings.Settings settings = SettingsManager.Load();
            var buffer = new byte[4];
            var start = ms.Position;
            ms.Read(buffer, 0, buffer.Length);
            uint blockLenComp = BitConverter.ToUInt32(buffer, 0);

            ms.Read(buffer, 0, buffer.Length);
            uint blockLenUnComp = BitConverter.ToUInt32(buffer, 0);

            var subBlockBuffer = new byte[blockLenComp - 4];

            ms.Read(subBlockBuffer, 0, subBlockBuffer.Length);
            var end = ms.Position;
            ms.Seek(0, SeekOrigin.Current);
            var subBlockData = Compressor.Decomp(subBlockBuffer);
            var uncompLength = subBlockData.Length;

            var vertexFile = new StreamWriter(new FileStream(Path.Combine(settings.SavePath, "vertexes.csv"), FileMode.Create));
            var readPosition=0;
            var nbVertexes = BitConverter.ToInt32(subBlockData, readPosition);
            readPosition += 4;
            var unknown = BitConverter.ToInt32(subBlockData, readPosition);
            readPosition += 4;
            //vertexFile.WriteLine("{0}\t{1}\t{2}", nbVertexes, unknown, 0);
            for (var i = 0; i < nbVertexes; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    var b=new byte[2];
                    b[1] = subBlockData[readPosition++];
                    b[0] = subBlockData[readPosition++];
                    //var val = Half.ToHalf(b, 0);//
                   var val= BitConverter.ToUInt16(b, 0);
                    vertexFile.Write("{0}\t", val);
                    //readPosition += 2;
                }
                vertexFile.WriteLine();
            }
            vertexFile.Flush();
        }
    }
}
