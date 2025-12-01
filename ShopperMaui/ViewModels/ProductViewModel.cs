using ShopperMaui.Helpers;
using ShopperMaui.Models;
using ShopperMaui.Services;
using ShopperMaui.ViewModels.Commands;
using System.Collections.ObjectModel;

namespace ShopperMaui.ViewModels;

public class ProductViewModel : BaseViewModel {
	private readonly Product _model;
	private CategoryViewModel _category;
	private readonly IDialogService _dialogService;
	private readonly Func<ProductViewModel, Task>? _onChanged;
	private readonly Func<ProductViewModel, Task>? _onPurchasedChanged;
	private readonly Func<ProductViewModel, Task>? _onDelete;
	private readonly Func<ProductViewModel, Task>? _onEdit;

	private string _name;
	private decimal _quantity;
	private string _unit;
	private bool _isPurchased;
	private bool _isOptional;
	private string? _storeName;

	public ProductViewModel(
		Product model,
		CategoryViewModel category,
		IDialogService dialogService,
		Func<ProductViewModel, Task>? onChanged,
		Func<ProductViewModel, Task>? onPurchasedChanged,
		Func<ProductViewModel, Task>? onDelete,
		Func<ProductViewModel, Task>? onEdit = null) {
		_model = model;
		_category = category;
		_dialogService = dialogService;
		_onChanged = onChanged;
		_onPurchasedChanged = onPurchasedChanged;
		_onDelete = onDelete;
		_onEdit = onEdit;

		_name = _model.Name;
		_quantity = _model.Quantity;
		_unit = _model.Unit;
		_isPurchased = _model.IsPurchased;
		_isOptional = _model.IsOptional;
		_storeName = _model.StoreName;

		IncreaseQuantityCommand = new RelayCommand(IncreaseQuantity);
		DecreaseQuantityCommand = new RelayCommand(DecreaseQuantity, () => Quantity > 0);
		TogglePurchasedCommand = new RelayCommand(TogglePurchased);
		ToggleOptionalCommand = new RelayCommand(ToggleOptional);
		DeleteProductCommand = new AsyncRelayCommand(DeleteProductAsync);
		EditProductCommand = new AsyncRelayCommand(EditProductAsync);
		ClearStoreCommand = new RelayCommand(() => StoreName = null);
	}

	public Product Model => _model;

	public CategoryViewModel ParentCategory => _category;

	public string CategoryName => _category.Name;

	public string Name {
		get => _name;
		set {
			if (SetProperty(ref _name, value)) {
				_model.Name = value;
				_ = NotifyChangedAsync();
			}
		}
	}

	public decimal Quantity {
		get => _quantity;
		set {
			if (SetProperty(ref _quantity, value)) {
				_model.Quantity = value;
				_ = NotifyChangedAsync();
				DecreaseQuantityCommand.RaiseCanExecuteChanged();
			}
		}
	}

	public string Unit {
		get => _unit;
		set {
			if (SetProperty(ref _unit, value)) {
				_model.Unit = value;
				_ = NotifyChangedAsync();
			}
		}
	}

	public bool IsPurchased {
		get => _isPurchased;
		set {
			if (SetProperty(ref _isPurchased, value)) {
				_model.IsPurchased = value;
				_ = NotifyPurchasedChangedAsync();
			}
		}
	}

	public bool IsOptional {
		get => _isOptional;
		set {
			if (SetProperty(ref _isOptional, value)) {
				_model.IsOptional = value;
				_ = NotifyChangedAsync();
			}
		}
	}

	public string? StoreName {
		get => _storeName;
		set {
			if (SetProperty(ref _storeName, value)) {
				_model.StoreName = value;
				_ = NotifyChangedAsync();
				OnPropertyChanged(nameof(HasStore));
				OnPropertyChanged(nameof(StoreDisplayText));
			}
		}
	}

	public bool HasStore => !string.IsNullOrWhiteSpace(StoreName);

	public string StoreDisplayText => string.IsNullOrWhiteSpace(StoreName)
		? "Brak sklepu"
		: StoreName!;

	public IReadOnlyList<string> AvailableUnits => Constants.AvailableUnits;

	public ObservableCollection<string> AvailableStores => _category.AvailableStores;

	public RelayCommand IncreaseQuantityCommand { get; }

	public RelayCommand DecreaseQuantityCommand { get; }

	public RelayCommand TogglePurchasedCommand { get; }

	public RelayCommand ToggleOptionalCommand { get; }

	public AsyncRelayCommand DeleteProductCommand { get; }

	public AsyncRelayCommand EditProductCommand { get; }

	public RelayCommand ClearStoreCommand { get; }

	public void RefreshCategoryName() => OnPropertyChanged(nameof(CategoryName));

	internal void UpdateParentCategory(CategoryViewModel category) {
		_category = category;
		OnPropertyChanged(nameof(ParentCategory));
		OnPropertyChanged(nameof(CategoryName));
		OnPropertyChanged(nameof(AvailableStores));
	}

	private void IncreaseQuantity() => Quantity += 1;

	private void DecreaseQuantity() {
		if (Quantity <= 0) {
			return;
		}

		Quantity -= 1;
	}

	private void TogglePurchased() => IsPurchased = !IsPurchased;

	private void ToggleOptional() => IsOptional = !IsOptional;

	private async Task DeleteProductAsync() {
		var confirm = await _dialogService.ShowConfirmAsync("Usuń produkt", $"Czy na pewno chcesz usunąć {Name}?");
		if (!confirm) {
			return;
		}

		if (_onDelete is not null) {
			await _onDelete(this);
		}
	}

	private Task NotifyChangedAsync() => _onChanged?.Invoke(this) ?? Task.CompletedTask;

	private Task NotifyPurchasedChangedAsync() => _onPurchasedChanged?.Invoke(this) ?? Task.CompletedTask;

	private Task EditProductAsync() => _onEdit?.Invoke(this) ?? Task.CompletedTask;

	internal void UpdateStoreNameFromManager(string? storeName) {
		if (string.Equals(_storeName, storeName, StringComparison.Ordinal)) {
			return;
		}

		_storeName = storeName;
		_model.StoreName = storeName;
		OnPropertyChanged(nameof(StoreName));
		OnPropertyChanged(nameof(HasStore));
		OnPropertyChanged(nameof(StoreDisplayText));
	}
}
