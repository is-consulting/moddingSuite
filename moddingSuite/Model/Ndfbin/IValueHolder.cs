using moddingSuite.BL;
using moddingSuite.Model.Ndfbin.Types.AllTypes;

namespace moddingSuite.Model.Ndfbin
{
    public interface IValueHolder
    {
        NdfValueWrapper Value { get; set; }

        NdfbinManager Manager { get; }

        long InstanceOffset { get; }
    }
}