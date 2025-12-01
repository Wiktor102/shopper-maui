namespace ShopperMaui.ViewModels.Commands;

public class AsyncRelayCommand : RelayCommand {
	private readonly Func<object?, Task> _executeAsync;
	private readonly Action<bool>? _isBusyChanged;
	private bool _isExecuting;

	public AsyncRelayCommand(Func<Task> executeAsync, Func<bool>? canExecute = null, Action<bool>? isBusyChanged = null)
		: this(_ => executeAsync(), canExecute is null ? null : _ => canExecute(), isBusyChanged) {
		ArgumentNullException.ThrowIfNull(executeAsync);
	}

	public AsyncRelayCommand(Func<object?, Task> executeAsync, Func<object?, bool>? canExecute = null, Action<bool>? isBusyChanged = null)
		: base(canExecute) {
		_executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
		_isBusyChanged = isBusyChanged;
	}

	public override bool CanExecute(object? parameter) => !_isExecuting && base.CanExecute(parameter);

	public override void Execute(object? parameter) => _ = ExecuteAsync(parameter);

	public async Task ExecuteAsync(object? parameter) {
		if (!CanExecute(parameter)) return;

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

	// RelayCommand already provides a public RaiseCanExecuteChanged method - reuse it.
}
