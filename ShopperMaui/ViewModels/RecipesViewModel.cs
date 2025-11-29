using ShopperMaui.Helpers;
using ShopperMaui.Models;
using ShopperMaui.Services;
using ShopperMaui.ViewModels.Commands;
using System.Collections.ObjectModel;

namespace ShopperMaui.ViewModels;

public class RecipesViewModel : BaseViewModel {
	private readonly IRecipeService _recipeService;
	private readonly INavigationService _navigationService;
	private readonly MainViewModel _mainViewModel;

	public RecipesViewModel(IRecipeService recipeService, INavigationService navigationService, MainViewModel mainViewModel) {
		_recipeService = recipeService;
		_navigationService = navigationService;
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
	}

	public ObservableCollection<Recipe> Recipes { get; }

	public AsyncRelayCommand LoadRecipesCommand { get; }

	public RelayCommand AddRecipeCommand { get; }

	public RelayCommand EditRecipeCommand { get; }

	public AsyncRelayCommand DeleteRecipeCommand { get; }

	public AsyncRelayCommand AddRecipeToListCommand { get; }

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

		foreach (var ingredient in recipe.Ingredients) {
			var category = _mainViewModel.Categories.FirstOrDefault(c =>
				string.Equals(c.Name, ingredient.CategoryName, StringComparison.OrdinalIgnoreCase));

			if (category is null && !string.IsNullOrWhiteSpace(ingredient.CategoryName)) {
				await _mainViewModel.AddCategoryAsync(ingredient.CategoryName);
				category = _mainViewModel.Categories.FirstOrDefault(c =>
					string.Equals(c.Name, ingredient.CategoryName, StringComparison.OrdinalIgnoreCase));
			}

			if (!_mainViewModel.Categories.Any()) {
				await _mainViewModel.AddCategoryAsync(Constants.DefaultCategories.First());
			}

			var targetCategory = category ?? _mainViewModel.Categories.First();
			var product = new Product {
				Id = Guid.NewGuid(),
				Name = ingredient.ProductName,
				Quantity = ingredient.Quantity,
				Unit = ingredient.Unit,
				IsOptional = false,
				IsPurchased = false,
				CategoryId = targetCategory.Model.Id
			};

			await _mainViewModel.AddProductAsync(targetCategory.Model.Id, product);
		}
	}
}
