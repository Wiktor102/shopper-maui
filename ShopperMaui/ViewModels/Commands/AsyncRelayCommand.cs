using System.Windows.Input;

namespace ShopperMaui.ViewModels.Commands;

public class AsyncRelayCommand : ICommand {
	private readonly Func<object?, Task> _executeAsync;
	private readonly Func<object?, bool>? _canExecute;
	private readonly Action<bool>? _isBusyChanged;
	private bool _isExecuting;

	public AsyncRelayCommand(Func<Task> executeAsync, Func<bool>? canExecute = null, Action<bool>? isBusyChanged = null) {
		if (executeAsync is null) {
			throw new ArgumentNullException(nameof(executeAsync));
		}

		_executeAsync = _ => executeAsync();
		if (canExecute is not null) {
			_canExecute = _ => canExecute();
		}
		_isBusyChanged = isBusyChanged;
	}

	public AsyncRelayCommand(Func<object?, Task> executeAsync, Func<object?, bool>? canExecute = null, Action<bool>? isBusyChanged = null) {
		_executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
		_canExecute = canExecute;
		_isBusyChanged = isBusyChanged;
	}

	public event EventHandler? CanExecuteChanged;

	public bool CanExecute(object? parameter) => !_isExecuting && (_canExecute?.Invoke(parameter) ?? true);

	public async void Execute(object? parameter) => await ExecuteAsync(parameter);

	public async Task ExecuteAsync(object? parameter) {
		if (!CanExecute(parameter)) {
			return;
		}

		try {
			_isExecuting = true;
			_isBusyChanged?.Invoke(true);
			RaiseCanExecuteChanged();
			await _executeAsync(parameter);
		} finally {
			_isExecuting = false;
			_isBusyChanged?.Invoke(false);
			RaiseCanExecuteChanged();
		}
	}

	public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
