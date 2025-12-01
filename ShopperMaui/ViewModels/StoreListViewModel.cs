using ShopperMaui.Helpers;
using ShopperMaui.Models;
using ShopperMaui.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ShopperMaui.ViewModels;

public class StoreListViewModel : BaseViewModel {
	private readonly MainViewModel _mainViewModel;
	private string _selectedStore;
	private SortingPreference _currentSorting = SortingPreference.Category;
	private bool _isUpdatingSelection;

	public StoreListViewModel(MainViewModel mainViewModel) {
		_mainViewModel = mainViewModel;
		Title = "Widok sklepu";
		_selectedStore = AllStoresLabel;

		FilteredProducts = new ObservableCollection<ProductViewModel>();
		AvailableStores = new ObservableCollection<string>();
		RefreshAvailableStores();

		SortByCategoryCommand = new RelayCommand(() => UpdateSorting(SortingPreference.Category));
		SortByNameCommand = new RelayCommand(() => UpdateSorting(SortingPreference.Name));
		SortByQuantityCommand = new RelayCommand(() => UpdateSorting(SortingPreference.Quantity));

		_mainViewModel.ShoppingListUpdated += (_, _) => {
			RefreshAvailableStores();
			RefreshProducts();
		};
		_mainViewModel.Stores.CollectionChanged += OnStoresCollectionChanged;
	}

	private const string AllStoresLabel = "Wszystkie sklepy";

	public ObservableCollection<ProductViewModel> FilteredProducts { get; }

	public ObservableCollection<string> AvailableStores { get; }

	public string SelectedStore {
		get => _selectedStore;
		set {
			if (_isUpdatingSelection) {
				if (!string.Equals(_selectedStore, value, StringComparison.Ordinal)) {
					_selectedStore = value;
					OnPropertyChanged(nameof(SelectedStore));
				}
				return;
			}

			if (SetProperty(ref _selectedStore, value)) {
				RefreshProducts();
			}
		}
	}

	public SortingPreference CurrentSorting {
		get => _currentSorting;
		set {
			if (SetProperty(ref _currentSorting, value)) {
				RefreshProducts();
			}
		}
	}

	public RelayCommand SortByCategoryCommand { get; }

	public RelayCommand SortByNameCommand { get; }

	public RelayCommand SortByQuantityCommand { get; }

	public Task InitializeAsync() {
		RefreshProducts();
		return Task.CompletedTask;
	}

	private void OnStoresCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
		RefreshAvailableStores();
		RefreshProducts();
	}

	private void UpdateSorting(SortingPreference preference) {
		CurrentSorting = preference;
	}

	private void RefreshProducts() {
		EnsureSelectedStoreIsValid();
		var filtered = _mainViewModel.GetAllProducts()
			.Where(p => FilterByStore(p))
			.Where(p => !string.IsNullOrWhiteSpace(p.Name))
			.ToList();

		filtered = CurrentSorting switch {
			SortingPreference.Name => SortingHelper.SortByName(filtered),
			SortingPreference.Quantity => SortingHelper.SortByQuantity(filtered),
			_ => SortingHelper.SortByCategory(filtered)
		};

		FilteredProducts.Clear();
		foreach (var product in filtered) {
			FilteredProducts.Add(product);
		}
	}

	private void RefreshAvailableStores() {
		var previousSelection = _selectedStore;
		var uniqueStores = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		AvailableStores.Clear();
		AvailableStores.Add(AllStoresLabel);
		uniqueStores.Add(AllStoresLabel);

		foreach (var store in _mainViewModel.Stores.Where(static store => !string.IsNullOrWhiteSpace(store))) {
			if (uniqueStores.Add(store)) {
				AvailableStores.Add(store);
			}
		}

		var matchingSelection = AvailableStores.FirstOrDefault(store => StoreNamesEqual(store, previousSelection)) ?? AllStoresLabel;
		SetSelectedStoreInternal(matchingSelection);
	}

	private void EnsureSelectedStoreIsValid() {
		if (AvailableStores.Count == 0) {
			RefreshAvailableStores();
			return;
		}

		if (AvailableStores.Any(store => StoreNamesEqual(store, _selectedStore))) {
			return;
		}

		SetSelectedStoreInternal(AllStoresLabel);
	}

	private void SetSelectedStoreInternal(string value) {
		if (string.Equals(_selectedStore, value, StringComparison.Ordinal)) {
			return;
		}

		_isUpdatingSelection = true;
		try {
			_selectedStore = value;
			OnPropertyChanged(nameof(SelectedStore));
		} finally {
			_isUpdatingSelection = false;
		}
	}

	private bool FilterByStore(ProductViewModel product) {
		if (SelectedStore == AllStoresLabel) {
			return true;
		}

		return string.Equals(product.StoreName, SelectedStore, StringComparison.OrdinalIgnoreCase);
	}

	private static bool StoreNamesEqual(string left, string right)
		=> string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
}
