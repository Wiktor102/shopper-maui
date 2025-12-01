using System.Windows.Input;

namespace ShopperMaui.ViewModels.Commands;

public class RelayCommand : ICommand {
	private readonly Action<object?> _execute;
	private readonly Func<object?, bool>? _canExecute;

	public RelayCommand(Action execute, Func<bool>? canExecute = null) {
		if (execute is null) {
			throw new ArgumentNullException(nameof(execute));
		}

		_execute = _ => execute();
		if (canExecute is not null) {
			_canExecute = _ => canExecute();
		}
	}

	public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null) {
		_execute = execute ?? throw new ArgumentNullException(nameof(execute));
		_canExecute = canExecute;
	}

	// Protected constructor for derived classes that don't want to provide a synchronous
	// action to execute (e.g. AsyncRelayCommand will override Execute).
	protected RelayCommand(Func<object?, bool>? canExecute) {
		_execute = _ => { };
		_canExecute = canExecute;
	}

	public event EventHandler? CanExecuteChanged;

	public virtual bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

	public virtual void Execute(object? parameter) => _execute(parameter);

	public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
