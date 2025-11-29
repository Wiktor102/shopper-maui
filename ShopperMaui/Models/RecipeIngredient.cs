namespace ShopperMaui.Models;

public class RecipeIngredient
{
    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; } = 1;
    public string Unit { get; set; } = string.Empty;
    public string? CategoryName { get; set; }
}
