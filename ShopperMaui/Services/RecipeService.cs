using ShopperMaui.Helpers;
using ShopperMaui.Models;

namespace ShopperMaui.Services;

public class RecipeService : IRecipeService {
	public async Task<List<Recipe>> GetRecipesAsync() {
		if (!File.Exists(GetRecipesPath())) {
			var defaults = await GetDefaultRecipesAsync();
			await SaveRecipesCollectionAsync(defaults);
			return defaults;
		}

		var xml = await File.ReadAllTextAsync(GetRecipesPath());
		return XmlSerializationHelper.DeserializeFromXml<List<Recipe>>(xml) ?? new List<Recipe>();
	}

	public async Task SaveRecipeAsync(Recipe recipe) {
		if (recipe is null) {
			throw new ArgumentNullException(nameof(recipe));
		}

		var recipes = await GetRecipesAsync();
		var existingIndex = recipes.FindIndex(r => r.Id == recipe.Id);
		if (existingIndex >= 0) {
			recipes[existingIndex] = recipe;
		} else {
			recipes.Add(recipe);
		}

		await SaveRecipesCollectionAsync(recipes);
	}

	public async Task DeleteRecipeAsync(Guid recipeId) {
		var recipes = await GetRecipesAsync();
		var recipe = recipes.FirstOrDefault(r => r.Id == recipeId);
		if (recipe is null) {
			return;
		}

		recipes.Remove(recipe);
		await SaveRecipesCollectionAsync(recipes);
	}

	public Task<List<Recipe>> GetDefaultRecipesAsync()
		=> Task.FromResult(DefaultDataProvider.GetDefaultRecipes());

	private static async Task SaveRecipesCollectionAsync(List<Recipe> recipes) {
		var xml = XmlSerializationHelper.SerializeToXml(recipes);
		var path = GetRecipesPath();
		Directory.CreateDirectory(Path.GetDirectoryName(path)!);
		await File.WriteAllTextAsync(path, xml);
	}

	private static string GetRecipesPath()
		=> Path.Combine(FileSystem.AppDataDirectory, Constants.RecipesFileName);
}
