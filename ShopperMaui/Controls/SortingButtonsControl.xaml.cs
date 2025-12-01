using System.Windows.Input;

namespace ShopperMaui.Controls;

public partial class SortingButtonsControl : ContentView {
	public static readonly BindableProperty SortByCategoryCommandProperty =
		BindableProperty.Create(
			nameof(SortByCategoryCommand),
			typeof(ICommand),
			typeof(SortingButtonsControl));

	public static readonly BindableProperty SortByNameCommandProperty =
		BindableProperty.Create(
			nameof(SortByNameCommand),
			typeof(ICommand),
			typeof(SortingButtonsControl));

	public static readonly BindableProperty SortByQuantityCommandProperty =
		BindableProperty.Create(
			nameof(SortByQuantityCommand),
			typeof(ICommand),
			typeof(SortingButtonsControl));

	public ICommand SortByCategoryCommand {
		get => (ICommand)GetValue(SortByCategoryCommandProperty);
		set => SetValue(SortByCategoryCommandProperty, value);
	}

	public ICommand SortByNameCommand {
		get => (ICommand)GetValue(SortByNameCommandProperty);
		set => SetValue(SortByNameCommandProperty, value);
	}

	public ICommand SortByQuantityCommand {
		get => (ICommand)GetValue(SortByQuantityCommandProperty);
		set => SetValue(SortByQuantityCommandProperty, value);
	}

	public SortingButtonsControl() {
		InitializeComponent();
	}
}
