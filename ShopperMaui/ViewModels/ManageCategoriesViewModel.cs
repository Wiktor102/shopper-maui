using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using ShopperMaui.Services;
using ShopperMaui.ViewModels.Commands;

namespace ShopperMaui.ViewModels;

public class ManageCategoriesViewModel : BaseViewModel {
	private readonly MainViewModel _mainViewModel;
	private readonly INavigationService _navigationService;

	public ManageCategoriesViewModel(MainViewModel mainViewModel, INavigationService navigationService) {
		_mainViewModel = mainViewModel;
		_navigationService = navigationService;

		Title = "Kategorie";

		AddCategoryCommand = new RelayCommand(() => _ = _mainViewModel.NavigateToAddCategoryAsync());
		CloseCommand = new RelayCommand(() => _ = _navigationService.GoBackAsync());
	}

	public ObservableCollection<CategoryViewModel> Categories => _mainViewModel.Categories;

	public RelayCommand AddCategoryCommand { get; }

	public RelayCommand CloseCommand { get; }

	public bool HasCategories => Categories.Any();

	public async Task InitializeAsync() {
		await _mainViewModel.InitializeAsync();
		OnPropertyChanged(nameof(HasCategories));
	}

	public void Attach() {
		_mainViewModel.Categories.CollectionChanged += OnCategoriesChanged;
		OnPropertyChanged(nameof(HasCategories));
	}

	public void Detach() {
		_mainViewModel.Categories.CollectionChanged -= OnCategoriesChanged;
	}

	private void OnCategoriesChanged(object? sender, NotifyCollectionChangedEventArgs e)
		=> OnPropertyChanged(nameof(HasCategories));
}
