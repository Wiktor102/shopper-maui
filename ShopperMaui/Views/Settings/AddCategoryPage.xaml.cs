using ShopperMaui.Helpers;
using ShopperMaui.ViewModels;

namespace ShopperMaui.Views;

public partial class AddCategoryPage : ContentPage {
	public AddCategoryPage() {
		InitializeComponent();
		BindingContext = ServiceHelper.GetService<AddCategoryViewModel>();
	}
}
