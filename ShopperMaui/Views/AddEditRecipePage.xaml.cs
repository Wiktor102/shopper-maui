using Microsoft.Maui.Controls;
using ShopperMaui.Helpers;
using ShopperMaui.ViewModels;

namespace ShopperMaui.Views;

public partial class AddEditRecipePage : ContentPage
{
    public AddEditRecipePage()
    {
        InitializeComponent();
        BindingContext = ServiceHelper.GetService<AddEditRecipeViewModel>();
    }
}
