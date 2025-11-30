using System.Collections.ObjectModel;

namespace ShopperMaui.Models;

public class ShoppingList {
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Name { get; set; } = "Lista Zakupów";
	public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
	public DateTime LastModified { get; set; } = DateTime.UtcNow;
	public ObservableCollection<Category> Categories { get; set; } = [];
	public ObservableCollection<string> Stores { get; set; } = [];
}
