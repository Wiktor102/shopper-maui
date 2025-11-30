using ShopperMaui.Helpers;
using ShopperMaui.ViewModels;

namespace ShopperMaui.Views;

public partial class RecipeDetailsPage : ContentPage {
	public RecipeDetailsPage() {
		InitializeComponent();
		BindingContext = ServiceHelper.GetService<RecipeDetailsViewModel>();
	}
}
