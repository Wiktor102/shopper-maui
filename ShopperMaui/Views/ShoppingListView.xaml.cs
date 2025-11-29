using Microsoft.Maui.Controls;
using ShopperMaui.ViewModels;

namespace ShopperMaui.Views;

public partial class ShoppingListView : ContentPage
{
    private MainViewModel ViewModel => (MainViewModel)BindingContext;

    public ShoppingListView(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel.InitializeAsync();
    }
}
