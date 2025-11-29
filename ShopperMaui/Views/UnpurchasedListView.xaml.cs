using ShopperMaui.Helpers;
using ShopperMaui.ViewModels;

namespace ShopperMaui.Views;

public partial class UnpurchasedListView : ContentPage {
	private UnpurchasedListViewModel ViewModel => (UnpurchasedListViewModel)BindingContext;

	public UnpurchasedListView() {
		InitializeComponent();
		BindingContext = ServiceHelper.GetService<UnpurchasedListViewModel>();
	}

	protected override async void OnAppearing() {
		base.OnAppearing();
		await ViewModel.InitializeAsync();
	}
}
