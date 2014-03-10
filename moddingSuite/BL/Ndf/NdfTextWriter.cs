using System;
using System.IO;
using System.Linq;
using System.Text;
using moddingSuite.Model.Ndfbin;

namespace moddingSuite.BL.Ndf
{
    public class NdfTextWriter : INdfWriter
    {
        public const string InstanceNamePrefix = "public";
        public static readonly Encoding NdfTextEncoding = Encoding.Unicode;

        public void Write(Stream outStrea, NdfBinary ndf, bool compressed)
        {
            throw new NotImplementedException();
        }

        public byte[] CreateNdfScript(NdfBinary ndf)
        {
            using (var ms = new MemoryStream())
            {
                byte[] buffer = NdfTextEncoding.GetBytes(string.Format("// Handwritten by enohka \n// For real\n\n\n"));

                ms.Write(buffer, 0, buffer.Length);

                foreach (NdfObject instance in ndf.Instances.Where(x => x.IsTopObject))
                {
                    buffer = instance.GetNdfText();
                    ms.Write(buffer, 0, buffer.Length);
                }

                return ms.ToArray();
            }
        }
    }
}
