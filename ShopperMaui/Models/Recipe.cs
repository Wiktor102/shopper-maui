namespace ShopperMaui.Models;

public class Recipe {
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public List<RecipeIngredient> Ingredients { get; set; } = new();
}
