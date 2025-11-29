using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShopperMaui.Helpers;
using ShopperMaui.Models;
using ShopperMaui.Services;
using ShopperMaui.ViewModels.Commands;

namespace ShopperMaui.ViewModels;

public class ProductViewModel : BaseViewModel
{
    private readonly Product _model;
    private readonly CategoryViewModel _category;
    private readonly IDialogService _dialogService;
    private readonly Action<ProductViewModel>? _onChanged;
    private readonly Action<ProductViewModel>? _onPurchasedChanged;
    private readonly Func<ProductViewModel, Task>? _onDelete;

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
        Action<ProductViewModel>? onChanged,
        Action<ProductViewModel>? onPurchasedChanged,
        Func<ProductViewModel, Task>? onDelete)
    {
        _model = model;
        _category = category;
        _dialogService = dialogService;
        _onChanged = onChanged;
        _onPurchasedChanged = onPurchasedChanged;
        _onDelete = onDelete;

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
    }

    public Product Model => _model;

    public CategoryViewModel ParentCategory => _category;

    public string CategoryName => _category.Name;

    public string Name
    {
        get => _name;
        set
        {
            if (SetProperty(ref _name, value))
            {
                _model.Name = value;
                NotifyChanged();
            }
        }
    }

    public decimal Quantity
    {
        get => _quantity;
        set
        {
            if (SetProperty(ref _quantity, value))
            {
                _model.Quantity = value;
                NotifyChanged();
                DecreaseQuantityCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string Unit
    {
        get => _unit;
        set
        {
            if (SetProperty(ref _unit, value))
            {
                _model.Unit = value;
                NotifyChanged();
            }
        }
    }

    public bool IsPurchased
    {
        get => _isPurchased;
        set
        {
            if (SetProperty(ref _isPurchased, value))
            {
                _model.IsPurchased = value;
                NotifyPurchasedChanged();
            }
        }
    }

    public bool IsOptional
    {
        get => _isOptional;
        set
        {
            if (SetProperty(ref _isOptional, value))
            {
                _model.IsOptional = value;
                NotifyChanged();
            }
        }
    }

    public string? StoreName
    {
        get => _storeName;
        set
        {
            if (SetProperty(ref _storeName, value))
            {
                _model.StoreName = value;
                NotifyChanged();
            }
        }
    }

    public IReadOnlyList<string> AvailableUnits => Constants.AvailableUnits;

    public IReadOnlyList<string> AvailableStores => Constants.DefaultStores;

    public RelayCommand IncreaseQuantityCommand { get; }

    public RelayCommand DecreaseQuantityCommand { get; }

    public RelayCommand TogglePurchasedCommand { get; }

    public RelayCommand ToggleOptionalCommand { get; }

    public AsyncRelayCommand DeleteProductCommand { get; }

    public void RefreshCategoryName() => OnPropertyChanged(nameof(CategoryName));

    private void IncreaseQuantity() => Quantity += 1;

    private void DecreaseQuantity()
    {
        if (Quantity <= 0)
        {
            return;
        }

        Quantity -= 1;
    }

    private void TogglePurchased() => IsPurchased = !IsPurchased;

    private void ToggleOptional() => IsOptional = !IsOptional;

    private async Task DeleteProductAsync()
    {
        var confirm = await _dialogService.ShowConfirmAsync("Usuń produkt", $"Czy na pewno chcesz usunąć {Name}?");
        if (!confirm)
        {
            return;
        }

        if (_onDelete is not null)
        {
            await _onDelete(this);
        }
    }

    private void NotifyChanged() => _onChanged?.Invoke(this);

    private void NotifyPurchasedChanged() => _onPurchasedChanged?.Invoke(this);
}
