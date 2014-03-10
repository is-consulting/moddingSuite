using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using moddingSuite.Model.Ndfbin;

namespace moddingSuite.BL.Ndf
{
    public interface INdfReader
    {
        NdfBinary Read(byte[] data);

    }
}
