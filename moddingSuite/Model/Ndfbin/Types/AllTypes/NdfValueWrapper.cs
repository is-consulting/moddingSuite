using moddingSuite.ViewModel.Base;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public abstract class NdfValueWrapper : ViewModelBase, INdfScriptSerializable
    {
        private NdfType _type;

        protected NdfValueWrapper(NdfType type, long offset)
        {
            Type = type;
            OffSet = offset;
        }

        public NdfType Type
        {
            get { return _type; }
            protected set
            {
                _type = value;
                OnPropertyChanged("Type");
            }
        }

        public long OffSet { get; set; }

        #region INdfScriptSerializable Members

        public abstract byte[] GetNdfText();

        #endregion

        public abstract byte[] GetBytes(out bool valid);
    }
}