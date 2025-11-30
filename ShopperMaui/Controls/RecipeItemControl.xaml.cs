using System.Windows.Input;

namespace ShopperMaui.Controls;

public partial class RecipeItemControl : ContentView {
	public RecipeItemControl() {
		InitializeComponent();
	}

	public static readonly BindableProperty AddCommandProperty = BindableProperty.Create(
		nameof(AddCommand), typeof(ICommand), typeof(RecipeItemControl));

	public static readonly BindableProperty EditCommandProperty = BindableProperty.Create(
		nameof(EditCommand), typeof(ICommand), typeof(RecipeItemControl));

	public static readonly BindableProperty DeleteCommandProperty = BindableProperty.Create(
		nameof(DeleteCommand), typeof(ICommand), typeof(RecipeItemControl));

	public static readonly BindableProperty OpenDetailsCommandProperty = BindableProperty.Create(
		nameof(OpenDetailsCommand), typeof(ICommand), typeof(RecipeItemControl));

	public ICommand? AddCommand {
		get => (ICommand?)GetValue(AddCommandProperty);
		set => SetValue(AddCommandProperty, value);
	}

	public ICommand? EditCommand {
		get => (ICommand?)GetValue(EditCommandProperty);
		set => SetValue(EditCommandProperty, value);
	}

	public ICommand? DeleteCommand {
		get => (ICommand?)GetValue(DeleteCommandProperty);
		set => SetValue(DeleteCommandProperty, value);
	}

	public ICommand? OpenDetailsCommand {
		get => (ICommand?)GetValue(OpenDetailsCommandProperty);
		set => SetValue(OpenDetailsCommandProperty, value);
	}
}
