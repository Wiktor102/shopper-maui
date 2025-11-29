namespace ShopperMaui.Helpers;

public static class Constants {
	public const string ShoppingListFileName = "shopping_list.xml";
	public const string RecipesFileName = "recipes.xml";

	public static readonly List<string> DefaultCategories = new()
	{
		"Nabiał",
		"Warzywa",
		"Owoce",
		"Mięso",
		"Pieczywo",
		"Chemia"
	};

	public static readonly List<string> DefaultStores = new()
	{
		"Biedronka",
		"Lidl",
		"Kaufland",
		"Auchan",
		"Carrefour",
		"Żabka"
	};

	public static readonly List<string> AvailableUnits = new()
	{
		"szt.",
		"kg",
		"g",
		"l",
		"ml",
		"opakowanie"
	};
}
