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
	private ShoppingList _currentShoppingList = new();
	private bool _isInitialized;
	private readonly object _syncRoot = new();

	public MainViewModel(IDataService dataService, INavigationService navigationService, IDialogService dialogService) {
		_dataService = dataService;
		_navigationService = navigationService;
		_dialogService = dialogService;

		Title = "Lista Zakupów";
		Categories = new ObservableCollection<CategoryViewModel>();
		Categories.CollectionChanged += OnCategoriesChanged;

		AddCategoryCommand = new RelayCommand(() => _ = NavigateToAddCategoryAsync());
		NavigateToUnpurchasedViewCommand = new RelayCommand(() => _ = NavigateToUnpurchasedViewAsync());
		NavigateToRecipesCommand = new RelayCommand(() => _ = NavigateToRecipesAsync());
		ExportListCommand = new AsyncRelayCommand(ExportListAsync, () => !IsBusy, busy => IsBusy = busy);
		ImportListCommand = new AsyncRelayCommand(ImportListAsync, () => !IsBusy, busy => IsBusy = busy);
	}

	public event EventHandler? ShoppingListUpdated;

	public ObservableCollection<CategoryViewModel> Categories { get; }

	public ShoppingList CurrentShoppingList {
		get => _currentShoppingList;
		private set => SetProperty(ref _currentShoppingList, value);
	}

	public RelayCommand AddCategoryCommand { get; }

	public RelayCommand NavigateToUnpurchasedViewCommand { get; }

	public RelayCommand NavigateToRecipesCommand { get; }

	public AsyncRelayCommand ExportListCommand { get; }

	public AsyncRelayCommand ImportListCommand { get; }

	public async Task InitializeAsync() {
		if (_isInitialized) {
			return;
		}

		await LoadAsync();
		_isInitialized = true;
	}

	public async Task LoadAsync() {
		if (IsBusy) {
			return;
		}

		try {
			IsBusy = true;
			ErrorMessage = string.Empty;
			CurrentShoppingList = await _dataService.LoadShoppingListAsync();
			if (!CurrentShoppingList.Categories.Any()) {
				foreach (var category in DefaultDataProvider.GetDefaultCategories()) {
					CurrentShoppingList.Categories.Add(category);
				}
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
		OnShoppingListUpdated();
	}

	private void OnCategoriesChanged(object? sender, NotifyCollectionChangedEventArgs e) {
		_ = SaveAsync();
	}

	private async Task ExportListAsync() {
		var filePath = await _dataService.ExportShoppingListAsync(CurrentShoppingList);
		await Share.RequestAsync(new ShareFileRequest {
			Title = "Eksport listy zakupów",
			File = new ShareFile(filePath)
		});
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

	private Task NavigateToUnpurchasedViewAsync()
		=> _navigationService.NavigateToAsync<UnpurchasedListViewModel>();

	private Task NavigateToRecipesAsync()
		=> _navigationService.NavigateToAsync<RecipesViewModel>();

	private void OnShoppingListUpdated()
		=> ShoppingListUpdated?.Invoke(this, EventArgs.Empty);
}
