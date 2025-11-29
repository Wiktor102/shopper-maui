using ShopperMaui.Helpers;
using ShopperMaui.Models;
using ShopperMaui.Services;
using ShopperMaui.ViewModels.Commands;
using System.Collections.ObjectModel;

namespace ShopperMaui.ViewModels;

public class AddProductViewModel : BaseViewModel, IQueryAttributable {
	private readonly MainViewModel _mainViewModel;
	private readonly INavigationService _navigationService;
	private Guid? _targetCategoryId;
	private string _productName = string.Empty;
	private decimal _quantity = 1;
	private string _selectedUnit = Constants.AvailableUnits.First();
	private bool _isOptional;
	private string? _storeName;
	private string? _selectedStoreSuggestion;
	private CategoryViewModel? _selectedCategory;

	public AddProductViewModel(MainViewModel mainViewModel, INavigationService navigationService) {
		_mainViewModel = mainViewModel;
		_navigationService = navigationService;
		Title = "Dodaj produkt";

		SaveProductCommand = new AsyncRelayCommand(SaveProductAsync, () => !IsBusy, busy => IsBusy = busy);
		CancelCommand = new RelayCommand(() => _ = _navigationService.GoBackAsync());
		IncreaseQuantityCommand = new RelayCommand(() => Quantity += 1);
		DecreaseQuantityCommand = new RelayCommand(() => {
			if (Quantity > 1) {
				Quantity -= 1;
			}
		});

		if (_mainViewModel.Categories.Any()) {
			SelectedCategory = _mainViewModel.Categories.First();
		}
	}

	public Product Model { get; private set; } = new();

	public ObservableCollection<CategoryViewModel> AvailableCategories => _mainViewModel.Categories;

	public IReadOnlyList<string> AvailableUnits => Constants.AvailableUnits;

	public IReadOnlyList<string> AvailableStores => Constants.DefaultStores;

	public string ProductName {
		get => _productName;
		set => SetProperty(ref _productName, value);
	}

	public decimal Quantity {
		get => _quantity;
		set => SetProperty(ref _quantity, Math.Max(0, value));
	}

	public string SelectedUnit {
		get => _selectedUnit;
		set => SetProperty(ref _selectedUnit, value);
	}

	public bool IsOptional {
		get => _isOptional;
		set => SetProperty(ref _isOptional, value);
	}

	public string? StoreName {
		get => _storeName;
		set => SetProperty(ref _storeName, value);
	}

	public string? SelectedStoreSuggestion {
		get => _selectedStoreSuggestion;
		set {
			if (SetProperty(ref _selectedStoreSuggestion, value) && !string.IsNullOrWhiteSpace(value)) {
				StoreName = value;
			}
		}
	}

	public CategoryViewModel? SelectedCategory {
		get => _selectedCategory;
		set {
			if (SetProperty(ref _selectedCategory, value)) {
				_targetCategoryId = value?.Model.Id;
			}
		}
	}

	public AsyncRelayCommand SaveProductCommand { get; }

	public RelayCommand CancelCommand { get; }

	public RelayCommand IncreaseQuantityCommand { get; }

	public RelayCommand DecreaseQuantityCommand { get; }

	public void ApplyQueryAttributes(IDictionary<string, object> query) {
		if (query.TryGetValue("categoryId", out var value)) {
			if (value is Guid guid) {
				_targetCategoryId = guid;
			} else if (value is string idString && Guid.TryParse(idString, out var parsed)) {
				_targetCategoryId = parsed;
			}

			SelectedCategory = _mainViewModel.Categories.FirstOrDefault(c => c.Model.Id == _targetCategoryId);
		}
	}

	private async Task SaveProductAsync() {
		if (string.IsNullOrWhiteSpace(ProductName)) {
			ErrorMessage = "Podaj nazwę produktu.";
			return;
		}

		if (Quantity <= 0) {
			ErrorMessage = "Ilość musi być większa od zera.";
			return;
		}

		if (_targetCategoryId is null) {
			ErrorMessage = "Wybierz kategorię.";
			return;
		}

		Model = new Product {
			Id = Guid.NewGuid(),
			Name = ProductName.Trim(),
			Quantity = Quantity,
			Unit = SelectedUnit,
			IsOptional = IsOptional,
			IsPurchased = false,
			StoreName = string.IsNullOrWhiteSpace(StoreName) ? null : StoreName?.Trim(),
			CategoryId = _targetCategoryId.Value
		};

		var result = await _mainViewModel.AddProductAsync(_targetCategoryId.Value, Model);
		if (!result) {
			ErrorMessage = "Nie udało się zapisać produktu.";
			return;
		}

		ProductName = string.Empty;
		Quantity = 1;
		StoreName = string.Empty;
		ErrorMessage = string.Empty;
		await _navigationService.GoBackAsync();
	}
}
