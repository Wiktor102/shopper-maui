using ShopperMaui.Models;
using ShopperMaui.ViewModels.Commands;

namespace ShopperMaui.ViewModels;

/// <summary>
/// Contract for view models that support user-selectable sorting of product lists.
/// Consolidates common sorting state and commands.
/// </summary>
public interface ISortableProductsViewModel {
	/// <summary>
	/// Gets or sets the current sorting preference.
	/// Implementations should refresh their product collection when this changes.
	/// </summary>
	SortingPreference CurrentSorting { get; set; }

	/// <summary>
	/// Command to sort by product category.
	/// </summary>
	RelayCommand SortByCategoryCommand { get; }

	/// <summary>
	/// Command to sort by product name.
	/// </summary>
	RelayCommand SortByNameCommand { get; }

	/// <summary>
	/// Command to sort by product quantity.
	/// </summary>
	RelayCommand SortByQuantityCommand { get; }
}
