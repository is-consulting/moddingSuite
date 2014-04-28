using System.Collections.ObjectModel;
using System.Globalization;
using moddingSuite.ViewModel.Base;

namespace moddingSuite.Model.Ndfbin
{
    public class NdfClass : ViewModelBase
    {
        private uint _id;

        private string _name;
        private readonly ObservableCollection<NdfObject> _instances = new ObservableCollection<NdfObject>();
        private readonly ObservableCollection<NdfProperty> _properties = new ObservableCollection<NdfProperty>();

        public uint Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged(() => Id);
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(() => Name);
            }
        }

        public ObservableCollection<NdfProperty> Properties
        {
            get { return _properties; }
        }

        public ObservableCollection<NdfObject> Instances
        {
            get { return _instances; }
        }

        public NdfBinary Manager { get; protected set; }

        public NdfClass(NdfBinary mgr, uint id)
        {
            Manager = mgr;
            Id = id;
        }

        public override string ToString()
        {
            return Name.ToString(CultureInfo.InvariantCulture);
        }
    }
}