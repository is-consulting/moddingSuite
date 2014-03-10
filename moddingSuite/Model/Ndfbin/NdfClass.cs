using System;
using System.Collections.ObjectModel;
using System.Globalization;
using moddingSuite.BL;
using moddingSuite.ViewModel.Base;

namespace moddingSuite.Model.Ndfbin
{
    public class NdfClass : ViewModelBase
    {
        private int _id;

        private string _name;
        private readonly ObservableCollection<NdfObject> _instances = new ObservableCollection<NdfObject>();
        private readonly ObservableCollection<NdfProperty> _properties = new ObservableCollection<NdfProperty>();

#if DEBUG
        private long _offset;
#endif

        public int Id
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

#if DEBUG
        public long Offset
        {
            get { return _offset; }
            set
            {
                _offset = value;
                OnPropertyChanged(() => Offset);
            }
        }
#endif

        public NdfBinary Manager { get; protected set; }

        public NdfClass(NdfBinary mgr)
        {
            Manager = mgr;
        }

        public override string ToString()
        {
            return Name.ToString(CultureInfo.InvariantCulture);
        }
    }
}