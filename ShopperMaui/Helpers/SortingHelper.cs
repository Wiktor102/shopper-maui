using ShopperMaui.ViewModels;

namespace ShopperMaui.Helpers;

public static class SortingHelper {
	public static List<ProductViewModel> SortByCategory(IEnumerable<ProductViewModel> products)
		=> products
			.OrderBy(p => p.CategoryName)
			.ThenBy(p => p.Name)
			.ToList();

	public static List<ProductViewModel> SortByName(IEnumerable<ProductViewModel> products)
		=> products
			.OrderBy(p => p.Name)
			.ThenBy(p => p.CategoryName)
			.ToList();

	public static List<ProductViewModel> SortByQuantity(IEnumerable<ProductViewModel> products)
		=> products
			.OrderByDescending(p => p.Quantity)
			.ThenBy(p => p.Name)
			.ToList();
}
