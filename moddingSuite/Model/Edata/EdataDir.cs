using moddingSuite.BL;

namespace moddingSuite.Model.Edata
{
    /// <summary>
    /// struct dictGroupEntry {
    ///     DWORD groupId;
    ///     DWORD entrySize;
    ///     zstring name;
    /// };
    /// </summary>
    public class EdataDir : EdataEntity
    {
        public EdataDir(EdataManager mgr) : base(mgr)
        {
        }
    }
}