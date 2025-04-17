namespace StockApp.Commands
{
    using System;
    using System.Windows.Input;

    public class RelayCommandGeneric<T> : ICommand
    {
        private readonly Action<T> execute;
        private readonly Func<T, bool>? canExecute;

        public event EventHandler CanExecuteChanged;

        public RelayCommandGeneric(Action<T> execute, Func<T, bool>? canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (parameter == null && typeof(T).IsValueType)
            {
                return false;
            }

            return this.canExecute == null || this.canExecute((T)parameter);
        }

        public void Execute(object parameter) => this.execute((T)parameter);

        public void RaiseCanExecuteChanged() => this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}