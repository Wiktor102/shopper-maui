using System.Collections;
using System.Windows.Input;

namespace ShopperMaui.Controls;

public partial class EditableItemListControl : ContentView {
	public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
		nameof(ItemsSource),
		typeof(IEnumerable),
		typeof(EditableItemListControl),
		null,
		propertyChanged: OnItemsSourceChanged);

	public static readonly BindableProperty AddCommandProperty = BindableProperty.Create(
		nameof(AddCommand),
		typeof(ICommand),
		typeof(EditableItemListControl),
		null);

	public static readonly BindableProperty AddButtonTextProperty = BindableProperty.Create(
		nameof(AddButtonText),
		typeof(string),
		typeof(EditableItemListControl),
		"+ Dodaj");

	public static readonly BindableProperty ItemPlaceholderProperty = BindableProperty.Create(
		nameof(ItemPlaceholder),
		typeof(string),
		typeof(EditableItemListControl),
		"Nazwa");

	public static readonly BindableProperty ShowSubtitleProperty = BindableProperty.Create(
		nameof(ShowSubtitle),
		typeof(bool),
		typeof(EditableItemListControl),
		true);

	public static readonly BindableProperty EmptyStateIconProperty = BindableProperty.Create(
		nameof(EmptyStateIcon),
		typeof(string),
		typeof(EditableItemListControl),
		"📋");

	public static readonly BindableProperty EmptyStateTitleProperty = BindableProperty.Create(
		nameof(EmptyStateTitle),
		typeof(string),
		typeof(EditableItemListControl),
		"Brak elementów");

	public static readonly BindableProperty EmptyStateDescriptionProperty = BindableProperty.Create(
		nameof(EmptyStateDescription),
		typeof(string),
		typeof(EditableItemListControl),
		"Dodaj pierwszy element, aby rozpocząć.");

	public static readonly BindableProperty HasItemsProperty = BindableProperty.Create(
		nameof(HasItems),
		typeof(bool),
		typeof(EditableItemListControl),
		false);

	public EditableItemListControl() {
		InitializeComponent();
	}

	public IEnumerable ItemsSource {
		get => (IEnumerable)GetValue(ItemsSourceProperty);
		set => SetValue(ItemsSourceProperty, value);
	}

	public ICommand AddCommand {
		get => (ICommand)GetValue(AddCommandProperty);
		set => SetValue(AddCommandProperty, value);
	}

	public string AddButtonText {
		get => (string)GetValue(AddButtonTextProperty);
		set => SetValue(AddButtonTextProperty, value);
	}

	public string ItemPlaceholder {
		get => (string)GetValue(ItemPlaceholderProperty);
		set => SetValue(ItemPlaceholderProperty, value);
	}

	public bool ShowSubtitle {
		get => (bool)GetValue(ShowSubtitleProperty);
		set => SetValue(ShowSubtitleProperty, value);
	}

	public string EmptyStateIcon {
		get => (string)GetValue(EmptyStateIconProperty);
		set => SetValue(EmptyStateIconProperty, value);
	}

	public string EmptyStateTitle {
		get => (string)GetValue(EmptyStateTitleProperty);
		set => SetValue(EmptyStateTitleProperty, value);
	}

	public string EmptyStateDescription {
		get => (string)GetValue(EmptyStateDescriptionProperty);
		set => SetValue(EmptyStateDescriptionProperty, value);
	}

	public bool HasItems {
		get => (bool)GetValue(HasItemsProperty);
		set => SetValue(HasItemsProperty, value);
	}

	private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue) {
		if (bindable is EditableItemListControl control) {
			control.UpdateHasItems();

			if (oldValue is System.Collections.Specialized.INotifyCollectionChanged oldCollection) {
				oldCollection.CollectionChanged -= control.OnCollectionChanged;
			}

			if (newValue is System.Collections.Specialized.INotifyCollectionChanged newCollection) {
				newCollection.CollectionChanged += control.OnCollectionChanged;
			}
		}
	}

	private void OnCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
		UpdateHasItems();
	}

	private void UpdateHasItems() {
		HasItems = ItemsSource?.Cast<object>().Any() ?? false;
	}
}
