using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ShopperMaui.Models;
using ShopperMaui.Services;
using ShopperMaui.ViewModels.Commands;

namespace ShopperMaui.ViewModels;

public class CategoryViewModel : BaseViewModel
{
    private readonly Category _model;
    private readonly MainViewModel _mainViewModel;
    private readonly IDialogService _dialogService;
    private string _name;
    private bool _isExpanded;

    public CategoryViewModel(Category model, MainViewModel mainViewModel, IDialogService dialogService)
    {
        _model = model;
        _mainViewModel = mainViewModel;
        _dialogService = dialogService;
        _name = _model.Name;
        _isExpanded = _model.IsExpanded;

        Products = new ObservableCollection<ProductViewModel>(
            _model.Products
                .OrderBy(p => p.IsPurchased)
                .ThenBy(p => p.SortOrder)
                .ThenBy(p => p.Name)
                .Select(CreateProductViewModel));

        ToggleExpandCommand = new RelayCommand(() => IsExpanded = !IsExpanded);
        AddProductCommand = new RelayCommand(() => _ = _mainViewModel.NavigateToAddProductAsync(this));
        DeleteCategoryCommand = new AsyncRelayCommand(DeleteCategoryAsync);
    }

    public Category Model => _model;

    public ObservableCollection<ProductViewModel> Products { get; }

    public RelayCommand ToggleExpandCommand { get; }

    public RelayCommand AddProductCommand { get; }

    public AsyncRelayCommand DeleteCategoryCommand { get; }

    public string Name
    {
        get => _name;
        set
        {
            if (SetProperty(ref _name, value))
            {
                _model.Name = value;
                foreach (var product in Products)
                {
                    product.RefreshCategoryName();
                }

                _ = _mainViewModel.SaveAsync();
            }
        }
    }

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (SetProperty(ref _isExpanded, value))
            {
                _model.IsExpanded = value;
                _ = _mainViewModel.SaveAsync();
            }
        }
    }

    public void RefreshProductOrdering()
    {
        var ordered = Products
            .OrderBy(p => p.IsPurchased)
            .ThenBy(p => p.Model.SortOrder)
            .ThenBy(p => p.Name)
            .ToList();

        for (var targetIndex = 0; targetIndex < ordered.Count; targetIndex++)
        {
            var product = ordered[targetIndex];
            var currentIndex = Products.IndexOf(product);
            if (currentIndex != targetIndex)
            {
                Products.Move(currentIndex, targetIndex);
            }

            product.Model.SortOrder = targetIndex;
        }
    }

    public void AddProduct(Product product)
    {
        product.CategoryId = _model.Id;
        _model.Products.Add(product);
        var productViewModel = CreateProductViewModel(product);
        Products.Add(productViewModel);
        RefreshProductOrdering();
    }

    public void RemoveProduct(ProductViewModel productViewModel)
    {
        Products.Remove(productViewModel);
        _model.Products.Remove(productViewModel.Model);
        RefreshProductOrdering();
    }

    internal void UpdateSortOrder(int sortOrder) => _model.SortOrder = sortOrder;

    internal void SyncModel()
    {
        _model.Name = Name;
        _model.IsExpanded = IsExpanded;
        _model.Products.Clear();
        foreach (var product in Products)
        {
            _model.Products.Add(product.Model);
        }
    }

    private ProductViewModel CreateProductViewModel(Product product)
        => new(
            product,
            this,
            _dialogService,
            productViewModel => _mainViewModel.HandleProductChangedAsync(productViewModel),
            productViewModel => _mainViewModel.HandleProductChangedAsync(productViewModel),
            productViewModel => _mainViewModel.DeleteProductAsync(productViewModel));

    private async Task DeleteCategoryAsync()
    {
        var confirm = await _dialogService.ShowConfirmAsync("Usuń kategorię", $"Czy na pewno chcesz usunąć {Name}?");
        if (!confirm)
        {
            return;
        }

        await _mainViewModel.RemoveCategoryAsync(this);
    }
}
