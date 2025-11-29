using ShopperMaui.Services;
using ShopperMaui.ViewModels.Commands;

namespace ShopperMaui.ViewModels;

public class SettingsViewModel : BaseViewModel {
	private readonly INavigationService _navigationService;

	public SettingsViewModel(INavigationService navigationService) {
		_navigationService = navigationService;
		Title = "Ustawienia";

		ManageCategoriesCommand = new RelayCommand(() => _ = NavigateToManageCategoriesAsync());
		ManageStoresCommand = new RelayCommand(() => _ = NavigateToManageStoresAsync());
	}

	public RelayCommand ManageCategoriesCommand { get; }

	public RelayCommand ManageStoresCommand { get; }

	private Task NavigateToManageCategoriesAsync()
		=> _navigationService.NavigateToAsync<ManageCategoriesViewModel>();

	private Task NavigateToManageStoresAsync()
		=> _navigationService.NavigateToAsync<ManageStoresViewModel>();
}
