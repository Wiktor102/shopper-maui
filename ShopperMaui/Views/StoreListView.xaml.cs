using ShopperMaui.Helpers;
using ShopperMaui.ViewModels;

namespace ShopperMaui.Views;

public partial class StoreListView : ContentPage {
	private StoreListViewModel ViewModel => (StoreListViewModel)BindingContext;

	public StoreListView() {
		InitializeComponent();
		BindingContext = ServiceHelper.GetService<StoreListViewModel>();
	}

	protected override async void OnAppearing() {
		base.OnAppearing();
		await ViewModel.InitializeAsync();
	}
}
