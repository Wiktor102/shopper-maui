using ShopperMaui.Services;
using ShopperMaui.ViewModels.Commands;

namespace ShopperMaui.ViewModels;

public class SettingsViewModel : BaseViewModel {
	private readonly INavigationService _navigationService;

	public SettingsViewModel(INavigationService navigationService) {
		_navigationService = navigationService;
		Title = "Ustawienia";

		ManageDataCommand = new RelayCommand(() => _ = NavigateToManageDataAsync());
	}

	public RelayCommand ManageDataCommand { get; }

	public string AppVersion => "ShopperMaui v1.0";

	private Task NavigateToManageDataAsync()
		=> _navigationService.NavigateToAsync<ManageDataViewModel>();
}
