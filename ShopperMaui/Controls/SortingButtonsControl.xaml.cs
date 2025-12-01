using ShopperMaui.Models;
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

	public static readonly BindableProperty CurrentSortingProperty =
		BindableProperty.Create(
			nameof(CurrentSorting),
			typeof(SortingPreference),
			typeof(SortingButtonsControl),
			SortingPreference.Category,
			propertyChanged: OnCurrentSortingChanged);

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

	public SortingPreference CurrentSorting {
		get => (SortingPreference)GetValue(CurrentSortingProperty);
		set => SetValue(CurrentSortingProperty, value);
	}

	public bool IsCategorySelected => CurrentSorting == SortingPreference.Category;
	public bool IsNameSelected => CurrentSorting == SortingPreference.Name;
	public bool IsQuantitySelected => CurrentSorting == SortingPreference.Quantity;

	public SortingButtonsControl() {
		InitializeComponent();
	}

	private static void OnCurrentSortingChanged(BindableObject bindable, object oldValue, object newValue) {
		var control = (SortingButtonsControl)bindable;
		control.OnPropertyChanged(nameof(IsCategorySelected));
		control.OnPropertyChanged(nameof(IsNameSelected));
		control.OnPropertyChanged(nameof(IsQuantitySelected));
	}
}
