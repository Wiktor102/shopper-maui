using ShopperMaui.Services;
using ShopperMaui.ViewModels;
using ShopperMaui.Views;

namespace ShopperMaui;

public partial class AppShell : Shell {
	public AppShell(INavigationService navigationService) {
		InitializeComponent();
		RegisterRoutes(navigationService);
	}

	private static void RegisterRoutes(INavigationService navigationService) {
		RegisterRoute<AddProductViewModel, AddProductPage>(navigationService, nameof(AddProductPage));
		RegisterRoute<AddCategoryViewModel, AddCategoryPage>(navigationService, nameof(AddCategoryPage));
		RegisterRoute<AddEditRecipeViewModel, AddEditRecipePage>(navigationService, nameof(AddEditRecipePage));
		RegisterRoute<ManageDataViewModel, ManageDataPage>(navigationService, nameof(ManageDataPage));
		RegisterRoute<UnpurchasedListViewModel, UnpurchasedListView>(navigationService, nameof(UnpurchasedListView));

		navigationService.RegisterRoute<RecipesViewModel>("//recipesTab");
		navigationService.RegisterRoute<MainViewModel>("//shoppingList");
		navigationService.RegisterRoute<StoreListViewModel>("//storeList");
		navigationService.RegisterRoute<SettingsViewModel>("//settings");
	}

	private static void RegisterRoute<TViewModel, TPage>(INavigationService navigationService, string route)
		where TPage : Page {
		Routing.RegisterRoute(route, typeof(TPage));
		navigationService.RegisterRoute<TViewModel>(route);
	}
}
