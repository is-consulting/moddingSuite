namespace moddingSuite.Model.Ndfbin.Types
{
    public enum NdfType : uint
    {
        Boolean = 0x00000000,
        Int8 = 0x00000001,

        Int16 = 25,

        Int32 = 0x00000002,
        UInt32 = 0x00000003,
        Float32 = 0x00000005,
        Float64 = 0x00000006,
        Float64_2 = 33,


        Guid = 26,
        Vector = 0x0000000b,
        Color128 = 0x0000000c,
        Color32 = 0x0000000d,

        TableString = 0x00000007,
        TableStringFile = 0x0000001C,
        WideString = 0x00000008,

        LocalisationHash = 29,

        Reference = 0x00000009,
        ObjectReference = 0xBBBBBBBB,
        TransTableReference = 0xAAAAAAAA,

        Map = 0x00000022,
        




        List = 0x00000011,
        MapList = 0x00000012,

        Unknown = 0xFFFFFFFF,
        Unset = 0xEEEEEEEE
    }
}
