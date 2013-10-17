using moddingSuite.BL;
using moddingSuite.Model.Ndfbin.Types.AllTypes;

namespace moddingSuite.Model.Ndfbin
{
    public class MapValueHolder : CollectionItemValueHolder
    {
        public MapValueHolder(NdfValueWrapper value, NdfbinManager manager, long instanceOffset)
            : base(value, manager, instanceOffset)
        {
        }
    }
}