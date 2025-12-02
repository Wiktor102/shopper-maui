using ShopperMaui.Helpers;
using ShopperMaui.Models;
using ShopperMaui.Services.Interfaces;
using ShopperMaui.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

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
	private CategoryViewModel? _selectedCategory;
	private ProductViewModel? _editingProduct;
	private bool _isEditMode;
	private string _pageTitle = "Dodaj produkt";

	public AddProductViewModel(MainViewModel mainViewModel, INavigationService navigationService) {
		_mainViewModel = mainViewModel;
		_navigationService = navigationService;
		Title = _pageTitle;

		SaveProductCommand = new AsyncRelayCommand(SaveProductAsync, () => !IsBusy, busy => IsBusy = busy);
		CancelCommand = new RelayCommand(() => _ = _navigationService.GoBackAsync());
		IncreaseQuantityCommand = new RelayCommand(() => Quantity += 1);
		DecreaseQuantityCommand = new RelayCommand(() => {
			if (Quantity > 1) {
				Quantity -= 1;
			}
		});
		ClearStoreSelectionCommand = new RelayCommand(() => StoreName = null);

		_mainViewModel.Stores.CollectionChanged += OnStoresCollectionChanged;

		if (_mainViewModel.Categories.Any()) {
			SelectedCategory = _mainViewModel.Categories.First();
		}
	}

	public Product Model { get; private set; } = new();

	public string PageTitle {
		get => _pageTitle;
		private set => SetProperty(ref _pageTitle, value);
	}

	public ObservableCollection<CategoryViewModel> AvailableCategories => _mainViewModel.Categories;

	public IReadOnlyList<string> AvailableUnits => Constants.AvailableUnits;

	public ObservableCollection<string> AvailableStores => _mainViewModel.Stores;

	public bool HasStores => _mainViewModel.Stores.Any();

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

	public RelayCommand ClearStoreSelectionCommand { get; }

	public void ApplyQueryAttributes(IDictionary<string, object> query) {
		if (TryExtractGuid(query, "productId", out var productId)) {
			LoadProductForEdit(productId);
			return;
		}

		_isEditMode = false;
		_editingProduct = null;
		PageTitle = "Dodaj produkt";
		Title = PageTitle;

		if (TryExtractGuid(query, "categoryId", out var categoryId)) {
			_targetCategoryId = categoryId;
			SelectedCategory = _mainViewModel.Categories.FirstOrDefault(c => c.Model.Id == _targetCategoryId);
		} else if (SelectedCategory is null && _mainViewModel.Categories.Any()) {
			SelectedCategory = _mainViewModel.Categories.First();
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

		if (_isEditMode) {
			await UpdateExistingProductAsync();
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
		StoreName = null;
		ErrorMessage = string.Empty;
		await _navigationService.GoBackAsync();
	}

	private async Task UpdateExistingProductAsync() {
		if (_editingProduct is null || _targetCategoryId is null) {
			ErrorMessage = "Nie udało się zaktualizować produktu.";
			return;
		}

		var normalizedStore = string.IsNullOrWhiteSpace(StoreName) ? null : StoreName.Trim();
		_editingProduct.Name = ProductName.Trim();
		_editingProduct.Quantity = Quantity;
		_editingProduct.Unit = SelectedUnit;
		_editingProduct.IsOptional = IsOptional;
		_editingProduct.StoreName = normalizedStore;

		var targetCategory = _mainViewModel.Categories.FirstOrDefault(c => c.Model.Id == _targetCategoryId);
		if (targetCategory is null) {
			ErrorMessage = "Nie znaleziono kategorii.";
			return;
		}

		if (!ReferenceEquals(_editingProduct.ParentCategory, targetCategory)) {
			await _mainViewModel.MoveProductAsync(_editingProduct, targetCategory);
		} else {
			await _mainViewModel.SaveAsync();
		}

		await _navigationService.GoBackAsync();
	}

	private void LoadProductForEdit(Guid productId) {
		var product = _mainViewModel
			.GetAllProducts()
			.FirstOrDefault(p => p.Model.Id == productId);

		if (product is null) {
			ErrorMessage = "Nie znaleziono produktu do edycji.";
			return;
		}

		_editingProduct = product;
		_isEditMode = true;
		PageTitle = "Edytuj produkt";
		Title = PageTitle;
		ProductName = product.Name;
		Quantity = product.Quantity;
		SelectedUnit = product.Unit;
		IsOptional = product.IsOptional;
		StoreName = product.StoreName;
		SelectedCategory = product.ParentCategory;
		_targetCategoryId = product.ParentCategory.Model.Id;
	}

	private static bool TryExtractGuid(IDictionary<string, object> query, string key, out Guid guid) {
		if (query.TryGetValue(key, out var value)) {
			if (value is Guid direct) {
				guid = direct;
				return true;
			}

			if (value is string stringValue && Guid.TryParse(stringValue, out var parsed)) {
				guid = parsed;
				return true;
			}
		}

		guid = Guid.Empty;
		return false;
	}

	private void OnStoresCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
		OnPropertyChanged(nameof(HasStores));
		OnPropertyChanged(nameof(AvailableStores));

		if (StoreName is not null && !_mainViewModel.Stores.Any(store => string.Equals(store, StoreName, StringComparison.OrdinalIgnoreCase))) {
			StoreName = null;
		}
	}
}
