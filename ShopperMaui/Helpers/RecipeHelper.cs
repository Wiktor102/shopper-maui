using ShopperMaui.Models;
using ShopperMaui.ViewModels;
using ShopperMaui.Services;

namespace ShopperMaui.Helpers;

public static class RecipeHelper {
	public static async Task AddRecipeToShoppingListAsync(Recipe recipe, MainViewModel mainViewModel, IDialogService dialogService) {
		if (recipe is null) {
			return;
		}

		foreach (var ingredient in recipe.Ingredients) {
			var category = mainViewModel.Categories.FirstOrDefault(c =>
				string.Equals(c.Name, ingredient.CategoryName, StringComparison.OrdinalIgnoreCase));

			if (category is null && !string.IsNullOrWhiteSpace(ingredient.CategoryName)) {
				await mainViewModel.AddCategoryAsync(ingredient.CategoryName);
				category = mainViewModel.Categories.FirstOrDefault(c =>
					string.Equals(c.Name, ingredient.CategoryName, StringComparison.OrdinalIgnoreCase));
			}

			if (!mainViewModel.Categories.Any()) {
				await mainViewModel.AddCategoryAsync(Constants.DefaultCategories.First());
			}

			var targetCategory = category ?? mainViewModel.Categories.First();
			var product = new Product {
				Id = Guid.NewGuid(),
				Name = ingredient.ProductName,
				Quantity = ingredient.Quantity,
				Unit = ingredient.Unit,
				IsOptional = false,
				IsPurchased = false,
				CategoryId = targetCategory.Model.Id
			};

			await mainViewModel.AddProductAsync(targetCategory.Model.Id, product);
		}

		await dialogService.ShowToastAsync($"Dodane składniki z przepisu '{recipe.Name}' do listy zakupów");
		await Shell.Current.GoToAsync("///shoppingList");
	}
}
