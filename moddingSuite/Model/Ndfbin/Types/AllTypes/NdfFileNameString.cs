namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfFileNameString : NdfString
    {
        public NdfFileNameString(NdfStringReference value, long offset) : base(value, offset)
        {
            Type = NdfType.TableStringFile;
        }
    }
}