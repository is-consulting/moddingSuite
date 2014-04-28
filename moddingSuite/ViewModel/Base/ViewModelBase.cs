using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Xml.Serialization;

namespace moddingSuite.ViewModel.Base
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        private bool _isUiBusy = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged<T>(params Expression<Func<T>>[] props)
        {
            foreach (var prop in props)
            {
                var body = prop.Body as MemberExpression;
                if (PropertyChanged != null && body != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(body.Member.Name));
            }
        }

        public void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        [XmlIgnore]
        public bool IsUIBusy
        {
            get
            {
                return _isUiBusy;
            }
            set
            {
                _isUiBusy = value;
                OnPropertyChanged(() => IsUIBusy);
            }
        }
    }
}
