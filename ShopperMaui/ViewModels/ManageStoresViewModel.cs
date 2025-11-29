using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ShopperMaui.Services;
using ShopperMaui.ViewModels.Commands;

namespace ShopperMaui.ViewModels;

public class ManageStoresViewModel : BaseViewModel {
	private readonly MainViewModel _mainViewModel;
	private readonly INavigationService _navigationService;
	private readonly IDialogService _dialogService;

	public ManageStoresViewModel(MainViewModel mainViewModel, INavigationService navigationService, IDialogService dialogService) {
		_mainViewModel = mainViewModel;
		_navigationService = navigationService;
		_dialogService = dialogService;

		Title = "Sklepy";

		AddStoreCommand = new AsyncRelayCommand(AddStoreAsync);
		CloseCommand = new RelayCommand(() => _ = _navigationService.GoBackAsync());
		RenameStoreCommand = new AsyncRelayCommand(parameter =>
			parameter is string store ? RenameStoreAsync(store) : Task.CompletedTask);
		DeleteStoreCommand = new AsyncRelayCommand(parameter =>
			parameter is string store ? DeleteStoreAsync(store) : Task.CompletedTask);
	}

	public ObservableCollection<string> Stores => _mainViewModel.Stores;

	public bool HasStores => Stores.Any();

	public AsyncRelayCommand AddStoreCommand { get; }

	public RelayCommand CloseCommand { get; }

	public AsyncRelayCommand RenameStoreCommand { get; }

	public AsyncRelayCommand DeleteStoreCommand { get; }

	public async Task InitializeAsync() {
		await _mainViewModel.InitializeAsync();
		OnPropertyChanged(nameof(HasStores));
	}

	public void Attach() {
		Stores.CollectionChanged += OnStoresChanged;
		OnPropertyChanged(nameof(HasStores));
	}

	public void Detach() {
		Stores.CollectionChanged -= OnStoresChanged;
	}

	private void OnStoresChanged(object? sender, NotifyCollectionChangedEventArgs e)
		=> OnPropertyChanged(nameof(HasStores));

	private async Task AddStoreAsync() {
		var storeName = await _dialogService.ShowPromptAsync("Dodaj sklep", "Podaj nazwę sklepu.", "np. Biedronka");
		if (storeName is null) {
			return;
		}

		await _mainViewModel.AddStoreAsync(storeName);
	}

	private async Task RenameStoreAsync(string store) {
		var newName = await _dialogService.ShowPromptAsync("Zmień nazwę sklepu", $"Nowa nazwa dla \"{store}\".", store);
		if (newName is null) {
			return;
		}

		await _mainViewModel.RenameStoreAsync(store, newName);
	}

	private async Task DeleteStoreAsync(string store) {
		var confirm = await _dialogService.ShowConfirmAsync("Usuń sklep", $"Czy na pewno chcesz usunąć sklep \"{store}\"? Wszystkie produkty przypisane do tego sklepu zostaną zaktualizowane.");
		if (!confirm) {
			return;
		}

		await _mainViewModel.RemoveStoreAsync(store);
	}
}
