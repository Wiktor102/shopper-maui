using ShopperMaui.Helpers;
using ShopperMaui.ViewModels;
using System.Windows.Input;

namespace ShopperMaui.Views;

public partial class ManageDataPage : ContentPage {
	private ManageDataViewModel ViewModel => (ManageDataViewModel)BindingContext;

	public ICommand SelectCategoriesTabCommand { get; }
	public ICommand SelectStoresTabCommand { get; }

	public ManageDataPage() {
		SelectCategoriesTabCommand = new Command(() => ViewModel.SelectedTabIndex = 0);
		SelectStoresTabCommand = new Command(() => ViewModel.SelectedTabIndex = 1);

		InitializeComponent();
		BindingContext = ServiceHelper.GetService<ManageDataViewModel>();
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
