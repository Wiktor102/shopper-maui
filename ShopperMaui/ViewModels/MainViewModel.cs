using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Storage;
using ShopperMaui.Helpers;
using ShopperMaui.Models;
using ShopperMaui.Services;
using ShopperMaui.ViewModels.Commands;

namespace ShopperMaui.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly IDataService _dataService;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;
    private ShoppingList _currentShoppingList = new();
    private bool _isInitialized;

    public MainViewModel(IDataService dataService, INavigationService navigationService, IDialogService dialogService)
    {
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

    public ObservableCollection<CategoryViewModel> Categories { get; }

    public ShoppingList CurrentShoppingList
    {
        get => _currentShoppingList;
        private set => SetProperty(ref _currentShoppingList, value);
    }

    public RelayCommand AddCategoryCommand { get; }

    public RelayCommand NavigateToUnpurchasedViewCommand { get; }

    public RelayCommand NavigateToRecipesCommand { get; }

    public AsyncRelayCommand ExportListCommand { get; }

    public AsyncRelayCommand ImportListCommand { get; }

    public async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        await LoadAsync();
        _isInitialized = true;
    }

    public async Task LoadAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            CurrentShoppingList = await _dataService.LoadShoppingListAsync();
            if (!CurrentShoppingList.Categories.Any())
            {
                foreach (var category in DefaultDataProvider.GetDefaultCategories())
                {
                    CurrentShoppingList.Categories.Add(category);
                }
            }

            BuildCategoryViewModels();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    internal async Task SaveAsync()
    {
        foreach (var (category, index) in Categories.Select((category, index) => (category, index)))
        {
            category.UpdateSortOrder(index);
            category.SyncModel();
        }

        CurrentShoppingList.LastModified = DateTime.UtcNow;
        await _dataService.SaveShoppingListAsync(CurrentShoppingList);
    }

    internal Task NavigateToAddProductAsync(CategoryViewModel category)
        => _navigationService.NavigateToAsync<AddProductViewModel>(new Dictionary<string, object>
        {
            ["categoryId"] = category.Model.Id
        });

    internal Task NavigateToAddCategoryAsync()
        => _navigationService.NavigateToAsync<AddCategoryViewModel>();

    internal async Task RemoveCategoryAsync(CategoryViewModel category)
    {
        Categories.Remove(category);
        CurrentShoppingList.Categories.Remove(category.Model);
        await SaveAsync();
    }

    internal async Task DeleteProductAsync(ProductViewModel productViewModel)
    {
        productViewModel.ParentCategory.RemoveProduct(productViewModel);
        await SaveAsync();
    }

    internal async Task HandleProductChangedAsync(ProductViewModel productViewModel)
    {
        productViewModel.ParentCategory.RefreshProductOrdering();
        await SaveAsync();
    }

    private void BuildCategoryViewModels()
    {
        Categories.CollectionChanged -= OnCategoriesChanged;
        Categories.Clear();

        foreach (var category in CurrentShoppingList.Categories.OrderBy(c => c.SortOrder).ThenBy(c => c.Name))
        {
            Categories.Add(new CategoryViewModel(category, this, _dialogService));
        }

        Categories.CollectionChanged += OnCategoriesChanged;
    }

    private void OnCategoriesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        _ = SaveAsync();
    }

    private async Task ExportListAsync()
    {
        var filePath = await _dataService.ExportShoppingListAsync(CurrentShoppingList);
        await Share.RequestAsync(new ShareFileRequest
        {
            Title = "Eksport listy zakupów",
            File = new ShareFile(filePath)
        });
    }

    private async Task ImportListAsync()
    {
        var fileResult = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Wybierz plik listy"
        });

        if (fileResult is null)
        {
            return;
        }

        CurrentShoppingList = await _dataService.ImportShoppingListAsync(fileResult.FullPath);
        BuildCategoryViewModels();
    }

    private Task NavigateToUnpurchasedViewAsync()
        => _navigationService.NavigateToAsync<UnpurchasedListViewModel>();

    private Task NavigateToRecipesAsync()
        => _navigationService.NavigateToAsync<RecipesViewModel>();
}
