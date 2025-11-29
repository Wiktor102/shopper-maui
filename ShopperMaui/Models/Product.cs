namespace ShopperMaui.Models;

public class Product {
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Name { get; set; } = string.Empty;
	public decimal Quantity { get; set; }
	public string Unit { get; set; } = string.Empty;
	public bool IsPurchased { get; set; }
	public bool IsOptional { get; set; }
	public Guid CategoryId { get; set; }
	public string? StoreName { get; set; }
	public int SortOrder { get; set; }
}
