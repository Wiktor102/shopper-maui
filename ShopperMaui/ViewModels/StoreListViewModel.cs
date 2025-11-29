using ShopperMaui.Helpers;
using ShopperMaui.Models;
using ShopperMaui.ViewModels.Commands;
using System.Collections.ObjectModel;

namespace ShopperMaui.ViewModels;

public class StoreListViewModel : BaseViewModel {
	private readonly MainViewModel _mainViewModel;
	private string _selectedStore;
	private SortingPreference _currentSorting = SortingPreference.Category;

	public StoreListViewModel(MainViewModel mainViewModel) {
		_mainViewModel = mainViewModel;
		Title = "Widok sklepu";
		_selectedStore = AllStoresLabel;

		FilteredProducts = new ObservableCollection<ProductViewModel>();
		AvailableStores = new List<string> { AllStoresLabel };
		AvailableStores.AddRange(Constants.DefaultStores);

		SortByCategoryCommand = new RelayCommand(() => UpdateSorting(SortingPreference.Category));
		SortByNameCommand = new RelayCommand(() => UpdateSorting(SortingPreference.Name));
		SortByQuantityCommand = new RelayCommand(() => UpdateSorting(SortingPreference.Quantity));

		_mainViewModel.ShoppingListUpdated += (_, _) => RefreshProducts();
	}

	private const string AllStoresLabel = "Wszystkie sklepy";

	public ObservableCollection<ProductViewModel> FilteredProducts { get; }

	public List<string> AvailableStores { get; }

	public string SelectedStore {
		get => _selectedStore;
		set {
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

	private void UpdateSorting(SortingPreference preference) {
		CurrentSorting = preference;
	}

	private void RefreshProducts() {
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

	private bool FilterByStore(ProductViewModel product) {
		if (SelectedStore == AllStoresLabel) {
			return true;
		}

		return string.Equals(product.StoreName, SelectedStore, StringComparison.OrdinalIgnoreCase);
	}
}
