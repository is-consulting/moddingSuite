namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfFileNameString : NdfString
    {
        public NdfFileNameString(NdfStringReference value) : base(value)
        {
            Type = NdfType.TableStringFile;
        }
    }
}