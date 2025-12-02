using Microsoft.Extensions.Logging;
using ShopperMaui.Helpers;
using ShopperMaui.Services;
using ShopperMaui.Services.Interfaces;
using ShopperMaui.ViewModels;

namespace ShopperMaui;

public static class MauiProgram {
	public static MauiApp CreateMauiApp() {
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts => {
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddSingleton<IDataService, XmlDataService>();
		builder.Services.AddSingleton<IRecipeService, RecipeService>();
		builder.Services.AddSingleton<IDialogService, DialogService>();
		builder.Services.AddSingleton<INavigationService, NavigationService>();
#if WINDOWS
		builder.Services.AddSingleton<IFileSaver, ShopperMaui.Platforms.Windows.FileSaverService>();
#else
		builder.Services.AddSingleton<IFileSaver, FileSaverService>();
#endif

		builder.Services.AddSingleton<MainViewModel>();
		builder.Services.AddTransient<StoreListViewModel>();
		builder.Services.AddTransient<UnpurchasedListViewModel>();
		builder.Services.AddTransient<AddProductViewModel>();
		builder.Services.AddTransient<AddCategoryViewModel>();
		builder.Services.AddTransient<RecipesViewModel>();
		builder.Services.AddTransient<AddEditRecipeViewModel>();
		builder.Services.AddTransient<RecipeDetailsViewModel>();
		builder.Services.AddTransient<ManageDataViewModel>();
		builder.Services.AddTransient<SettingsViewModel>();

		builder.Services.AddSingleton<AppShell>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		var app = builder.Build();
		ServiceHelper.Services = app.Services;
		return app;
	}
}
