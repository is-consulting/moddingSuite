using moddingSuite.Model.Ndfbin.Types.AllTypes;

namespace moddingSuite.Model.Ndfbin
{
    public interface IValueHolder
    {
        NdfValueWrapper Value { get; set; }

        NdfBinary Manager { get; }
    }
}