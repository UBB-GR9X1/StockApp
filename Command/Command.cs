namespace StockApp.Command
{
    using System;
    using System.Windows.Input;

    public partial class Command : ICommand
    {
        private readonly Action execute;
        private readonly Func<bool>? canExecute;

        public event EventHandler CanExecuteChanged;

        public Command(Action execute, Func<bool>? canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => this.canExecute == null || this.canExecute();

        public void Execute(object parameter) => this.execute();

        public void RaiseCanExecuteChanged() => this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
