using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using moddingSuite.ViewModel.Base;

namespace moddingSuite.ViewModel.UnhandledException
{
    public class UnhandledExceptionViewModel : ViewModelBase
    {
        private string _title;
        private string _errorText;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        public string ErrorText
        {
            get { return _errorText; }
            set
            {
                _errorText = value;
                OnPropertyChanged("ErrorText");
            }
        }

        public ICommand SendErrorCommand { get; set; }

        public UnhandledExceptionViewModel(Exception exception)
        {
            Title = "An unhandled exception occured";

            SendErrorCommand = new ActionCommand(SendErrorExecute);

            var sb = new StringBuilder();

            var excep = exception;

            while (excep != null)
            {
                sb.Append(exception);
                excep = excep.InnerException;
            }

            ErrorText = sb.ToString();
        }

        private void SendErrorExecute(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
