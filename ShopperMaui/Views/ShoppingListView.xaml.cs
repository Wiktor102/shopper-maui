using ShopperMaui.Helpers;
using ShopperMaui.ViewModels;

namespace ShopperMaui.Views;

public partial class ShoppingListView : ContentPage {
	private MainViewModel ViewModel => (MainViewModel)BindingContext;

	public ShoppingListView() {
		InitializeComponent();
		BindingContext = ServiceHelper.GetService<MainViewModel>();
	}

	protected override async void OnAppearing() {
		base.OnAppearing();
		await ViewModel.InitializeAsync();
	}
}
