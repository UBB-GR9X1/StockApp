namespace StockApp.Commands
{
    using System;
    using System.Windows.Input;

    public class RelayCommandGeneric<T> : ICommand
    {
        private readonly Action<T> execute;
        private readonly Func<T, bool>? canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommandGeneric{T}"/> class.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommandGeneric{T}"/> class.
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public RelayCommandGeneric(Action<T> execute, Func<T, bool>? canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            if (parameter == null && typeof(T).IsValueType)
            {
                return false;
            }

            return this.canExecute == null || this.canExecute((T)parameter);
        }

        /// <summary>
        /// Invokes the <see cref="Execute"/> method on the command.
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter) => this.execute((T)parameter);

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event to indicate that the command's ability to execute has changed.
        /// </summary>
        public void RaiseCanExecuteChanged() => this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}