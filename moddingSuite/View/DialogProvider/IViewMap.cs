using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.View.DialogProvider
{
    public interface IViewMap
    {
        Type ViewType { get; }
        Type ViewModelType { get; }
    }
}
