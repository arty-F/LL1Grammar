using System;
using System.Windows.Input;

namespace LL1GrammarUI
{
    public class CommandHandler : ICommand
    {
        private Action action;
        private Action<object> actionParam;
        private Func<bool> canExecute;
        private Func<object, bool> canExecuteParam;
        private bool withParam;

        public CommandHandler(Action action, Func<bool> canExecute)
        {
            this.action = action;
            this.canExecute = canExecute;
            withParam = false;
        }

        public CommandHandler(Action<object> action, Func<object, bool> canExecute)
        {
            actionParam = action;
            canExecuteParam = canExecute;
            withParam = true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return withParam ? canExecuteParam.Invoke(parameter) : canExecute.Invoke();
        }

        public void Execute(object parameter)
        {
            if (withParam)
                actionParam(parameter);
            else
                action();
        }
    }
}
