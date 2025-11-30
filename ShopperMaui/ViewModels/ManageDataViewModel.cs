using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ShopperMaui.Services;
using ShopperMaui.ViewModels.Commands;

namespace ShopperMaui.ViewModels;

public class ManageDataViewModel : BaseViewModel {
	private readonly MainViewModel _mainViewModel;
	private readonly INavigationService _navigationService;
	private readonly IDialogService _dialogService;
	private int _selectedTabIndex;

	public ManageDataViewModel(MainViewModel mainViewModel, INavigationService navigationService, IDialogService dialogService) {
		_mainViewModel = mainViewModel;
		_navigationService = navigationService;
		_dialogService = dialogService;

		Title = "Zarządzanie danymi";

		CategoryItems = [];
		StoreItems = [];

		// Category commands
		AddCategoryCommand = new RelayCommand(() => _ = _mainViewModel.NavigateToAddCategoryAsync());

		// Store commands
		AddStoreCommand = new AsyncRelayCommand(AddStoreAsync);

		CloseCommand = new RelayCommand(() => _ = _navigationService.GoBackAsync());
	}

	public int SelectedTabIndex {
		get => _selectedTabIndex;
		set => SetProperty(ref _selectedTabIndex, value);
	}

	// Categories
	public ObservableCollection<EditableItemViewModel> CategoryItems { get; }
	public RelayCommand AddCategoryCommand { get; }

	// Stores
	public ObservableCollection<EditableItemViewModel> StoreItems { get; }
	public AsyncRelayCommand AddStoreCommand { get; }

	public RelayCommand CloseCommand { get; }

	public async Task InitializeAsync() {
		await _mainViewModel.InitializeAsync();
		RebuildCategoryItems();
		RebuildStoreItems();
	}

	public void Attach() {
		_mainViewModel.Categories.CollectionChanged += OnCategoriesChanged;
		_mainViewModel.Stores.CollectionChanged += OnStoresChanged;
		RebuildCategoryItems();
		RebuildStoreItems();
	}

	public void Detach() {
		_mainViewModel.Categories.CollectionChanged -= OnCategoriesChanged;
		_mainViewModel.Stores.CollectionChanged -= OnStoresChanged;
	}

	private void OnCategoriesChanged(object? sender, NotifyCollectionChangedEventArgs e)
		=> RebuildCategoryItems();

	private void OnStoresChanged(object? sender, NotifyCollectionChangedEventArgs e)
		=> RebuildStoreItems();

	private void RebuildCategoryItems() {
		CategoryItems.Clear();
		foreach (var category in _mainViewModel.Categories) {
			var item = new EditableItemViewModel(
				category.Name,
				category.ProductSummary,
				async () => await DeleteCategoryAsync(category),
				(newName) => {
					category.Name = newName;
					return Task.CompletedTask;
				});
			CategoryItems.Add(item);
		}
	}

	private void RebuildStoreItems() {
		StoreItems.Clear();
		foreach (var store in _mainViewModel.Stores) {
			var originalName = store;
			var item = new EditableItemViewModel(
				store,
				null,
				async () => await DeleteStoreAsync(originalName),
				async (newName) => await RenameStoreAsync(originalName, newName));
			StoreItems.Add(item);
		}
	}

	private async Task AddStoreAsync() {
		var storeName = await _dialogService.ShowPromptAsync("Dodaj sklep", "Podaj nazwę sklepu.", "np. Biedronka");
		if (storeName is null) {
			return;
		}

		await _mainViewModel.AddStoreAsync(storeName);
	}

	private async Task RenameStoreAsync(string oldName, string newName) {
		if (string.IsNullOrWhiteSpace(newName) || oldName == newName) {
			return;
		}

		await _mainViewModel.RenameStoreAsync(oldName, newName);
	}

	private async Task DeleteCategoryAsync(CategoryViewModel category) {
		var confirm = await _dialogService.ShowConfirmAsync("Usuń kategorię", $"Czy na pewno chcesz usunąć {category.Name}?");
		if (!confirm) return;
		await _mainViewModel.RemoveCategoryAsync(category);
	}

	private async Task DeleteStoreAsync(string store) {
		var confirm = await _dialogService.ShowConfirmAsync("Usuń sklep", $"Czy na pewno chcesz usunąć sklep \"{store}\"? Wszystkie produkty przypisane do tego sklepu zostaną zaktualizowane.");
		if (!confirm) return;
		await _mainViewModel.RemoveStoreAsync(store);
	}
}
