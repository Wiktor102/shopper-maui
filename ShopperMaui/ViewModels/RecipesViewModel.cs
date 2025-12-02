using ShopperMaui.Helpers;
using ShopperMaui.Models;
using ShopperMaui.Services.Interfaces;
using ShopperMaui.ViewModels.Commands;
using System.Collections.ObjectModel;

namespace ShopperMaui.ViewModels;

public class RecipesViewModel : BaseViewModel {
	private readonly IRecipeService _recipeService;
	private readonly INavigationService _navigationService;
	private readonly IDialogService _dialogService;
	private readonly MainViewModel _mainViewModel;

	public RecipesViewModel(IRecipeService recipeService, INavigationService navigationService, IDialogService dialogService, MainViewModel mainViewModel) {
		_recipeService = recipeService;
		_navigationService = navigationService;
		_dialogService = dialogService;
		_mainViewModel = mainViewModel;
		Title = "Przepisy";

		Recipes = new ObservableCollection<Recipe>();
		LoadRecipesCommand = new AsyncRelayCommand(LoadRecipesAsync, () => !IsBusy, busy => IsBusy = busy);
		AddRecipeCommand = new RelayCommand(() => _ = _navigationService.NavigateToAsync<AddEditRecipeViewModel>());
		EditRecipeCommand = new RelayCommand(recipe => {
			if (recipe is Recipe model) {
				_ = _navigationService.NavigateToAsync<AddEditRecipeViewModel>(new Dictionary<string, object> {
					["recipeId"] = model.Id
				});
			}
		});
		DeleteRecipeCommand = new AsyncRelayCommand(recipe => DeleteRecipeAsync(recipe as Recipe));
		AddRecipeToListCommand = new AsyncRelayCommand(recipe => AddRecipeToListAsync(recipe as Recipe));
		ViewRecipeDetailsCommand = new RelayCommand(recipe => {
			if (recipe is Recipe model) {
				_ = _navigationService.NavigateToAsync<RecipeDetailsViewModel>(new Dictionary<string, object> {
					["recipeId"] = model.Id
				});
			}
		});
	}

	public ObservableCollection<Recipe> Recipes { get; }

	public AsyncRelayCommand LoadRecipesCommand { get; }

	public RelayCommand AddRecipeCommand { get; }

	public RelayCommand EditRecipeCommand { get; }

	public AsyncRelayCommand DeleteRecipeCommand { get; }

	public AsyncRelayCommand AddRecipeToListCommand { get; }

	public RelayCommand ViewRecipeDetailsCommand { get; }

	public async Task InitializeAsync() {
		if (!Recipes.Any()) {
			await LoadRecipesAsync();
		}
	}

	private async Task LoadRecipesAsync() {
		var recipes = await _recipeService.GetRecipesAsync();
		Recipes.Clear();
		foreach (var recipe in recipes) {
			Recipes.Add(recipe);
		}
	}

	private async Task DeleteRecipeAsync(Recipe? recipe) {
		if (recipe is null) {
			return;
		}

		await _recipeService.DeleteRecipeAsync(recipe.Id);
		Recipes.Remove(recipe);
	}

	private async Task AddRecipeToListAsync(Recipe? recipe) {
		if (recipe is null) {
			return;
		}

		await RecipeHelper.AddRecipeToShoppingListAsync(recipe, _mainViewModel, _dialogService);
	}
}
