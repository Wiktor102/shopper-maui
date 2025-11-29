using ShopperMaui.Helpers;
using ShopperMaui.ViewModels;

namespace ShopperMaui.Views;

public partial class ManageStoresPage : ContentPage {
	private ManageStoresViewModel ViewModel => (ManageStoresViewModel)BindingContext;

	public ManageStoresPage() {
		InitializeComponent();
		BindingContext = ServiceHelper.GetService<ManageStoresViewModel>();
	}

	protected override async void OnAppearing() {
		base.OnAppearing();
		await ViewModel.InitializeAsync();
		ViewModel.Attach();
	}

	protected override void OnDisappearing() {
		base.OnDisappearing();
		ViewModel.Detach();
	}
}
