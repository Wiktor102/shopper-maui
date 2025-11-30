using ShopperMaui.Helpers;
using ShopperMaui.Models;
using ShopperMaui.Services;
using ShopperMaui.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ShopperMaui.ViewModels;

public class RecipeDetailsViewModel : BaseViewModel, IQueryAttributable {
	private readonly IRecipeService _recipeService;
	private readonly INavigationService _navigationService;
	private readonly MainViewModel _mainViewModel;
	private Guid? _recipeId;
	private Recipe? _currentRecipe;
	private string _recipeName = string.Empty;
	private string _recipeDescription = string.Empty;
	private string _recipeDirections = string.Empty;

	public RecipeDetailsViewModel(IRecipeService recipeService, INavigationService navigationService, MainViewModel mainViewModel) {
		_recipeService = recipeService;
		_navigationService = navigationService;
		_mainViewModel = mainViewModel;
		Title = "Przepis";
		Ingredients = new ObservableCollection<RecipeIngredient>();
		AddToShoppingListCommand = new AsyncRelayCommand(AddRecipeToListAsync, () => _currentRecipe is not null, busy => IsBusy = busy);
		DeleteRecipeCommand = new AsyncRelayCommand(DeleteRecipeAsync, () => _currentRecipe is not null, busy => IsBusy = busy);
		EditRecipeCommand = new RelayCommand(_ => NavigateToEdit(), _ => _currentRecipe is not null);
	}

	public ObservableCollection<RecipeIngredient> Ingredients { get; }

	public AsyncRelayCommand AddToShoppingListCommand { get; }

	public AsyncRelayCommand DeleteRecipeCommand { get; }

	public RelayCommand EditRecipeCommand { get; }

	public string RecipeName {
		get => _recipeName;
		set => SetProperty(ref _recipeName, value);
	}

	public string RecipeDescription {
		get => _recipeDescription;
		set => SetProperty(ref _recipeDescription, value);
	}

	public string RecipeDirections {
		get => _recipeDirections;
		set => SetProperty(ref _recipeDirections, value);
	}

	public void ApplyQueryAttributes(IDictionary<string, object> query) {
		_recipeId = null;
		_currentRecipe = null;
		if (query.TryGetValue("recipeId", out var value)) {
			if (value is Guid guid) {
				_recipeId = guid;
			} else if (value is string str && Guid.TryParse(str, out var parsed)) {
				_recipeId = parsed;
			}
		}

		_ = LoadRecipeAsync();
	}

	private async Task LoadRecipeAsync() {
		Ingredients.Clear();
		if (_recipeId is null) {
			_currentRecipe = null;
			RecipeName = string.Empty;
			RecipeDescription = string.Empty;
			RecipeDirections = string.Empty;
			Title = "Przepis";
			UpdateActionStates();
			return;
		}

		var recipes = await _recipeService.GetRecipesAsync();
		var recipe = recipes.FirstOrDefault(r => r.Id == _recipeId);
		if (recipe is null) {
			_currentRecipe = null;
			RecipeName = string.Empty;
			RecipeDescription = string.Empty;
			RecipeDirections = string.Empty;
			Title = "Przepis";
			UpdateActionStates();
			return;
		}

		_currentRecipe = recipe;
		RecipeName = recipe.Name ?? string.Empty;
		RecipeDescription = recipe.Description ?? string.Empty;
		RecipeDirections = recipe.Directions ?? string.Empty;
		Title = string.IsNullOrWhiteSpace(recipe.Name) ? "Przepis" : recipe.Name;

		foreach (var ingredient in recipe.Ingredients) {
			Ingredients.Add(ingredient);
		}

		UpdateActionStates();
	}

	private async Task AddRecipeToListAsync() {
		var recipe = _currentRecipe;
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

	private async Task DeleteRecipeAsync() {
		if (_currentRecipe is null) {
			return;
		}

		await _recipeService.DeleteRecipeAsync(_currentRecipe.Id);
		await _navigationService.GoBackAsync();
	}

	private void NavigateToEdit() {
		if (_currentRecipe is null) {
			return;
		}

		_ = _navigationService.NavigateToAsync<AddEditRecipeViewModel>(new Dictionary<string, object> {
			["recipeId"] = _currentRecipe.Id
		});
	}

	private void UpdateActionStates() {
		AddToShoppingListCommand.RaiseCanExecuteChanged();
		DeleteRecipeCommand.RaiseCanExecuteChanged();
		EditRecipeCommand.RaiseCanExecuteChanged();
	}
}
