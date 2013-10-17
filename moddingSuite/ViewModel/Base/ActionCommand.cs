using System;
using System.Diagnostics;
using System.Windows.Input;

namespace moddingSuite.ViewModel.Base
{
    public class ActionCommand : ICommand
    {
        private readonly Action<object> _executeHandler;
        private readonly Func<bool> _canExecuteHandler;

        public ActionCommand(Action<object> execute, Func<bool> canExecute = null)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _executeHandler = execute;
            _canExecuteHandler = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public void Execute(object parameter)
        {
            _executeHandler(parameter);
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecuteHandler == null || _canExecuteHandler();

        }
    }
}
