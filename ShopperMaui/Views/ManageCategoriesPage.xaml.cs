using ShopperMaui.Helpers;
using ShopperMaui.ViewModels;

namespace ShopperMaui.Views;

public partial class ManageCategoriesPage : ContentPage {
	private ManageCategoriesViewModel ViewModel => (ManageCategoriesViewModel)BindingContext;

	public ManageCategoriesPage() {
		InitializeComponent();
		BindingContext = ServiceHelper.GetService<ManageCategoriesViewModel>();
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
