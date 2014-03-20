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
        }

        private void ExportKdTreeExecute(object obj)
        {
            var data = GetBytes();

            using (var ms = new MemoryStream(data))
            {
                Settings.Settings settings = SettingsManager.Load();

                var buffer = new byte[4];

                //ms.Read(buffer, 0, buffer.Length);
                //uint lenSum = BitConverter.ToUInt32(buffer, 0);

                int blockId = 1;
                var creationDate = DateTime.Now;

                while (ms.Position < ms.Length)
                {
                    ms.Read(buffer, 0, buffer.Length);
                    uint blockLenComp = BitConverter.ToUInt32(buffer, 0);

                    ms.Read(buffer, 0, buffer.Length);
                    uint blockLenUnComp = BitConverter.ToUInt32(buffer, 0);

                    var subBlockBuffer = new byte[blockLenComp - 4];

                    ms.Read(subBlockBuffer, 0, subBlockBuffer.Length);

                    var subBlockData = Compressor.Decomp(subBlockBuffer);

                    using (var fs = new FileStream(Path.Combine(settings.SavePath, string.Format("Kd_tree_{0}_{1}", creationDate.ToString("dd_MM_yyyy_HH_mm_ff"), blockId)), FileMode.OpenOrCreate))
                    {
                        fs.Write(subBlockData, 0, subBlockData.Length);
                        fs.Flush();
                    }

                    blockId++;
                }
            }
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
    }
}
