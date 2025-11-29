using ShopperMaui.Services;
using ShopperMaui.ViewModels.Commands;

namespace ShopperMaui.ViewModels;

public class AddCategoryViewModel : BaseViewModel {
	private readonly MainViewModel _mainViewModel;
	private readonly INavigationService _navigationService;
	private string _categoryName = string.Empty;

	public AddCategoryViewModel(MainViewModel mainViewModel, INavigationService navigationService) {
		_mainViewModel = mainViewModel;
		_navigationService = navigationService;
		Title = "Dodaj kategorię";

		SaveCategoryCommand = new AsyncRelayCommand(SaveCategoryAsync, () => !IsBusy, busy => IsBusy = busy);
		CancelCommand = new RelayCommand(() => _ = _navigationService.GoBackAsync());
	}

	public string CategoryName {
		get => _categoryName;
		set => SetProperty(ref _categoryName, value);
	}

	public AsyncRelayCommand SaveCategoryCommand { get; }

	public RelayCommand CancelCommand { get; }

	private async Task SaveCategoryAsync() {
		if (string.IsNullOrWhiteSpace(CategoryName)) {
			ErrorMessage = "Podaj nazwę kategorii.";
			return;
		}

		if (_mainViewModel.CategoryExists(CategoryName)) {
			ErrorMessage = "Taka kategoria już istnieje.";
			return;
		}

		var success = await _mainViewModel.AddCategoryAsync(CategoryName.Trim());
		if (!success) {
			ErrorMessage = "Nie udało się dodać kategorii.";
			return;
		}

		CategoryName = string.Empty;
		ErrorMessage = string.Empty;
		await _navigationService.GoBackAsync();
	}
}
