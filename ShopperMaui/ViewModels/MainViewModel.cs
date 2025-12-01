using ShopperMaui.Helpers;
using ShopperMaui.Models;
using ShopperMaui.Services;
using ShopperMaui.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ShopperMaui.ViewModels;

public class MainViewModel : BaseViewModel {
	private readonly IDataService _dataService;
	private readonly INavigationService _navigationService;
	private readonly IDialogService _dialogService;
	private readonly IFileSaver _fileSaver;
	private ShoppingList _currentShoppingList = new();
	private bool _isInitialized;
	private readonly object _syncRoot = new();

	public MainViewModel(IDataService dataService, INavigationService navigationService, IDialogService dialogService, IFileSaver fileSaver) {
		_dataService = dataService;
		_navigationService = navigationService;
		_dialogService = dialogService;
		_fileSaver = fileSaver;

		Title = "Lista Zakupów";
		Categories = [];
		Categories.CollectionChanged += OnCategoriesChanged;

		Stores = [];
		Stores.CollectionChanged += OnStoresChanged;

		AddCategoryCommand = new RelayCommand(() => _ = NavigateToAddCategoryAsync());
		NavigateToUnpurchasedViewCommand = new RelayCommand(() => _ = NavigateToUnpurchasedViewAsync());
		NavigateToRecipesCommand = new RelayCommand(() => _ = NavigateToRecipesAsync());
		AddProductCommand = new AsyncRelayCommand(NavigateToAddProductAsync, () => !IsBusy, busy => IsBusy = busy);
		ExportListCommand = new AsyncRelayCommand(ExportListAsync, () => !IsBusy, busy => IsBusy = busy);
		ImportListCommand = new AsyncRelayCommand(ImportListAsync, () => !IsBusy, busy => IsBusy = busy);
	}

	public event EventHandler? ShoppingListUpdated;

	public ObservableCollection<CategoryViewModel> Categories { get; }

	public ObservableCollection<CategoryViewModel> VisibleCategories { get; } = new();

	public bool HasVisibleCategories => VisibleCategories.Any();

	public ObservableCollection<string> Stores { get; }

	public bool HasStores => Stores.Any();

	public ShoppingList CurrentShoppingList {
		get => _currentShoppingList;
		private set => SetProperty(ref _currentShoppingList, value);
	}

	public RelayCommand AddCategoryCommand { get; }

	public RelayCommand NavigateToUnpurchasedViewCommand { get; }

	public RelayCommand NavigateToRecipesCommand { get; }

	public AsyncRelayCommand AddProductCommand { get; }

	public AsyncRelayCommand ExportListCommand { get; }

	public AsyncRelayCommand ImportListCommand { get; }

	public async Task InitializeAsync() {
		if (_isInitialized) return;

		await LoadAsync();
		_isInitialized = true;
	}

	public async Task LoadAsync() {
		if (IsBusy) return;

		try {
			IsBusy = true;
			ErrorMessage = string.Empty;
			CurrentShoppingList = await _dataService.LoadShoppingListAsync();
			if (!CurrentShoppingList.Categories.Any()) {
				foreach (var category in DefaultDataProvider.GetDefaultCategories()) {
					CurrentShoppingList.Categories.Add(category);
				}
			}

			if (CurrentShoppingList.Stores is null || !CurrentShoppingList.Stores.Any()) {
				CurrentShoppingList.Stores = new ObservableCollection<string>(Constants.DefaultStores);
			}

			BuildCategoryViewModels();
		} catch (Exception ex) {
			ErrorMessage = ex.Message;
		} finally {
			IsBusy = false;
		}
	}

	internal async Task SaveAsync() {
		lock (_syncRoot) {
			foreach (var (category, index) in Categories.Select((category, index) => (category, index))) {
				category.UpdateSortOrder(index);
				category.SyncModel();
			}

			CurrentShoppingList.Stores ??= new ObservableCollection<string>();
			CurrentShoppingList.Stores.Clear();
			foreach (var store in Stores) {
				var normalized = NormalizeStoreName(store);
				if (normalized is null) {
					continue;
				}

				CurrentShoppingList.Stores.Add(normalized);
			}

			CurrentShoppingList.LastModified = DateTime.UtcNow;
		}

		await _dataService.SaveShoppingListAsync(CurrentShoppingList);
		OnShoppingListUpdated();
	}

	public IEnumerable<ProductViewModel> GetAllProducts()
		=> Categories.SelectMany(c => c.Products).ToList();

	public bool CategoryExists(string categoryName)
		=> Categories.Any(c => string.Equals(c.Name, categoryName, StringComparison.OrdinalIgnoreCase));

	public async Task<bool> AddCategoryAsync(string categoryName) {
		if (string.IsNullOrWhiteSpace(categoryName) || CategoryExists(categoryName)) {
			return false;
		}

		var newCategory = new Category {
			Name = categoryName.Trim(),
			SortOrder = Categories.Count
		};

		CurrentShoppingList.Categories.Add(newCategory);
		var categoryViewModel = new CategoryViewModel(newCategory, this, _dialogService);
		Categories.Add(categoryViewModel);
		await SaveAsync();
		return true;
	}

	public bool StoreExists(string storeName) {
		var normalized = NormalizeStoreName(storeName);
		return normalized is not null && Stores.Any(store => StoreNamesEqual(store, normalized));
	}

	public async Task<bool> AddStoreAsync(string storeName) {
		var normalized = NormalizeStoreName(storeName);
		if (normalized is null) {
			await _dialogService.ShowAlertAsync("Nieprawidłowa nazwa", "Podaj nazwę sklepu.");
			return false;
		}

		if (StoreExists(normalized)) {
			await _dialogService.ShowAlertAsync("Sklep istnieje", $"Sklep \"{normalized}\" już istnieje.");
			return false;
		}

		Stores.Add(normalized);
		await SaveAsync();
		return true;
	}

	public async Task<bool> RenameStoreAsync(string existingName, string newName) {
		if (!TryFindStore(existingName, out var index)) {
			await _dialogService.ShowAlertAsync("Nie znaleziono sklepu", "Wybrany sklep nie istnieje na liście.");
			return false;
		}

		var normalized = NormalizeStoreName(newName);
		if (normalized is null) {
			await _dialogService.ShowAlertAsync("Nieprawidłowa nazwa", "Podaj nazwę sklepu.");
			return false;
		}

		if (Stores.Where((_, idx) => idx != index).Any(store => StoreNamesEqual(store, normalized))) {
			await _dialogService.ShowAlertAsync("Sklep istnieje", $"Sklep \"{normalized}\" już istnieje.");
			return false;
		}

		var originalName = Stores[index];
		if (string.Equals(originalName, normalized, StringComparison.Ordinal)) {
			return true;
		}

		Stores[index] = normalized;
		UpdateProductsStoreReference(originalName, normalized);
		await SaveAsync();
		return true;
	}

	public async Task<bool> RemoveStoreAsync(string storeName) {
		if (!TryFindStore(storeName, out var index)) {
			return false;
		}

		var originalName = Stores[index];
		Stores.RemoveAt(index);
		UpdateProductsStoreReference(originalName, null);
		await SaveAsync();
		return true;
	}

	private bool TryFindStore(string storeName, out int index) {
		var normalized = NormalizeStoreName(storeName);
		if (normalized is null) {
			index = -1;
			return false;
		}

		for (var i = 0; i < Stores.Count; i++) {
			if (StoreNamesEqual(Stores[i], normalized)) {
				index = i;
				return true;
			}
		}

		index = -1;
		return false;
	}

	private void UpdateProductsStoreReference(string oldName, string? newName) {
		var normalizedNewName = NormalizeStoreName(newName);
		foreach (var category in Categories) {
			foreach (var product in category.Products) {
				if (product.StoreName is null) {
					continue;
				}

				if (StoreNamesEqual(product.StoreName, oldName)) {
					product.UpdateStoreNameFromManager(normalizedNewName);
				}
			}
		}
	}

	public async Task<bool> AddProductAsync(Guid categoryId, Product product) {
		var categoryViewModel = Categories.FirstOrDefault(c => c.Model.Id == categoryId);
		if (categoryViewModel is null) {
			return false;
		}

		categoryViewModel.AddProduct(product);
		await SaveAsync();
		return true;
	}

	internal Task NavigateToAddProductAsync(CategoryViewModel category)
		=> _navigationService.NavigateToAsync<AddProductViewModel>(new Dictionary<string, object> {
			["categoryId"] = category.Model.Id
		});

	private async Task NavigateToAddProductAsync() {
		if (!Categories.Any()) {
			await _dialogService.ShowAlertAsync("Brak kategorii", "Dodaj kategorię, aby móc dodać produkt.");
			return;
		}

		await _navigationService.NavigateToAsync<AddProductViewModel>();
	}

	internal Task NavigateToAddCategoryAsync()
		=> _navigationService.NavigateToAsync<AddCategoryViewModel>();

	internal async Task RemoveCategoryAsync(CategoryViewModel category) {
		Categories.Remove(category);
		CurrentShoppingList.Categories.Remove(category.Model);
		await SaveAsync();
	}

	internal async Task DeleteProductAsync(ProductViewModel productViewModel) {
		productViewModel.ParentCategory.RemoveProduct(productViewModel);
		await SaveAsync();
	}

	internal async Task HandleProductChangedAsync(ProductViewModel productViewModel) {
		productViewModel.ParentCategory.RefreshProductOrdering();
		await SaveAsync();
	}

	private void BuildCategoryViewModels() {
		Categories.CollectionChanged -= OnCategoriesChanged;
		Categories.Clear();

		foreach (var category in CurrentShoppingList.Categories.OrderBy(c => c.SortOrder).ThenBy(c => c.Name)) {
			Categories.Add(new CategoryViewModel(category, this, _dialogService));
		}

		Categories.CollectionChanged += OnCategoriesChanged;
		RefreshVisibleCategories();
		RefreshStores();
		OnShoppingListUpdated();
	}

	private void OnCategoriesChanged(object? sender, NotifyCollectionChangedEventArgs e) {
		RefreshVisibleCategories();
		_ = SaveAsync();
	}

	private void OnStoresChanged(object? sender, NotifyCollectionChangedEventArgs e)
		=> OnPropertyChanged(nameof(HasStores));

	internal void NotifyCategoryProductsChanged() => RefreshVisibleCategories();

	private void RefreshVisibleCategories() {
		VisibleCategories.Clear();
		foreach (var category in Categories.Where(c => c.HasProducts)
				.OrderBy(c => c.Model.SortOrder)
				.ThenBy(c => c.Name)) {
			VisibleCategories.Add(category);
		}

		OnPropertyChanged(nameof(HasVisibleCategories));
	}

	private void RefreshStores() {
		Stores.Clear();
		if (CurrentShoppingList.Stores is null) {
			CurrentShoppingList.Stores = new ObservableCollection<string>();
		}

		foreach (var store in CurrentShoppingList.Stores) {
			var normalized = NormalizeStoreName(store);
			if (normalized is null) {
				continue;
			}

			if (!Stores.Any(existing => StoreNamesEqual(existing, normalized))) {
				Stores.Add(normalized);
			}
		}

		var storesFromProducts = CurrentShoppingList.Categories
			.SelectMany(category => category.Products)
			.Select(product => NormalizeStoreName(product.StoreName))
			.Where(static name => name is not null)
			.Cast<string>();

		foreach (var store in storesFromProducts) {
			if (!Stores.Any(existing => StoreNamesEqual(existing, store))) {
				Stores.Add(store);
			}
		}

		OnPropertyChanged(nameof(Stores));
		OnPropertyChanged(nameof(HasStores));
	}

	private async Task ExportListAsync() {
		var xml = XmlSerializationHelper.SerializeToXml(CurrentShoppingList);
		var fileName = $"shopping_list_{DateTime.UtcNow:yyyyMMddHHmmss}.xml";

		var filePath = await _fileSaver.SaveAsync(fileName, xml);

		if (filePath != null) {
			await _dialogService.ShowAlertAsync("Sukces", $"Zapisano plik: {filePath}");
		}
	}

	private async Task ImportListAsync() {
		var fileResult = await FilePicker.PickAsync(new PickOptions {
			PickerTitle = "Wybierz plik listy"
		});

		if (fileResult is null) {
			return;
		}

		CurrentShoppingList = await _dataService.ImportShoppingListAsync(fileResult.FullPath);
		BuildCategoryViewModels();
	}

	private static string? NormalizeStoreName(string? storeName)
		=> string.IsNullOrWhiteSpace(storeName) ? null : storeName.Trim();

	private static bool StoreNamesEqual(string? left, string? right)
		=> string.Equals(left, right, StringComparison.OrdinalIgnoreCase);

	private Task NavigateToUnpurchasedViewAsync()
		=> _navigationService.NavigateToAsync<UnpurchasedListViewModel>();

	private Task NavigateToRecipesAsync()
		=> _navigationService.NavigateToAsync<RecipesViewModel>();

	private void OnShoppingListUpdated()
		=> ShoppingListUpdated?.Invoke(this, EventArgs.Empty);
}
