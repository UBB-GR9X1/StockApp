namespace StockApp.Commands
{
    using System;
    using System.Windows.Input;

    public class StockNewsRelayCommand : ICommand
    {
        private readonly Action execute;
        private readonly Func<bool>? canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="StockNewsRelayCommand"/> class.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="StockNewsRelayCommand"/> class.
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public StockNewsRelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter) => this.canExecute?.Invoke() ?? true;

        /// <summary>
        /// Invokes the <see cref="Execute"/> method on the command.
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter) => this.execute();

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event to indicate that the command's ability to execute has changed.
        /// </summary>
        public void RaiseCanExecuteChanged() => this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}