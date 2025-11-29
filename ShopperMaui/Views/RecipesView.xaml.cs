using Microsoft.Maui.Controls;
using ShopperMaui.Helpers;
using ShopperMaui.ViewModels;

namespace ShopperMaui.Views;

public partial class RecipesView : ContentPage
{
    private RecipesViewModel ViewModel => (RecipesViewModel)BindingContext;

    public RecipesView()
    {
        InitializeComponent();
        BindingContext = ServiceHelper.GetService<RecipesViewModel>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel.LoadRecipesCommand.ExecuteAsync(null);
    }
}
