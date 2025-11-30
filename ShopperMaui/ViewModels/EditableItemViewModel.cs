using System.Windows.Input;
using ShopperMaui.ViewModels.Commands;

namespace ShopperMaui.ViewModels;

/// <summary>
/// A unified wrapper for editable list items (categories, stores, etc.)
/// Provides a common interface for the EditableItemListControl.
/// </summary>
public class EditableItemViewModel : BaseViewModel {
	private string _name;
	private readonly string? _subtitle;
	private readonly Func<string, Task>? _onNameChanged;

	public EditableItemViewModel(
		string name,
		string? subtitle,
		Func<Task> deleteAction,
		Func<string, Task>? onNameChanged = null) {
		_name = name;
		_subtitle = subtitle;
		_onNameChanged = onNameChanged;
		DeleteCommand = new AsyncRelayCommand(deleteAction);
	}

	public string Name {
		get => _name;
		set {
			if (SetProperty(ref _name, value) && _onNameChanged != null) {
				_ = _onNameChanged(value);
			}
		}
	}

	public string? Subtitle => _subtitle;

	public ICommand DeleteCommand { get; }
}
