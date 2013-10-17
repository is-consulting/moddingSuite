namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfDouble_2 : NdfDouble
    {
        public NdfDouble_2(double value, long offset) : base(value, offset)
        {
            Type = NdfType.Float64_2;
        }
    }
}