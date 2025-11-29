using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ShopperMaui.Helpers;
using ShopperMaui.Models;
using ShopperMaui.ViewModels.Commands;

namespace ShopperMaui.ViewModels;

public class UnpurchasedListViewModel : BaseViewModel
{
	private readonly MainViewModel _mainViewModel;
	private SortingPreference _currentSorting = SortingPreference.Category;

	public UnpurchasedListViewModel(MainViewModel mainViewModel)
	{
		_mainViewModel = mainViewModel;
		Title = "Niezakupione";
		UnpurchasedProducts = new ObservableCollection<ProductViewModel>();

		SortByCategoryCommand = new RelayCommand(() => UpdateSorting(SortingPreference.Category));
		SortByNameCommand = new RelayCommand(() => UpdateSorting(SortingPreference.Name));
		SortByQuantityCommand = new RelayCommand(() => UpdateSorting(SortingPreference.Quantity));

		_mainViewModel.ShoppingListUpdated += (_, _) => RefreshProducts();
	}

	public ObservableCollection<ProductViewModel> UnpurchasedProducts { get; }

	public SortingPreference CurrentSorting
	{
		get => _currentSorting;
		set
		{
			if (SetProperty(ref _currentSorting, value))
			{
				RefreshProducts();
			}
		}
	}

	public RelayCommand SortByCategoryCommand { get; }

	public RelayCommand SortByNameCommand { get; }

	public RelayCommand SortByQuantityCommand { get; }

	public Task InitializeAsync()
	{
		RefreshProducts();
		return Task.CompletedTask;
	}

	private void UpdateSorting(SortingPreference preference)
		=> CurrentSorting = preference;

	private void RefreshProducts()
	{
		var filtered = _mainViewModel
			.GetAllProducts()
			.Where(p => !p.IsPurchased)
			.ToList();

		filtered = CurrentSorting switch
		{
			SortingPreference.Name => SortingHelper.SortByName(filtered),
			SortingPreference.Quantity => SortingHelper.SortByQuantity(filtered),
			_ => SortingHelper.SortByCategory(filtered)
		};

		UnpurchasedProducts.Clear();
		foreach (var product in filtered)
		{
			UnpurchasedProducts.Add(product);
		}
	}
}
