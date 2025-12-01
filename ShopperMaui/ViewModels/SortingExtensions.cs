using ShopperMaui.Helpers;
using ShopperMaui.Models;

namespace ShopperMaui.ViewModels;

/// <summary>
/// Helper for applying sorting preferences to product lists.
/// Centralizes the switch logic to avoid duplication across view models.
/// </summary>
public static class SortingExtensions {
	/// <summary>
	/// Applies the specified sorting preference to a collection of products.
	/// </summary>
	/// <param name="products">The filtered product list to sort.</param>
	/// <param name="preference">The sorting preference (Category, Name, or Quantity).</param>
	/// <returns>A sorted list of products.</returns>
	public static List<ProductViewModel> ApplySorting(
		this IEnumerable<ProductViewModel> products,
		SortingPreference preference) {
		return preference switch {
			SortingPreference.Name => SortingHelper.SortByName(products),
			SortingPreference.Quantity => SortingHelper.SortByQuantity(products),
			_ => SortingHelper.SortByCategory(products)
		};
	}
}
