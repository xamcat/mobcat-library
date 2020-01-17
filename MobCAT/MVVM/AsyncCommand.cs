using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Microsoft.MobCAT.MVVM
{
    public class AsyncCommand : ICommand
    {
        bool _enableCoalescing;
        Func<object, Task> _action;
        readonly Func<object, bool> _canExecute;
        object lockObject = new object();
        Task _task;

        public AsyncCommand(Func<object, Task> action, bool enableCoalescing = false)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _enableCoalescing = enableCoalescing;
        }

        public AsyncCommand(Func<Task> action, bool enableCoalescing = false) : this(o => action(), enableCoalescing)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
        }

        public AsyncCommand(Func<object, Task> action, Func<object, bool> canExecute, bool enableCoalescing = false) : this(action, enableCoalescing)
        {
            this._canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        public AsyncCommand(Func<Task> action, Func<bool> canExecute, bool enableCoalescing = false) : this(o => action(), o => canExecute(), enableCoalescing)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (canExecute == null)
                throw new ArgumentNullException(nameof(canExecute));
        }

        /// <inheritdoc />
        public bool CanExecute(object parameter)
        {
            if (_task == null)
                return _canExecute == null ? true : _canExecute(parameter);

            return _canExecute == null ? _task.IsCompleted : _canExecute(parameter) && _task.IsCompleted;
        }

        public event EventHandler CanExecuteChanged;

        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Potential Code Quality Issues", "RECS0165:Asynchronous methods should return a Task instead of void", Justification = "Required to facilitate use of async code within the constraints of the ICommand interface")]
        public async void Execute(object parameter) => await ExecuteAsync(parameter).ConfigureAwait(false);

        /// <summary>
        /// Raises the CanExecuteChanged event if there is a handler.
        /// </summary>
        void OnCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Executes the command is invoked.
        /// </summary>
        /// <returns>Task with result of type object.</returns>
        /// <param name="parameter">Parameter.</param>
        public Task ExecuteAsync(object parameter)
        {
            if (!CanExecute(parameter))
                return Task.FromResult(false);

            if (_enableCoalescing)
            {
                lock (lockObject)
                {
                    if (_task == null || _task.IsCompleted)
                        _task = ExecuteTask(parameter);
                }

                return _task;
            }

            return ExecuteTask(parameter);
        }

        public void ChangeCanExecute()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        async Task ExecuteTask(object parameter)
        {
            OnCanExecuteChanged();
            await _action(parameter);
            OnCanExecuteChanged();
        }
    }
} 