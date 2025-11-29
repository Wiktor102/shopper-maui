using Microsoft.Maui.Controls;
using ShopperMaui.Helpers;
using ShopperMaui.ViewModels;

namespace ShopperMaui.Views;

public partial class AddProductPage : ContentPage
{
    public AddProductPage()
    {
        InitializeComponent();
        BindingContext = ServiceHelper.GetService<AddProductViewModel>();
    }
}
