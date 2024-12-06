using System;
using System.Windows.Input;

namespace Galaga.Model.ViewModel
{
    /// <summary>
    /// Represents a command that can be executed and whose ability to execute can be dynamically determined.
    /// Implements the <see cref="ICommand"/> interface.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action execute;
        private readonly Func<bool> canExecute;

        /// <summary>
        /// Occurs when the ability of the command to execute has changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class with specified execution logic and condition.
        /// </summary>
        /// <param name="execute">The action to be executed when the command is invoked.</param>
        /// <param name="canExecute">The function that determines whether the command can be executed. If <c>null</c>, the command is always executable.</param>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="execute"/> parameter is <c>null</c>.</exception>
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">The parameter passed to the command (unused in this implementation).</param>
        /// <returns><c>true</c> if the command can execute; otherwise, <c>false</c>.</returns>
        public bool CanExecute(object parameter) => this.canExecute?.Invoke() ?? true;

        /// <summary>
        /// Executes the command's associated action.
        /// </summary>
        /// <param name="parameter">The parameter passed to the command (unused in this implementation).</param>
        public void Execute(object parameter) => this.execute();

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event, notifying listeners that the ability to execute has changed.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}