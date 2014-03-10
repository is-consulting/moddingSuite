using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using moddingSuite.Model.Ndfbin;

namespace moddingSuite.BL.Ndf
{
    interface INdfWriter
    {
        void Write(Stream outStrea, NdfBinary ndf, bool compressed);
    }
}
