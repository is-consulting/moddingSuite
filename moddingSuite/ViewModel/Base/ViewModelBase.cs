using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace moddingSuite.ViewModel.Base
{
    public class ViewModelBase : INotifyPropertyChanged, IEditableObject
    {
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

        public virtual void BeginEdit()
        {
            //throw new NotImplementedException();
        }

        public virtual void CancelEdit()
        {
            //throw new NotImplementedException();
        }

        public virtual void EndEdit()
        {
            //throw new NotImplementedException();
        }
    }
}
