using ShopperMaui.Helpers;
using ShopperMaui.Models;
using ShopperMaui.Services.Interfaces;
using ShopperMaui.ViewModels.Commands;
using System.Collections.ObjectModel;

namespace ShopperMaui.ViewModels;

public class AddEditRecipeViewModel : BaseViewModel, IQueryAttributable {
	private readonly IRecipeService _recipeService;
	private readonly INavigationService _navigationService;
	private Guid? _recipeId;
	private string _recipeName = string.Empty;
	private string _recipeDescription = string.Empty;
	private string _recipeDirections = string.Empty;

	public AddEditRecipeViewModel(IRecipeService recipeService, INavigationService navigationService) {
		_recipeService = recipeService;
		_navigationService = navigationService;
		Title = "Przepis";

		Ingredients = new ObservableCollection<RecipeIngredientViewModel>();
		AddIngredientCommand = new RelayCommand(AddIngredient);
		RemoveIngredientCommand = new RelayCommand(parameter => RemoveIngredient(parameter as RecipeIngredientViewModel));
		SaveRecipeCommand = new AsyncRelayCommand(SaveRecipeAsync, () => !IsBusy, busy => IsBusy = busy);
		CancelCommand = new RelayCommand(() => _ = _navigationService.GoBackAsync());
	}

	public ObservableCollection<RecipeIngredientViewModel> Ingredients { get; }

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

	public RelayCommand AddIngredientCommand { get; }

	public RelayCommand RemoveIngredientCommand { get; }

	public AsyncRelayCommand SaveRecipeCommand { get; }

	public RelayCommand CancelCommand { get; }

	public void ApplyQueryAttributes(IDictionary<string, object> query) {
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
			AddIngredient();
			return;
		}

		var recipes = await _recipeService.GetRecipesAsync();
		var recipe = recipes.FirstOrDefault(r => r.Id == _recipeId);
		if (recipe is null) {
			AddIngredient();
			return;
		}

		RecipeName = recipe.Name;
		RecipeDescription = recipe.Description ?? string.Empty;
		RecipeDirections = recipe.Directions ?? string.Empty;
		foreach (var ingredient in recipe.Ingredients) {
			Ingredients.Add(new RecipeIngredientViewModel(ingredient));
		}

		if (Ingredients.Count == 0) {
			AddIngredient();
		}
	}

	private void AddIngredient()
		=> Ingredients.Add(new RecipeIngredientViewModel(new RecipeIngredient { Quantity = 1, Unit = Constants.AvailableUnits.First() }));

	private void RemoveIngredient(RecipeIngredientViewModel? ingredient) {
		if (ingredient is null) {
			return;
		}

		Ingredients.Remove(ingredient);
		if (Ingredients.Count == 0) {
			AddIngredient();
		}
	}

	private async Task SaveRecipeAsync() {
		if (string.IsNullOrWhiteSpace(RecipeName)) {
			ErrorMessage = "Podaj nazwę przepisu.";
			return;
		}

		if (Ingredients.All(i => string.IsNullOrWhiteSpace(i.ProductName))) {
			ErrorMessage = "Dodaj co najmniej jeden składnik.";
			return;
		}

		if (string.IsNullOrWhiteSpace(RecipeDirections)) {
			ErrorMessage = "Dodaj instrukcje przygotowania.";
			return;
		}

		var recipe = new Recipe {
			Id = _recipeId ?? Guid.NewGuid(),
			Name = RecipeName.Trim(),
			Description = RecipeDescription?.Trim() ?? string.Empty,
			Directions = RecipeDirections?.Trim() ?? string.Empty,
			Ingredients = Ingredients.Select(i => i.Model).ToList()
		};

		await _recipeService.SaveRecipeAsync(recipe);
		ErrorMessage = string.Empty;
		await _navigationService.GoBackAsync();
	}
}
