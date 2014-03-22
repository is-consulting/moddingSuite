using System.Globalization;
using moddingSuite.ViewModel.Base;

namespace moddingSuite.Model.Ndfbin
{
    public class NdfProperty : ViewModelBase
    {
        private int _id;
        private string _name;
        private NdfClass _class;

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

        public NdfClass Class
        {
            get { return _class; }
            set
            {
                _class = value;
                OnPropertyChanged(() => Class);
            }
        }

        public NdfProperty(int id)
        {
            Id = id;
        }

        public override string ToString()
        {
            return Name.ToString(CultureInfo.InvariantCulture);
        }
    }
}