using System.Collections.ObjectModel;

namespace ShopperMaui.Models;

public class Category {
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Name { get; set; } = string.Empty;
	public int SortOrder { get; set; }
	public bool IsExpanded { get; set; } = true;
	public ObservableCollection<Product> Products { get; set; } = [];
}
